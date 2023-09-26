using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Pool;

[RequireComponent(typeof(NavMeshAgent))]
public class Monster : Character
{
	[ReadOnly(true)][SerializeField] protected GameObject m_TargetObj;
	[ReadOnly(true)][SerializeField] protected Renderer m_Renderer;

	private NavMeshAgent m_NavAgent;
	private WaitForSeconds m_UpdateTime = new WaitForSeconds(.1f);
	private bool m_VisibleTarget;
	private float m_Timer;
	private float m_HitTime = 2f;
	private IObjectPool<Monster> m_Pool;
	private WaitForSeconds m_PathUpdateTimer = new WaitForSeconds(.3f);
	private LayerMask m_PlayerMask;

	protected Rigidbody m_Player;
	protected Player m_PlayerObj;
	protected bool m_NavUpdate = true;
	protected bool m_UseRangeAttack;
	protected bool m_PlayerLook; // 한번이라도 플레이어를 향해 바라본 경우
	protected float m_PlayerDist = 1f;

	public bool IsEnabled { get { return m_Renderer.enabled; } }

	protected override void Destroy()
	{
		base.Destroy();

		if (m_Pool != null)
			m_Pool.Release(this);
	}

	public void SetPool(IObjectPool<Monster> pool)
	{
		m_Pool = pool;
	}

	public override void DieAnim()
	{
		base.DieAnim();

		m_NavUpdate = false;

		m_TargetObj.gameObject.SetActive(false);

		StageManager.DeactiveList(this);

		m_NavAgent.enabled = false;
	}

	private void OnCollisionEnter(Collision collision)
	{
		if (m_Dead)
			return;

		if (collision.gameObject.CompareTag("Player"))
		{
			m_UseRangeAttack = false;
			m_NavUpdate = false;

			IDamageable damageableObj = m_Player.GetComponent<IDamageable>();
			damageableObj.TakeDamage(m_CharInfo.Damage);
		}
	}

	private void OnCollisionStay(Collision collision)
	{
		if (m_Dead)
			return;

		if (collision.gameObject.CompareTag("Player"))
		{
			m_Timer += m_deltaTime;

			if (m_Timer >= m_HitTime)
			{
				m_Timer = 0f;

				IDamageable damageableObj = m_Player.GetComponent<IDamageable>();
				damageableObj.TakeDamage(m_CharInfo.Damage);
			}
		}
	}

	private void OnCollisionExit(Collision collision)
	{
		if (m_Dead)
			return;

		if (collision.gameObject.CompareTag("Player"))
		{
			m_UseRangeAttack = true;
			m_NavUpdate = true;

			m_Timer = 0f;
		}
	}

	public void SetVisibleTarget(bool visible)
	{
		m_VisibleTarget = visible;
	}

	private IEnumerator UpdatePath()
	{
		Vector3 targetPos = Vector3.zero;

		while (!m_Dead)
		{
			if (!StageManager.IsPlayerDeath && m_NavAgent.isOnNavMesh)
			{
				if ((m_NavAgent.pathPending || m_NavUpdate) && m_NavAgent.isStopped)
					m_NavAgent.isStopped = false;

				targetPos.x = m_Player.position.x;
				targetPos.z = m_Player.position.z;

				if (m_NavUpdate)
					m_NavAgent.SetDestination(targetPos);

				else
					m_NavAgent.isStopped = true;

				targetPos = (targetPos - Pos).normalized;
				targetPos.y = 0f;

				if (targetPos != Vector3.zero)
					transform.rotation = Quaternion.LookRotation(targetPos);
			}

			yield return m_PathUpdateTimer;
		}

		m_NavAgent.enabled = false;
	}

	private IEnumerator VisibleTarget()
	{
		while (!m_Dead)
		{
			m_TargetObj.SetActive(m_VisibleTarget);

			yield return m_UpdateTime;
		}
	}

	protected override void Awake()
	{
		base.Awake();

		m_NavAgent = GetComponent<NavMeshAgent>();
		m_NavAgent.speed = m_CharInfo.MoveSpeed;
		m_NavAgent.updateRotation = false; // 회전 업데이트 속도가 너무 느리므로 비활성화 후 코루틴에서 회전을 업데이트 하게 한다.

#if UNITY_EDITOR
		if (!m_TargetObj)
			Debug.LogError("if (!m_TargetObj)");

		if (!m_Renderer)
			Debug.LogError("if (!m_Renderer)");
#endif

		m_PlayerObj = StageManager.Player;
		m_Player = StageManager.Player.Rigidbody;

		m_AudioClip = AudioManager.MonsterClip;

		m_PlayerMask = StageManager.PlayerMask;
	}

	private IEnumerator CheckPlayerLook()
	{
		while (!m_PlayerLook && m_PlayerObj != null)
		{
			m_PlayerDist = Vector3.Distance(m_Player.position, SpawnerPos);
			// 만약 몬스터 시점에서 플레이어의 방향이라면 그때부터 m_PlayerLook를 활성화하여 RangeBase 에서 사격을 허용한다.
			// 이것을 해주는 이유는 몬스터가 생기자마자 엉뚱한 방향으로 총알을 발사하는 것을 방지하기 위함이다.
			if (Physics.Raycast(new Ray(SpawnerPos, transform.forward), m_PlayerDist, m_PlayerMask, QueryTriggerInteraction.Collide))
				m_PlayerLook = true;

			yield return null;
		}
	}

	protected override void OnEnable()
	{
		base.OnEnable();

		transform.localPosition = Vector3.zero;

		m_Dead = false;
		m_CharInfo.Heal(1f);

		m_Renderer.enabled = false;
		m_HPBar.gameObject.SetActive(false);

		StartCoroutine(CheckInNavPath());
	}

	private IEnumerator CheckInNavPath()
	{
		if (!m_NavAgent.enabled)
			m_NavAgent.enabled = true;

		while (!m_NavAgent.isOnNavMesh)
		{
			gameObject.transform.position = StageManager.RandomSpawnPos;

			yield return null;
		}

		m_HPBar.gameObject.SetActive(true);

		m_NavUpdate = true;
		m_Renderer.enabled = true;

		if (m_Spawner)
			StartCoroutine(CheckPlayerLook());

		StartCoroutine(UpdatePath());
		StartCoroutine(VisibleTarget());
	}

	protected override void OnDisable()
	{
		base.OnDisable();

		if (!m_Quit)
			m_NavAgent.enabled = true;
	}
}

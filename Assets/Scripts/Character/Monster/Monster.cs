using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Monster : Character
{
	[ReadOnly(true)][SerializeField] protected GameObject m_TargetObj;

	private NavMeshAgent m_NavAgent;
	private WaitForSeconds m_UpdateTime = new WaitForSeconds(.1f);
	private bool m_VisibleTarget;
	private bool m_SetPos;
	private float m_Timer;
	private float m_HitTime = 2f;
	private Vector3 m_NextPos;

	protected Rigidbody m_Player;
	protected bool m_UseUpdatePath = true;
	protected bool m_UseRangeAttack;

	public override void DieAnim()
	{
		base.DieAnim();

		m_TargetObj.gameObject.SetActive(false);

		StageManager.DeactiveList(this);
	}

	public void SetPos(Vector3 pos)
	{
		m_SetPos = true;

		m_NextPos = pos;
	}

	private void OnCollisionEnter(Collision collision)
	{
		if (m_Dead)
			return;

		if (collision.gameObject.CompareTag("Player"))
		{
			m_UseRangeAttack = false;
			m_UseUpdatePath = false;

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
			m_UseUpdatePath = true;

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
			if (!StageManager.IsPlayerDeath)
			{
				targetPos.x = m_Player.position.x;
				targetPos.z = m_Player.position.z;

				if (m_UseUpdatePath)
				{
					m_NavAgent.isStopped = false;

					m_NavAgent.SetDestination(targetPos);
				}

				else
					m_NavAgent.isStopped = true;

				targetPos = (targetPos - m_Rig.position).normalized;
				targetPos.y = 0f;

				if (targetPos != Vector3.zero)
					transform.rotation = Quaternion.LookRotation(targetPos);
			}

			yield return null;
		}
	}

	private IEnumerator VisibleTarget()
	{
		while (!m_Dead)
		{
			m_TargetObj.SetActive(m_VisibleTarget);

			yield return m_UpdateTime;
		}
	}

	protected override void Init()
	{
		base.Init();

		if (m_SetPos)
		{
			m_SetPos = false;

			transform.position = m_NextPos;
		}
	}

	protected override void Awake()
	{
		base.Awake();

		m_NavAgent = GetComponent<NavMeshAgent>();
		m_NavAgent.speed = m_CharInfo.MoveSpeed;
		m_NavAgent.updateRotation = false; // 회전 업데이트 속도가 너무 느리므로 비활성화 후 코루틴에서 회전을 업데이트 하게 한다.

		if (!m_TargetObj)
			Debug.LogError("if (!m_TargetObj)");

		m_Player = StageManager.Player.Rigidbody;
	}

	protected override void Destroy()
	{
		base.Destroy();

		m_NavAgent.ResetPath();
		m_NavAgent.isStopped = true;
	}

	protected override void OnEnable()
	{
		base.OnEnable();

		StartCoroutine(UpdatePath());
		StartCoroutine(VisibleTarget());
	}
}

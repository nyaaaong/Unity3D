using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Monster : Character
{
	protected GameObject m_TargetObj;
	protected Renderer m_Renderer;
	protected NavMeshAgent m_NavAgent;
	private WaitForSeconds m_UpdateTime = new WaitForSeconds(.1f);
	private bool m_VisibleTarget;
	private float m_Timer;
	private float m_HitTime = 0.5f;
	private LayerMask m_PlayerMask;
	private float m_AttackTimer = 0f;
	private WaitUntil m_Playing;
	private float m_NearDist = 2f; // �÷��̾�� ������ �ִٸ� ���Ÿ� �������� �ʰ� �ؾ� �Ѵ�

	protected Rigidbody m_Player;
	protected Player m_PlayerObj;
	protected bool m_NavUpdate = true;
	protected bool m_CanAttack;
	protected bool m_PlayerLook; // �ѹ��̶� �÷��̾ ���� �ٶ� ���
	protected bool m_Update; // false��� �״� ��� Ȥ�� ���� ó�� �׺���̼� ��� üũ

	public bool IsUpdate => m_Update;

	public float NavMoveSpeed { get => m_NavAgent.speed; set => m_NavAgent.speed = value; }

	private void RespawnMonster()
	{
		StageManager.RespawnMonster();
	}

	private void RemoveMonsterCount()
	{
		StageManager.RemoveMonsterCount();
	}

	public void MonsterInit()
	{
		transform.localPosition = Vector3.zero;
		transform.position = StageManager.RandomSpawnPos;
	}

	protected override void OnEnable()
	{
		base.OnEnable();

		if (m_Type >= Char_Type.Boss1)
			return;

		m_Dead = false;
		m_CharData.Heal(1f);

		m_AttackTimer = m_CharData.FireRateTime;

		m_Renderer.enabled = false;

		m_HPBar.gameObject.SetActive(false);

		if (m_Spawner)
		{
			StartCoroutine(CheckAttackDist());
			StartCoroutine(Attack());
		}

		StartCoroutine(CheckInNavPath());

		if (StageManager.NeedExpUpdate)
			StageManager.NeedExpUpdate = false;
	}

	protected override void OnDisable()
	{
		base.OnDisable();

		if (m_NavAgent)
			m_NavAgent.enabled = true;
	}

	public void SetCharData(CharData data)
	{
		m_CharData.Copy(data);
	}

	// �÷��̾��� ��ġ�� �Ѿ� �����Ÿ� ���� �ִ� ��� ������ ����Ѵ�.
	protected IEnumerator CheckAttackDist()
	{
		while (!m_Dead)
		{
			if (!StageManager.IsPlayerDeath)
				m_CanAttack = Physics.Raycast(new Ray(SpawnerPos, transform.forward), m_CharData.Range, m_PlayerMask, QueryTriggerInteraction.Collide);

			else
				m_CanAttack = false;

			yield return null;
		}
	}

	// ������ ���Ǿ� ������ ���Ͱ� �÷��̾ �ٶ� �� ���� �ð����� �����Ѵ�.
	protected IEnumerator Attack()
	{
		while (!m_Dead)
		{
			if (m_CanAttack && m_PlayerLook)
			{
				m_AttackTimer += Time.deltaTime;

				if (m_AttackTimer >= m_CharData.FireRateTime)
				{
					m_AttackTimer = 0f;

					if (!Physics.Raycast(new Ray(SpawnerPos, m_Player.position), m_NearDist, m_PlayerMask, QueryTriggerInteraction.Collide))
						m_Spawner.AttackEvent();
				}
			}

			yield return null;
		}
	}

	// �÷��̾ ��������� �׺���̼��� �۵��� �� �ִ� ������ �� �÷��̷��� ���󰡵��� �÷��̾� ��ġ�� ���� �ð����� �����Ѵ�.
	protected IEnumerator UpdatePath()
	{
		Vector3 targetPos = Vector3.zero;

		while (!m_Dead)
		{
			if (!StageManager.IsPlayerDeath && m_NavAgent.isOnNavMesh)
			{
				if ((m_NavAgent.pathPending || m_NavUpdate) && m_NavAgent.isStopped)
					m_NavAgent.isStopped = false;

				if (m_PlayerObj == null)
				{
					m_PlayerObj = StageManager.Player;
					m_Player = StageManager.Player.Rigidbody;
				}

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

			yield return null;
		}

		m_NavAgent.enabled = false;
	}

	protected IEnumerator VisibleTarget()
	{
		while (!m_Dead)
		{
			m_TargetObj.SetActive(m_VisibleTarget);

			yield return m_UpdateTime;
		}
	}

	// �÷��̾ �ٶ��� �ʾ��� �� �����Ͽ� �ٸ� �������� �߻��ϴ� ���� �����ϱ� ���� �÷��̾ �ٶ󺸴��� ��� �˻��Ѵ�. �׺���̼� �ý����� ����ϱ� ������ �׳� ���θ� ��� üũ�Ѵ�.
	private IEnumerator CheckPlayerLook()
	{
		while (!m_PlayerLook && m_PlayerObj != null)
		{
			if (Physics.Raycast(new Ray(SpawnerPos, transform.forward), m_CharData.Range, m_PlayerMask, QueryTriggerInteraction.Collide))
				m_PlayerLook = true;

			yield return null;
		}
	}

	private IEnumerator CheckInNavPath()
	{
		if (!m_NavAgent.enabled)
			m_NavAgent.enabled = true;

		if (!m_NavAgent.isOnNavMesh)
		{
			yield return m_Playing;

			RemoveMonsterCount();
			RespawnMonster();

			PoolManager.Clear(m_RootObject);
			OnBeforeDestroy();

			yield break;
		}

		m_HPBar.gameObject.SetActive(true);

		m_NavUpdate = true;
		m_Renderer.enabled = true;
		m_Update = true;

		SetCharData(DataManager.CharData[(int)m_Type]);

		m_HPBar.SetHPBar(HP, HPMax);

		if (m_Spawner)
			StartCoroutine(CheckPlayerLook());

		StartCoroutine(UpdatePath());
		StartCoroutine(VisibleTarget());
	}

	public override void Destroy()
	{
		base.Destroy();

		OnBeforeDestroy();

		if (m_Type < Char_Type.Boss1)
			PoolManager.Release(m_RootObject);

		else
			Destroy(gameObject);
	}

	public override void DieAnim()
	{
		base.DieAnim();

		OnBeforeDestroy();
	}

	private void OnBeforeDestroy()
	{
		m_NavUpdate = false;

		if (m_NavAgent != null)
			m_NavAgent.enabled = false;
		
		m_Update = false;

		if (m_TargetObj != null && m_TargetObj.gameObject != null)
		m_TargetObj.gameObject.SetActive(false);

		StopAllCoroutines();
	}

	private void OnCollisionEnter(Collision collision)
	{
		if (m_Dead)
			return;

		if (collision.gameObject.CompareTag("Player"))
		{
			m_CanAttack = false;
			m_NavUpdate = false;

			m_Timer = 0f;
		}
	}

	private void OnCollisionStay(Collision collision)
	{
		if (m_Dead)
			return;

		if (collision.gameObject.CompareTag("Player"))
		{
			m_Timer += Time.deltaTime;

			if (m_Timer >= m_HitTime)
			{
				m_Timer = 0f;

				IDamageable damageableObj = m_Player.GetComponent<IDamageable>();
				damageableObj.TakeDamage(m_CharData.Damage, collision.contacts[0].point, true);
			}
		}
	}

	private void OnCollisionExit(Collision collision)
	{
		if (m_Dead)
			return;

		if (collision.gameObject.CompareTag("Player"))
		{
			m_CanAttack = true;
			m_NavUpdate = true;
		}
	}

	public void SetVisibleTarget(bool visible)
	{
		m_VisibleTarget = visible;
	}

	protected override void Awake()
	{
		base.Awake();

		m_TargetObj = GetComponentInChildren<SpriteRenderer>().gameObject;
		m_Renderer = GetComponentInChildren<SkinnedMeshRenderer>();

		m_NavAgent = GetComponent<NavMeshAgent>();
		m_NavAgent.speed = m_CharData.MoveSpeed;
		m_NavAgent.updateRotation = false; // ȸ�� ������Ʈ �ӵ��� �ʹ� �����Ƿ� ��Ȱ��ȭ �� �ڷ�ƾ���� ȸ���� ������Ʈ �ϰ� �Ѵ�.

		Utility.CheckEmpty(m_TargetObj, "m_TargetObj");
		Utility.CheckEmpty(m_Renderer, "m_Renderer");

		m_TargetObj.SetActive(false);

		m_PlayerObj = StageManager.Player;
		m_Player = StageManager.Player.Rigidbody;
		m_CharClip = AudioManager.MonsterClip;
		m_PlayerMask = StageManager.PlayerMask;
		m_Playing = new WaitUntil(() => !StageManager.IsPause);
		m_HitClip = AudioManager.EffectClip.MonsterHit;

		OnDeathInstant += RemoveMonsterCount;
	}
}

using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Monster : Character
{
	[SerializeField] protected GameObject m_TargetObj;

	protected NavMeshAgent m_NavAgent;
	protected Rigidbody m_Player;
	protected WaitForSeconds m_UpdateTime = new WaitForSeconds(.1f);
	protected Collider m_Collider;
	protected bool m_VisibleTarget;
	protected bool m_UseUpdatePath = true;
	protected bool m_UseRangeAttack;
	protected float m_Timer;
	protected float m_HitTime = 2f;
	private bool m_SetPos;
	private Vector3 m_NextPos;

	public void SetPos(Vector3 pos)
	{
		m_SetPos = true;

		m_NextPos = pos;
	}

	private void OnCollisionEnter(Collision collision)
	{
		if (collision.gameObject.CompareTag("Player"))
		{
			m_UseRangeAttack = false;
			m_UseUpdatePath = false;

			IDamageable damageableObj = m_Player.GetComponent<IDamageable>();
			damageableObj.TakeDamage(m_Damage);
		}
	}

	private void OnCollisionStay(Collision collision)
	{
		if (collision.gameObject.CompareTag("Player"))
		{
			m_Timer += m_deltaTime;

			if (m_Timer >= m_HitTime)
			{
				m_Timer = 0f;

				IDamageable damageableObj = m_Player.GetComponent<IDamageable>();
				damageableObj.TakeDamage(m_Damage);
			}
		}
	}

	private void OnCollisionExit(Collision collision)
	{
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

		while (true)
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
		while (true)
		{
			m_TargetObj.SetActive(m_VisibleTarget);

			yield return m_UpdateTime;
		}
	}

	protected override void Awake()
	{
		base.Awake();

		m_NavAgent = GetComponent<NavMeshAgent>();
		m_NavAgent.speed = m_MoveSpeed;
		m_NavAgent.updateRotation = false; // 회전 업데이트 속도가 너무 느리므로 비활성화 후 코루틴에서 회전을 업데이트 하게 한다.

		m_Collider = GetComponent<Collider>();

		if (!m_Collider)
			Debug.LogError("if (!m_Collider)");

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

		if (m_SetPos)
		{
			m_SetPos = false;

			transform.position = m_NextPos;
		}
	}
}

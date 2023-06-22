using System.Collections;
using UnityEngine;
using UnityEngine.Pool;

[RequireComponent(typeof(GunController))]
public class RangeMonster : Monster
{
	[SerializeField] protected float m_AttackDist = 5f;

	protected IObjectPool<RangeMonster> m_Pool;
	protected GunController m_GunController;
	protected bool m_UseUpdatePath = true;
	protected bool m_UseAttack;
	protected float m_AttackRate = 1f;
	protected float m_AttackTimer = 0f;
	protected float m_PlayerDist = 1f;

	protected override IEnumerator UpdatePath()
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

				targetPos = (targetPos - transform.position).normalized;
				targetPos.y = 0f;

				if (targetPos != Vector3.zero)
					transform.rotation = Quaternion.LookRotation(targetPos);
			}				

			yield return null;
		}
	}

	protected IEnumerator CheckDist()
	{
		while (true)
		{
			if (!StageManager.IsPlayerDeath)
			{
				m_PlayerDist = (m_Player.position - transform.position).sqrMagnitude;

				if (Mathf.Pow(m_AttackDist, 2) >= m_PlayerDist)
				{
					m_UseAttack = true;
					m_UseUpdatePath = false;
				}

				else
				{
					m_UseAttack = false;
					m_UseUpdatePath = true;
				}
			}

			else
			{
				m_UseAttack = false;
				m_UseUpdatePath = false;
			}

			yield return null;
		}
	}

	protected IEnumerator Attack()
	{
		while (true)
		{
			if (m_UseAttack)
			{
				m_AttackTimer += m_deltaTime;

				if (m_AttackTimer >= m_AttackRate)
				{
					m_AttackTimer = 0f;

					// Mathf.Pow(밑, 지수) 즉 거듭제곱을 구하는 함수이다.
					if (m_PlayerDist < Mathf.Pow(m_AttackDist, 2))
						m_GunController.Shoot(true);

					else
						m_GunController.Shoot(false);
				}

				else
					m_GunController.Shoot(false);
			}

			yield return null;
		}
	}

	protected override void OnEnable()
	{
		base.OnEnable();

		m_GunController = GetComponent<GunController>();
		m_GunController.SetInfo(m_AttackDist, Bullet_Owner.Monster, m_FireRateTime);

		StartCoroutine(CheckDist());
		StartCoroutine(Attack());
	}

	public void SetPool(IObjectPool<RangeMonster> pool)
	{
		m_Pool = pool;
	}

	public override void Destroy()
	{
		base.Destroy();

		if (m_Pool != null)
			m_Pool.Release(this);
	}
}

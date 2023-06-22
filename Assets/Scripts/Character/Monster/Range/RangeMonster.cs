using System.Collections;
using UnityEngine;
using UnityEngine.Pool;

public class RangeMonster : Monster
{
	[SerializeField] protected float m_AttackDist = 5f;

	protected IObjectPool<RangeMonster> m_Pool;
	protected float m_AttackRate = 1f;
	protected float m_AttackTimer = 0f;
	protected float m_PlayerDist = 1f;

	protected IEnumerator CheckDist()
	{
		while (true)
		{
			if (!StageManager.IsPlayerDeath)
			{
				m_PlayerDist = (m_Player.position - transform.position).sqrMagnitude;

				if (Mathf.Pow(m_AttackDist, 2) >= m_PlayerDist)
				{
					m_UseRangeAttack = true;
					m_UseUpdatePath = false;
				}

				else
				{
					m_UseRangeAttack = false;
					m_UseUpdatePath = true;
				}
			}

			else
			{
				m_UseRangeAttack = false;
				m_UseUpdatePath = false;
			}

			yield return null;
		}
	}

	protected IEnumerator Attack()
	{
		while (true)
		{
			if (m_UseRangeAttack)
			{
				m_AttackTimer += m_deltaTime;

				if (m_AttackTimer >= m_AttackRate)
				{
					m_AttackTimer = 0f;

					// Mathf.Pow(밑, 지수) 즉 거듭제곱을 구하는 함수이다.
					if (m_PlayerDist < Mathf.Pow(m_AttackDist, 2))
						m_Gun.Shoot(true);

					else
						m_Gun.Shoot(false);
				}

				else
					m_Gun.Shoot(false);
			}

			yield return null;
		}
	}

	protected override void OnEnable()
	{
		base.OnEnable();

		m_Gun.SetWeaponInfo(m_AttackDist, Bullet_Owner.Monster, m_FireRateTime, m_Damage);

		StartCoroutine(CheckDist());
		StartCoroutine(Attack());
	}

	public void SetPool(IObjectPool<RangeMonster> pool)
	{
		m_Pool = pool;
	}

	protected override void Destroy()
	{
		base.Destroy();

		if (m_Pool != null)
			m_Pool.Release(this);
	}
}

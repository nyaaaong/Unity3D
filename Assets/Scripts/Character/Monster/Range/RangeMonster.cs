using System.Collections;
using UnityEngine;
using UnityEngine.Pool;

public class RangeMonster : Monster
{
	protected LayerMask m_WallMask;
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
				m_PlayerDist = Vector3.Distance(m_Player.position, transform.position);

				bool IsWall = Physics.Raycast(new Ray(transform.position, transform.forward), m_PlayerDist, m_WallMask, QueryTriggerInteraction.Collide);

				if (!IsWall)
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

			Debug.Log("m_UseUpdatePath : " + m_UseUpdatePath);

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
					m_Gun.Shoot(true);

				else
					m_Gun.Shoot(false);
			}

			yield return null;
		}
	}

	protected override void Awake()
	{
		base.Awake();

		m_WallMask = StageManager.WallLayer;
	}

	protected override void OnEnable()
	{
		base.OnEnable();

		m_Gun.SetWeaponInfo(Bullet_Owner.Monster, m_FireRateTime, m_Damage);

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

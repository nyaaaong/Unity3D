using System.Collections;
using UnityEngine;
using UnityEngine.Pool;

public class RangeMonster : Monster
{
	private IObjectPool<RangeMonster> m_Pool;
	private float m_AttackTimer = 0f;
	private float m_PlayerDist = 1f;

	protected IEnumerator CheckDist()
	{
		while (!m_Dead)
		{
			if (!StageManager.IsPlayerDeath)
			{
				m_PlayerDist = Vector3.Distance(m_Player.position, SpawnerPos);

				bool IsWall = Physics.Raycast(new Ray(SpawnerPos, transform.forward), m_PlayerDist, m_WallMask, QueryTriggerInteraction.Collide);

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

			yield return null;
		}
	}

	protected IEnumerator Attack()
	{
		while (!m_Dead)
		{
			if (m_UseRangeAttack)
			{
				m_AttackTimer += m_deltaTime;

				if (m_AttackTimer >= m_FireRateTime)
				{
					m_AttackTimer = 0f;

					m_Spawner.Attack();
				}
			}

			else
				m_AttackTimer = m_FireRateTime;

			yield return null;
		}
	}

	protected override void Awake()
	{
		base.Awake();

		m_Spawner.SetSpawnInfo(Bullet_Type.Range, m_Damage);
	}

	protected override void OnEnable()
	{
		base.OnEnable();

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

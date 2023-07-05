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
				m_PlayerDist = Vector3.Distance(m_Player.position, m_Rig.position);

				bool IsWall = Physics.Raycast(new Ray(m_Rig.position, transform.forward), m_PlayerDist, m_WallMask, QueryTriggerInteraction.Collide);

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
		while (true)
		{
			if (m_UseRangeAttack)
			{
				m_AttackTimer += m_deltaTime;

				if (m_AttackTimer >= m_AttackRate)
					m_Spawner.Attack(true);

				else
					m_Spawner.Attack(false);
			}

			yield return null;
		}
	}

	protected override void Awake()
	{
		base.Awake();

		m_WallMask = StageManager.WallLayer;

		m_Spawner.SetSpawnInfo(Bullet_Type.Range, m_FireRateTime, m_Damage);
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

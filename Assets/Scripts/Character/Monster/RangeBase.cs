using System.Collections;
using UnityEngine;

public class RangeBase : Monster
{
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
					m_NavUpdate = false;
				}

				else
				{
					m_UseRangeAttack = false;
					m_NavUpdate = true;
				}
			}

			else
			{
				m_UseRangeAttack = false;
				m_NavUpdate = false;
			}

			yield return null;
		}
	}

	protected IEnumerator Attack()
	{
		while (!m_Dead)
		{
			if (m_UseRangeAttack && m_PlayerLook)
			{
				m_AttackTimer += m_deltaTime;

				if (m_AttackTimer >= m_CharInfo.FireRateTime)
				{
					m_AttackTimer = 0f;

					m_Spawner.Attack();
				}
			}

			else
				m_AttackTimer = m_CharInfo.FireRateTime;

			yield return null;
		}
	}

	protected override void OnEnable()
	{
		base.OnEnable();

		m_AttackTimer = m_CharInfo.FireRateTime;

		StartCoroutine(CheckDist());
		StartCoroutine(Attack());
	}
}
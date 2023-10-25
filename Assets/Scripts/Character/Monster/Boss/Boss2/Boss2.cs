
using System.Collections.Generic;
using UnityEngine;

public class Boss2 : Boss
{
	private float m_SlowAttackDur = 10f;
	private float m_SlowAttackDelay = 0.5f;
	private float m_SlowAttackAngleStep = 7f;
	private float m_SlowFireSpeed = 3f;
	private int m_SlowAttackCount;

	private float m_TornadoDur = 10f;
	private float m_TornadoDelay = 0.1f;
	private float m_TornadoAngle = 0f;
	private float m_TornadoAngleStep = 2f;
	private bool m_TornadoReverse;

	private Quaternion m_SpawnerRot;
	private float m_FireSpeed;

	protected override void PatternEndEvent()
	{
		base.PatternEndEvent();

		RemoveAllBulletAngle();

		m_Spawner.transform.rotation = m_SpawnerRot;
		m_Spawner.FireSpeed = m_FireSpeed;
		m_SlowAttackCount = 0;
		m_TornadoReverse = false;
	}

	private void TornadorInit()
	{
		// 8방향을 각도로 지정한다
		AddBulletAngle(0f);

		for (int i = 1; i < 4; ++i)
		{
			AddBulletAngle(i * 45f);
			AddBulletAngle(i * -45f);
		}

		AddBulletAngle(180f);
	}

	private void TornadorLoop()
	{
		m_Spawner.transform.rotation = Quaternion.Euler(0f, m_TornadoAngle, 0f);

		if (m_TornadoAngle >= 90f)
			m_TornadoReverse = !m_TornadoReverse;

		m_TornadoAngle = m_TornadoReverse ? m_TornadoAngle - m_TornadoAngleStep : m_TornadoAngle + m_TornadoAngleStep;

		m_Spawner.AttackEvent();
	}

	private void SlowAttackInit()
	{
		AddBulletAngle(0f);
		AddBulletAngle(60f);
		AddBulletAngle(-60f);
		AddBulletAngle(120f);
		AddBulletAngle(-120f);
		AddBulletAngle(180f);

		m_Spawner.FireSpeed = m_SlowFireSpeed;
	}

	private void SlowAttackLoop()
	{
		m_Spawner.transform.rotation = Quaternion.Euler(0f, m_SlowAttackCount * m_SlowAttackAngleStep, 0f);

		++m_SlowAttackCount;

		m_Spawner.AttackEvent();
	}

	protected override void OnEnable()
	{
		base.OnEnable();

		AddPattern(30f, m_SlowAttackDur, SlowAttackInit, SlowAttackLoop, m_SlowAttackDelay);
		AddPattern(30f, m_TornadoDur, TornadorInit, TornadorLoop, m_TornadoDelay);
	}

	protected override void Awake()
	{
		base.Awake();

		m_UseNavSystem = false;

		m_SpawnerRot = m_Spawner.transform.rotation;
		m_FireSpeed = m_CharData.FireSpeed;
	}
}

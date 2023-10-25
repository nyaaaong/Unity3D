
using System.Collections.Generic;
using UnityEngine;

public class Boss2 : Boss
{
	private float m_ManyAttackDur = 4f;
	private float m_ManyAttackDelay = 1f;
	private bool m_ManyAttackChangeRot;
	private float m_ManyAttackRotAngle = 30f;

	private float m_MonsterSpawnDur = 5f;
	private float m_MonsterSpawnLoopDelay = 1f;
	private int m_MonsterSpawnBase;
	private int m_MonsterSpawnCount = 0;
	private int m_MonsterSpawnCountMax = 10;
	private IReadOnlyList<GameObject> m_MonsterPrefebList;

	private float m_TornadoDur = 10f;
	private float m_TornadoDelay = 0.1f;
	private float m_TornadoAngle = 0f;

	private Quaternion m_SpawnerRot;

	protected override void PatternEndEvent()
	{
		base.PatternEndEvent();

		m_MonsterSpawnCount = 0;
		m_ManyAttackChangeRot = false;
		RemoveAllBulletAngle();

		m_Spawner.transform.rotation = m_SpawnerRot;
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

		m_TornadoAngle = m_TornadoAngle >= 360f ? 0f : m_TornadoAngle + 2f;

		m_Spawner.AttackEvent();
	}

	private void MonsterSpawnAndAttackInit()
	{
		// 몬스터를 스폰할 Base 개수는 1~2 랜덤으로 뽑아준다.
		m_MonsterSpawnBase = Random.Range(1, 3);
	}

	private void MonsterSpawnAndAttackLoop()
	{
		if (StageManager.MonsterCount < m_MonsterSpawnCountMax)
			StageManager.RequestMonsterSpawn(m_MonsterPrefebList[Random.Range(0, m_MonsterPrefebList.Count)], m_MonsterSpawnBase + m_MonsterSpawnCount++);

		else
			PatternSkip();
	}

	private void ManyAttackInit()
	{
		AddBulletAngle(0f);
		AddBulletAngle(60f);
		AddBulletAngle(-60f);
		AddBulletAngle(120f);
		AddBulletAngle(-120f);
		AddBulletAngle(180f);
	}

	private void ManyAttackLoop()
	{
		m_ManyAttackChangeRot = !m_ManyAttackChangeRot;

		if (m_ManyAttackChangeRot)
			m_Spawner.transform.rotation = Quaternion.Euler(0f, m_ManyAttackRotAngle, 0f);

		else
			m_Spawner.transform.rotation = m_SpawnerRot;

		m_Spawner.AttackEvent();
	}

	protected override void OnEnable()
	{
		base.OnEnable();

		AddPattern(30f, m_ManyAttackDur, ManyAttackInit, ManyAttackLoop, m_ManyAttackDelay);
		AddPattern(30f, m_TornadoDur, TornadorInit, TornadorLoop, m_TornadoDelay);
		AddPattern(30f, m_MonsterSpawnDur, MonsterSpawnAndAttackInit, MonsterSpawnAndAttackLoop, m_MonsterSpawnLoopDelay);
	}

	protected override void Awake()
	{
		base.Awake();

		m_UseNavSystem = false;

		m_MonsterPrefebList = StageManager.MonsterPrefebList;

		m_SpawnerRot = m_Spawner.transform.rotation;
	}
}

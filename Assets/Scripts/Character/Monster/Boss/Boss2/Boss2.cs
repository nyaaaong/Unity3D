
using System.Collections.Generic;
using UnityEngine;

public class Boss2 : Boss
{
	private float m_TriangleDur = 1f;
	private float m_TriangleDelay = 0.5f;
	private int m_TriangleAttackCount;

	private float m_ManyAttackDur = 3f;
	private float m_ManyAttackDelay = 1f;
	private int m_ManyAttackCount = 0;

	private float m_MonsterSpawnDur = 5f;
	private float m_MonsterSpawnLoopDelay = 1f;
	private int m_MonsterSpawnBase;
	private int m_MonsterSpawnCount = 0;
	private int m_MonsterSpawnCountMax = 30;
	private float m_MonsterSpawnMultiplier = 1.3f;
	private IReadOnlyList<GameObject> m_MonsterPrefebList;

	protected override void PatternEndEvent()
	{
		base.PatternEndEvent();

		m_MonsterSpawnCount = 0;
		m_TriangleAttackCount = 0;
		m_ManyAttackCount = 0;
		RemoveAllBulletAngle();
	}

	private void MonsterSpawnAndAttackInit()
	{
		// 몬스터를 스폰할 Base 개수는 1~3 랜덤으로 뽑아준다.
		m_MonsterSpawnBase = Random.Range(1, 4);

		// 스폰된 몬스터가 Max에 도달하면 총알을 쏴야 하므로 총알 각도 등록
		TriangleInit();
		ReverseTriangleInit();
	}

	private void MonsterSpawnAndAttackLoop()
	{
		// Base + Count * m_MonsterSpawnMultiplier 만큼 몬스터를 소환한다. Count는 소환이 거듭될 수록 증가한다.
		if (StageManager.MonsterCount < m_MonsterSpawnCountMax)
			StageManager.RequestMonsterSpawn(m_MonsterPrefebList[Random.Range(0, m_MonsterPrefebList.Count)], Mathf.CeilToInt(m_MonsterSpawnBase + m_MonsterSpawnCount++ + m_MonsterSpawnMultiplier));

		else
			PatternSkip();
	}

	private void TriangleInit()
	{
		AddBulletAngle(0f);
		AddBulletAngle(120f);
		AddBulletAngle(-120f);
	}

	private void ReverseTriangleInit()
	{
		AddBulletAngle(180f);
		AddBulletAngle(60f);
		AddBulletAngle(-60f);
	}

	private void TriangleAttackLoop()
	{
		if (m_TriangleAttackCount % 3 == 0)
		{
			RemoveAllBulletAngle();
			ReverseTriangleInit();
		}

		++m_TriangleAttackCount;
		m_Spawner.AttackEvent();
	}

	private void ManyAttackLoop()
	{
		++m_ManyAttackCount;

		if (m_TriangleAttackCount % 2 == 0)
		{
			RemoveAllBulletAngle();
			ReverseTriangleInit();
		}

		else
		{
			RemoveAllBulletAngle();
			TriangleInit();
		}

		m_Spawner.AttackEvent();
	}

	protected override void OnEnable()
	{
		base.OnEnable();

		AddPattern(40f, m_TriangleDur, TriangleInit, TriangleAttackLoop, m_TriangleDelay);
		AddPattern(30f, m_ManyAttackDur, TriangleInit, ManyAttackLoop, m_ManyAttackDelay);
		AddPattern(10f, m_MonsterSpawnDur, MonsterSpawnAndAttackInit, MonsterSpawnAndAttackLoop, m_MonsterSpawnLoopDelay);
	}

	protected override void Awake()
	{
		base.Awake();

		m_UseNavSystem = false;

		m_MonsterPrefebList = StageManager.MonsterPrefebList;
	}
}

﻿using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using static UnityEngine.GraphicsBuffer;

public class Stage : BaseScript
{
	[SerializeField] private Wave[] m_Waves;

	[Serializable]
	public class Wave
	{
		public int m_EnemyCount;
		public float m_SpawnTime;
	}

	private Player m_Player;
	private IObjectPool<MeleeMonster> m_MeleePool;
	private IObjectPool<RangeMonster> m_RangePool;
	private LinkedList<Monster> m_ActiveList;
	private float m_Timer;
	private float m_NextSpawnTime;
	private Wave m_Wave;
	private int m_WaveNum;
	private int m_EnemyCount;
	private int m_NeedSpawnCount;
	private bool m_NeedUpdate = true;
	private bool m_PlayerDeath;
	private Monster m_Target;
	private bool m_StageClear;
	private MapGenerator m_Map;

	public event Action OnStageClear;

	public bool IsEnemyEmpty { get { return m_EnemyCount == 0; } }
	public bool IsPlayerDeath { get { return m_PlayerDeath; } }
	public bool IsStageClear { get { return m_StageClear; } }
	public Player Player { get { return m_Player; } }
	public Vector2Int MapSize { get { return new Vector2Int(m_Map.MapSize.x, m_Map.MapSize.y); } }

	public void DeactiveList(Monster monster)
	{
		if (monster.IsDead())
			DeleteList(monster);
	}

	public void SetInvisibleTarget(Monster monster)
	{
		if (m_Target == monster)
			m_Target = null;

		monster.SetVisibleTarget(false);
	}

	public void SetVisibleTarget(Monster monster)
	{
		// 다른 타겟으로 변경될 때 이전 타겟의 발판을 제거시킨다.
		if (m_Target != monster)
		{
			if (m_Target)
				m_Target.SetVisibleTarget(false);

			m_Target = monster;
			m_Target.SetVisibleTarget(true);
		}
	}

	private void OnMonsterDeath()
	{
		--m_EnemyCount;

		if (m_NeedSpawnCount == 0 && m_EnemyCount == 0)
		{
			if (m_EnemyCount == 0 && m_NeedUpdate)
			{
				// 스킬창을 띄우고 스킬이 찍히면 다음 웨이브로

				if (m_Waves.Length > m_WaveNum)
					NextWave();

				else
				{
					m_NeedUpdate = false;
					m_StageClear = true;

					Debug.Log("스테이지 클리어");
				}
			}
		}
	}

	private void OnPlayerDeath()
	{
		if (!m_StageClear)
		{
			m_PlayerDeath = true;
			m_NeedUpdate = false;

			Debug.Log("플레이어 사망");
		}
	}

	private void NextWave()
	{
		++m_WaveNum;

		m_Wave = m_Waves[m_WaveNum - 1];
		m_NeedSpawnCount = m_Wave.m_EnemyCount;
		m_EnemyCount = m_NeedSpawnCount;
		m_NextSpawnTime = m_Wave.m_SpawnTime;

		Debug.Log("현재 웨이브 : " + m_WaveNum);
	}

	// ref readonly 를 사용하여 m_ActiveList 읽기전용, 참조로 내보낸다.
	public ref readonly LinkedList<Monster> GetActiveMonsters()
	{
		return ref m_ActiveList;
	}

	private MeleeMonster CreateMeleeMonster()
	{
		MeleeMonster monster = Instantiate(StageManager.MeleeObjPrefeb).GetComponentInChildren<MeleeMonster>();
		monster.SetPool(m_MeleePool);

		return monster;
	}

	private RangeMonster CreateRangeMonster()
	{
		RangeMonster monster = Instantiate(StageManager.RangeObjPrefeb).GetComponentInChildren<RangeMonster>();
		monster.SetPool(m_RangePool);

		return monster;
	}

	private void OnGetMonster(Monster monster)
	{
		m_ActiveList.AddLast(monster);

		monster.transform.root.gameObject.SetActive(true);
	}

	private void OnReleaseMonster(Monster monster)
	{
		monster.transform.root.gameObject.SetActive(false);
	}

	private void OnDestroyMonster(Monster monster)
	{
		if (monster)
		{
			DeleteList(monster);

			if (monster.transform.root.gameObject)
				Destroy(monster.transform.root.gameObject);
		}
	}

	private void DeleteList(Monster monster)
	{
		var node = m_ActiveList.Find(monster);

		if (node != null)
			m_ActiveList.Remove(node);
	}

	private void OnDestroy()
	{
		if (OnStageClear == null)
			return;

		OnStageClear();

		m_MeleePool.Clear();
		m_RangePool.Clear();

		// 총알들 제거
		GameObject[] bulletObjs = GameObject.FindGameObjectsWithTag("Bullet");

		foreach (GameObject obj in bulletObjs)
		{
			Destroy(obj);
		}

		// 몬스터들 제거
		foreach (Monster item in m_ActiveList)
		{
			if (item != null)
				Destroy(item.transform.root.gameObject);
		}

		if (m_ActiveList != null)
			m_ActiveList.Clear();

		if (m_Player)
			m_Player.Die();
	}

	protected override void Awake()
	{
		base.Awake();

		m_Map = FindObjectOfType<MapGenerator>();

		m_MeleePool = new ObjectPool<MeleeMonster>(CreateMeleeMonster, OnGetMonster, OnReleaseMonster, OnDestroyMonster, maxSize: 5);
		m_RangePool = new ObjectPool<RangeMonster>(CreateRangeMonster, OnGetMonster, OnReleaseMonster, OnDestroyMonster, maxSize: 5);

		m_ActiveList = new LinkedList<Monster>();
	}

	protected override void Start()
	{
		base.Start();

		var PlayerObj = StageManager.CreatePlayerObject;

		m_Player = PlayerObj.transform.GetComponentInChildren<Player>();
		m_Player.OnDeath += OnPlayerDeath;

		m_Player.transform.position = Vector3.zero;

		NextWave();
	}

	private Vector3 GetMonsterRandPos()
	{
		return m_Map.GetRandomOpenTile().position;
	}

	protected override void BeforeUpdate()
	{
		if (!m_NeedUpdate)
		{
			Destroy(gameObject);
			return;
		}

		base.BeforeUpdate();

		m_Timer += m_deltaTime;

		Debug.Log("남아있는 적의 수 : " + m_EnemyCount);

		if (m_NeedSpawnCount > 0 && m_Timer > m_NextSpawnTime)
		{
			--m_NeedSpawnCount;
			m_Timer = 0f;

			int percent = UnityEngine.Random.Range(1, 100);

			Monster newMonster;

			if (percent <= 50)
				newMonster = m_MeleePool.Get();

			else
				newMonster = m_RangePool.Get();

			newMonster.SetPos(GetMonsterRandPos());

			if (!newMonster.IsUseOnDeath)
				newMonster.OnDeath += OnMonsterDeath;
		}
	}
}

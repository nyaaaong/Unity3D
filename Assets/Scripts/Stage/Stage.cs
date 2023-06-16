using UnityEngine;
using UnityEngine.Pool;
using System;
using System.Collections.Generic;

public class Stage : BaseScript
{
	[SerializeField] private Monster m_Monster;
	[SerializeField] private Wave[] m_Waves;

	[Serializable] public class Wave
	{
		public int m_EnemyCount;
		public float m_SpawnTime;
	}

	private IObjectPool<Monster> m_Pool;
	private LinkedList<Monster> m_ActiveList;
	private float m_NextSpawnTime;
	private Wave m_CurWave;
	private int m_CurWaveNum;
	private int m_CurEnemyCount;
	private int m_NeedSpawnCount;
	private bool m_NeedUpdate = true;

	public int CurEnemyCount { get { return m_CurEnemyCount; } }

	private void OnMonsterDeath()
	{
		--m_CurEnemyCount;
	}

	private void NextWave()
	{
		++m_CurWaveNum;

		m_CurWave = m_Waves[m_CurWaveNum - 1];
		m_NeedSpawnCount = m_CurWave.m_EnemyCount;
		m_CurEnemyCount = m_NeedSpawnCount;

		Debug.Log("현재 웨이브 : " + m_CurWaveNum);		
	}

	// ref readonly 를 사용하여 m_ActiveList 읽기전용, 참조로 내보낸다.
	public ref readonly LinkedList<Monster> GetActiveMonsters()
	{
		return ref m_ActiveList;
	}

	private Monster CreateMonster()
	{
		Monster monster = Instantiate(m_Monster).GetComponent<Monster>();
		monster.SetPool(m_Pool);

		return monster;
	}

	private void OnGetMonster(Monster monster)
	{
		m_ActiveList.AddLast(monster);

		monster.gameObject.SetActive(true);
		// 스테이지 내 랜덤한 위치에 생성
	}

	private void OnReleaseMonster(Monster monster)
	{
		DeleteList(monster);

		monster.gameObject.SetActive(false);
	}

	private void OnDestroyMonster(Monster monster)
	{
		DeleteList(monster);

		Destroy(monster.gameObject);
	}

	private void DeleteList(Monster monster)
	{
		var node = m_ActiveList.Find(monster);

		if (node != null)
			m_ActiveList.Remove(node);
	}

	protected override void Awake()
	{
		base.Awake();

		m_Pool = new ObjectPool<Monster>(CreateMonster, OnGetMonster, OnReleaseMonster, OnDestroyMonster, maxSize: 20);

		m_ActiveList = new LinkedList<Monster>();
	}

	protected override void Start()
	{
		base.Start();

		NextWave();
	}

	protected override void BeforeUpdate()
	{
		base.BeforeUpdate();

		Debug.Log("남아있는 적의 수 : " + m_CurEnemyCount);

		if (m_NeedSpawnCount > 0 && Time.time > m_NextSpawnTime)
		{
			--m_NeedSpawnCount;
			m_NextSpawnTime = Time.time + m_CurWave.m_SpawnTime;

			Monster newMonster = m_Pool.Get();
			newMonster.SetInfo(Vector3.zero, Quaternion.identity);

			if (!newMonster.IsUseOnDeath)
				newMonster.OnDeath += OnMonsterDeath;
		}

		else if (m_CurEnemyCount == 0 && m_NeedUpdate)
		{
			// 스킬창을 띄우고 스킬이 찍히면 다음 웨이브로

			if (m_Waves.Length > m_CurWaveNum)
				NextWave();

			else
			{
				m_NeedUpdate = false;

				Debug.Log("스테이지 종료");
			}
		}
	}
}

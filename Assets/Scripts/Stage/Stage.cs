using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class Stage : BaseScript
{
	[SerializeField] private Wave[] m_Waves;

	[Serializable] public class Wave
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
	private Monster m_Target;

	public event Action OnStageClear;

	public bool IsEnemyEmpty { get { return m_EnemyCount == 0; } }
	public Player GetPlayer { get { return m_Player; } }

	public void SetVisibleTarget(Monster monster)
	{
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
		MeleeMonster monster = Instantiate(StageManager.GetMeleePrefeb).GetComponent<MeleeMonster>();
		monster.SetPool(m_MeleePool);

		return monster;
	}

	private RangeMonster CreateRangeMonster()
	{
		RangeMonster monster = Instantiate(StageManager.GetRangePrefeb).GetComponent<RangeMonster>();
		monster.SetPool(m_RangePool);

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
		if (monster)
		{
			DeleteList(monster);

			if (monster.gameObject)
				Destroy(monster.gameObject);
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
		OnStageClear();

		m_MeleePool.Clear();
		m_RangePool.Clear();
	}

	protected override void Awake()
	{
		base.Awake();

		m_Player = StageManager.CreatePlayer;

		m_MeleePool = new ObjectPool<MeleeMonster>(CreateMeleeMonster, OnGetMonster, OnReleaseMonster, OnDestroyMonster, maxSize: 20);
		m_RangePool = new ObjectPool<RangeMonster>(CreateRangeMonster, OnGetMonster, OnReleaseMonster, OnDestroyMonster, maxSize: 20);

		m_ActiveList = new LinkedList<Monster>();
	}

	protected override void Start()
	{
		base.Start();

		NextWave();
	}

	protected override void BeforeUpdate()
	{
		if (!m_NeedUpdate)
			Destroy(gameObject);

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

			if (!newMonster)
				Debug.LogError("if (!newMonster)");

			newMonster.SetInfo(Vector3.zero, Quaternion.identity);

			if (!newMonster.IsUseOnDeath)
				newMonster.OnDeath += OnMonsterDeath;
		}

		else if (m_EnemyCount == 0 && m_NeedUpdate)
		{
			// 스킬창을 띄우고 스킬이 찍히면 다음 웨이브로

			if (m_Waves.Length > m_WaveNum)
				NextWave();

			else
			{
				m_NeedUpdate = false;

				Debug.Log("스테이지 종료");
			}
		}
	}
}

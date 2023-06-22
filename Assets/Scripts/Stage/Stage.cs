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

	public class SpawnPoint
	{
		public Vector3 m_LeftUp;
		public Vector3 m_RightDown;
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
	private SpawnPoint m_SpawnPoint;
	private bool m_StageClear;

	public event Action OnStageClear;

	public bool IsEnemyEmpty { get { return m_EnemyCount == 0; } }
	public bool IsPlayerDeath { get { return m_PlayerDeath; } }
	public bool IsStageClear { get { return m_StageClear; } }
	public Player GetPlayer { get { return m_Player; } }

	public void SetSpawnPoint(Spawn_Type type, params Transform[] tr)
	{
		if (tr == null)
			Debug.LogError("if (tr == null)");

		switch (type)
		{
			case Spawn_Type.Player:
				m_Player.transform.position = tr[0].position;
				break;
			case Spawn_Type.Monster:
				if (m_SpawnPoint == null)
					m_SpawnPoint = new SpawnPoint();

				if (tr.Length != 2)
					Debug.LogError("if (tr.Length != 2)");

				m_SpawnPoint.m_LeftUp = tr[0].position;
				m_SpawnPoint.m_RightDown = tr[1].position;
				break;
		}
	}

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
				Destroy(item.gameObject);
		}

		m_ActiveList.Clear();

		if (m_Player)
			m_Player.Die();
	}

	protected override void Awake()
	{
		base.Awake();

		m_Player = StageManager.CreatePlayer;
		m_Player.OnDeath += OnPlayerDeath;

		m_MeleePool = new ObjectPool<MeleeMonster>(CreateMeleeMonster, OnGetMonster, OnReleaseMonster, OnDestroyMonster, maxSize: 5);
		m_RangePool = new ObjectPool<RangeMonster>(CreateRangeMonster, OnGetMonster, OnReleaseMonster, OnDestroyMonster, maxSize: 5);

		m_ActiveList = new LinkedList<Monster>();
	}

	protected override void Start()
	{
		base.Start();

		NextWave();
	}

	private Vector3 GetMonsterRandPos()
	{
		bool IsWall = false;
		Vector3 Result = Vector3.zero;

		do
		{
			Result.x = UnityEngine.Random.Range(m_SpawnPoint.m_LeftUp.x, m_SpawnPoint.m_RightDown.x + 1);
			Result.z = UnityEngine.Random.Range(m_SpawnPoint.m_LeftUp.z, m_SpawnPoint.m_RightDown.z + 1);

			// 벽이 있는지 검사
		} while (IsWall);

		return Result;
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

			Vector3 RandPos = GetMonsterRandPos();

			newMonster.SetCharacterInfo(RandPos, Quaternion.identity);

			if (!newMonster.IsUseOnDeath)
				newMonster.OnDeath += OnMonsterDeath;
		}
	}
}

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class Stage : BaseScript
{
	[SerializeField] private Player m_Player;
	[SerializeField] private Monster m_Monster;
	[SerializeField] private Wave[] m_Waves;

	[Serializable] public class Wave
	{
		public int m_EnemyCount;
		public float m_SpawnTime;
	}

	private IObjectPool<Monster> m_Pool;
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

		m_Pool.Clear();
	}

	protected override void Awake()
	{
		base.Awake();

		m_Pool = new ObjectPool<Monster>(CreateMonster, OnGetMonster, OnReleaseMonster, OnDestroyMonster, maxSize: 20);

		m_ActiveList = new LinkedList<Monster>();

		if (!m_Player)
			Debug.LogError("if (!m_Player)");

		if (!m_Monster)
			Debug.LogError("if (!m_Monster)");

		m_Player = Instantiate(m_Player).GetComponent<Player>();
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

			Monster newMonster = m_Pool.Get();
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

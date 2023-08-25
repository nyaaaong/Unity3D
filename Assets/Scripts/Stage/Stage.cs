using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

[RequireComponent(typeof(AudioSource))]
public class Stage : BaseScript
{
	private Player m_Player;
	private IObjectPool<Monster> m_MonsterPool;
	private LinkedList<Monster> m_ActiveList;
	private Monster m_Target;
	private GameObject[] m_MonsterPrefeb;
	private float m_Timer;
	private float m_NextSpawnTime;
	private int m_NeedSpawnCount;
	private int m_WaveNum;
	private int m_EnemyCount;
	private bool m_NeedUpdate = true;
	private bool m_PlayerDeath;
	private bool m_StageClear;

	public event Action OnStageClear;

	public bool IsEnemyEmpty { get { return m_EnemyCount == 0; } }
	public bool IsPlayerDeath { get { return m_PlayerDeath; } }
	public bool IsStageClear { get { return m_StageClear; } }
	public Player Player { get { return m_Player; } }

	public void NextStage()
	{
		m_NeedUpdate = false;
		m_StageClear = true;

		UIManager.ShowPopup(Popup_Type.StageClear);
	}

	public void PlayAudio()
	{
		AudioManager.PlayStageBGM();
	}

	public void DeactiveList(Monster monster)
	{
		if (monster.IsDead())
			RemoveList(monster);
	}

	public void SetInvisibleTarget(Monster monster)
	{
		if (m_Target == monster)
			m_Target = null;

		if (monster != null)
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

				if (InfoManager.WaveCount > m_WaveNum)
					UIManager.ShowPopup(Popup_Type.Wave, m_WaveNum + 1);

				else
					NextStage();
			}
		}
	}

	private void OnPlayerDeath()
	{
		if (!m_StageClear)
		{
			m_PlayerDeath = true;
			m_NeedUpdate = false;

			if (!m_Quit)
				UIManager.ShowMenu(Menu_Type.Continue);
		}

		else
			InfoManager.PlayerHP = m_Player.HP;
	}

	public void NextWave()
	{
		++m_WaveNum;

		m_NeedSpawnCount = InfoManager.EnemyCount(m_WaveNum);
		m_EnemyCount = m_NeedSpawnCount;
		m_NextSpawnTime = InfoManager.SpawnTime();
	}

	// ref readonly 를 사용하여 m_ActiveList 읽기전용, 참조로 내보낸다.
	public ref readonly LinkedList<Monster> GetActiveMonsters()
	{
		// 보내기 전, 죽은 몬스터인지 검사 및 리스트에서 제거한다.
		foreach (Monster monster in m_ActiveList)
		{
			if (monster.IsDead())
				RemoveList(monster);
		}

		return ref m_ActiveList;
	}

	private Monster CreateMonster()
	{
		Monster monster = Instantiate(m_MonsterPrefeb[UnityEngine.Random.Range(0, m_MonsterPrefeb.Length)],
			StageManager.GetMonsterRandPos(), Quaternion.identity).GetComponentInChildren<Monster>();
		monster.SetPool(m_MonsterPool);

		return monster;
	}

	private void OnGetMonster(Monster monster)
	{
		m_ActiveList.AddLast(monster);

		monster.transform.root.position = StageManager.GetMonsterRandPos();
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
			RemoveList(monster);

			if (monster.transform.root.gameObject)
				Destroy(monster.transform.root.gameObject);
		}
	}

	private void KillAllMonster()
	{
		foreach (Monster monster in m_ActiveList)
			monster.Kill();
	}

	private void RemoveList(Monster monster)
	{
		var node = m_ActiveList.Find(monster);

		if (node != null)
			m_ActiveList.Remove(node);
	}

	protected override void OnDisable()
	{
		if (OnStageClear == null)
			return;

		OnStageClear();

		m_MonsterPool.Clear();

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

		if (m_Player && !m_Player.IsDead())
			m_Player.Die();
	}

	protected override void Awake()
	{
		base.Awake();

		int max = (int)Character_Type.Max;

		// 플레이어는 제외
		m_MonsterPrefeb = new GameObject[max - 1];

		for (int i = (int)Character_Type.Melee; i < max; ++i)
		{
			m_MonsterPrefeb[i - 1] = StageManager.CharObjPrefeb[i];
		}

		m_MonsterPool = new ObjectPool<Monster>(CreateMonster, OnGetMonster, OnReleaseMonster, OnDestroyMonster, maxSize: 20);

		m_ActiveList = new LinkedList<Monster>();
	}

	protected override void Start()
	{
		base.Start();

		var PlayerObj = StageManager.CreatePlayerObject;

		m_Player = PlayerObj.transform.GetComponentInChildren<Player>();
		m_Player.OnDeath += OnPlayerDeath;

		m_Player.transform.position = Vector3.zero;

		PlayAudio();

		UIManager.ShowPopup(Popup_Type.Wave, m_WaveNum + 1);
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

		if (m_NeedSpawnCount > 0 && m_Timer > m_NextSpawnTime)
		{
			--m_NeedSpawnCount;
			m_Timer = 0f;

			int percent = UnityEngine.Random.Range(1, 100);

			Monster newMonster = m_MonsterPool.Get();

			if (!newMonster.IsUseOnDeath)
				newMonster.OnDeath += OnMonsterDeath;
		}
	}
}

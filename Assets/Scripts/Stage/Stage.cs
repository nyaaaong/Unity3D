
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class Stage : BaseScript
{
	private Player m_Player;
	private Monster m_Target;
	private Monster m_Boss;
	private WaitUntil m_WaitDifferentMonsterCount;
	private WaitUntil m_WaitExpBarStop;
	private float m_Timer;
	private float m_NextSpawnTime;
	private int m_NeedSpawnCount; // 스폰되야 할 몬스터 수
	private int m_Wave;
	private int m_MonsterCount; // 현재 남아있는 몬스터 수
	private bool m_NeedUpdate = true;
	private bool m_PlayerDeath;
	private bool m_StageClear;
	private bool m_BossDeath;
	private LinkedList<Monster> m_ActiveList; // 살아있는 몬스터 리스트
	private GameObject m_WaveMonsterPrefeb;

	public event Action OnStageClear;

	public bool IsMonsterEmpty => m_MonsterCount == 0 && m_Boss == null;
	public bool IsPlayerDeath => m_PlayerDeath;
	public bool IsStageClear => m_StageClear;
	public Player Player => m_Player;
	public int Wave => m_Wave;

	private void BossDeath()
	{
		m_BossDeath = true;

		RemoveActiveList(m_Boss);
		// 보스 체력바 UI 비활성화, Exp UI 활성화, 레벨업
		StartCoroutine(AfterBossDeath());
	}

	private IEnumerator AfterBossDeath()
	{
		// 경험치 오르는 거 보여주고 레벨업 확인
		yield return m_WaitExpBarStop;

		NextWave();
	}

	private void CreateBoss()
	{
		m_Boss = Utility.Instantiate(StageManager.GetBoss(), StageManager.RandomSpawnPos, Quaternion.identity).GetComponentInChildren<Monster>();
		m_Boss.OnRespawn += RespawnMonster;
		m_Boss.OnDeath += BossDeath;

		AddActiveList(m_Boss);
		UIManager.SetBossHPOwner(m_Boss);
	}

	// ref readonly 를 사용하여 m_ActiveList 읽기전용, 참조로 내보낸다.
	public ref readonly LinkedList<Monster> GetActiveMonsters()
	{
		// 보내기 전, 죽은 몬스터인지 검사 및 리스트에서 제거한다.
		RemoveDeathList();

		return ref m_ActiveList;
	}

	// 살아있는 몬스터 추가
	public void AddActiveList(Monster monster)
	{
		var node = m_ActiveList.Find(monster);

		if (node == null)
			m_ActiveList.AddLast(monster);
	}

	// 죽어있는 몬스터 제외
	public void RemoveActiveList(Monster monster)
	{
		var node = m_ActiveList.Find(monster);

		if (node != null)
			m_ActiveList.Remove(node);
	}

	private void RemoveDeathList()
	{
		foreach (Monster monster in m_ActiveList)
		{
			if (monster.IsDead())
				m_ActiveList.Remove(monster);
		}
	}

	// 몬스터 개수가 달라질 때 디버그 매니저에도 갱신 시켜준다.
	private IEnumerator RefreshMonsterCountLog()
	{
		while (!m_StageClear)
		{
			DebugManager.MonsterCount = m_MonsterCount;

			yield return m_WaitDifferentMonsterCount;
		}
	}

	public void RespawnMonster()
	{
		++m_NeedSpawnCount;
	}

	private void PlayAudio()
	{
		AudioManager.PlayStageBGM();
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
		--m_MonsterCount;

		if (m_NeedSpawnCount == 0 && m_MonsterCount == 0)
		{
			if (m_MonsterCount == 0 && m_NeedUpdate)
				NextWave();
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
			DataManager.PlayerHP = m_Player.HP;
	}

	public void NextWave()
	{
		if (DataManager.WaveCount > m_Wave)
		{
			++m_Wave;
			StageManager.NeedExpUpdate = true;

			DataManager.RefreshMonsterData();

			m_NeedSpawnCount = DataManager.MonsterCount();
			m_NextSpawnTime = DataManager.SpawnTime();

			m_WaveMonsterPrefeb = StageManager.GetWaveMonsterPrefeb();
		}

		else
		{
			// 여기서 보스를 생성시키고 보스가 죽었는지 판단한다
			// 만약 죽었다면 다음 스테이지로.
			if (!m_BossDeath)
				CreateBoss();

			else
			{
				m_NeedUpdate = false;
				m_StageClear = true;

				NextStage();
				return;
			}
		}

		DebugManager.Wave = m_Wave;
		DebugManager.MonsterCount = m_MonsterCount;
	}

	private void NextStage()
	{
		StageManager.NextStage();
	}

	protected override void OnDisable()
	{
		if (OnStageClear != null)
			OnStageClear();

		// Pool 싹 정리
		PoolManager.ReleaseAll();
	}

	protected override void Awake()
	{
		base.Awake();

		m_WaitDifferentMonsterCount = new WaitUntil(() => DebugManager.MonsterCount != m_MonsterCount);
		m_WaitExpBarStop = new WaitUntil(() => !UIManager.IsBarUpdate && !UIManager.NeedShowAbility);
		m_ActiveList = new LinkedList<Monster>();
	}

	protected override void Start()
	{
		base.Start();

		m_Player = StageManager.Player;
		m_Player.OnDeath += OnPlayerDeath;

		m_Player.transform.position = Vector3.zero;

		PlayAudio();

		NextWave();

		StartCoroutine(RefreshMonsterCountLog());
	}

	protected override void Update()
	{
		if (!m_NeedUpdate)
		{
			Destroy(gameObject);
			return;
		}

		base.Update();

		if (m_NeedSpawnCount > 0)
		{
			m_Timer += Time.deltaTime;

			if (m_Timer > m_NextSpawnTime)
			{
				--m_NeedSpawnCount;
				++m_MonsterCount;

				m_Timer = 0f;

				int percent = UnityEngine.Random.Range(1, 100);

				Monster newMonster = PoolManager.Get(m_WaveMonsterPrefeb, StageManager.RandomSpawnPos, Quaternion.identity).GetComponentInChildren<Monster>();
				newMonster.SetCharData(DataManager.CharData[(int)newMonster.Type]);

				if (!newMonster.HasOnDeath)
					newMonster.OnDeath += OnMonsterDeath;

				if (!newMonster.HasOnRespawn)
					newMonster.OnRespawn += RespawnMonster;
			}
		}
	}
}

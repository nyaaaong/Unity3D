using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage : BaseScript
{
	private Player m_Player;
	private Monster m_Target;
	private Monster m_Boss;
	private float m_Timer;
	private float m_NextSpawnTime;
	private int m_SpawnCount; // 현재 스폰된 몬스터
	private int m_SpawnCountMax; // 한번에 몇 마리 까지 소환될 지
	private int m_WaveSpawnCount; // 스폰되야 할 몬스터 수
	private int m_RespawnCount; // 리스폰되야 할 몬스터 수
	private int m_Wave;
	private bool m_NeedUpdate = true;
	private bool m_PlayerDeath;
	private bool m_StageClear;
	private bool m_NeedCreateBoss;
	private LinkedList<Monster> m_AliveList; // 살아있는 몬스터 리스트
	private GameObject m_WaveMonsterPrefeb;
	private GameObject m_BossPrefeb;
	private Vector3 m_BossSpawnPos;
	private Quaternion m_BossSpawnRot;
	private BossSpawnEffect m_BossSpawnEffect;
	private Boss_State m_BossState;
	private bool m_CompleteBossDeathEvent;
	private WaitForSeconds m_WaitBossClear = new WaitForSeconds(4f);

	public bool IsMonsterEmpty => m_SpawnCount == 0;
	public bool IsPlayerDeath => m_PlayerDeath;
	public bool IsStageClear => m_StageClear;
	public Player Player => m_Player;
	public int Wave => m_Wave;
	public int MonsterAliveCount => m_SpawnCount;
	public Boss_State BossState => m_BossState;

	private void CreateMonster(GameObject monsterPrefeb)
	{
		AddAliveList(PoolManager.Get(monsterPrefeb, StageManager.RandomSpawnPos(m_Player), Quaternion.identity).GetComponentInChildren<Monster>());
	}

	public void RequestMonsterSpawn(GameObject monsterPrefeb, int count)
	{
		for (int i = 0; i < count; ++i)
		{
			CreateMonster(monsterPrefeb);
		}
	}

	public void RemoveMonsterCount()
	{
		--m_SpawnCount;
	}

	private IEnumerator BossDeath()
	{
		AudioManager.PlayBossClearBGM();

		m_BossState = Boss_State.Clear;

		yield return m_WaitBossClear;

		m_CompleteBossDeathEvent = true;
	}

	private void CreateBoss()
	{
		m_BossPrefeb = StageManager.GetBoss();

		if (!m_NeedUpdate || !m_BossPrefeb)
			return;

		m_BossState = Boss_State.Spawn;

		AudioManager.PlayBossBGM();

		m_Boss = Utility.Instantiate(m_BossPrefeb, m_BossSpawnPos, Quaternion.identity).GetComponentInChildren<Monster>();
		m_Boss.AddOnDeathEvent(() => StartCoroutine(BossDeath()));

		AddAliveList(m_Boss);
	}

	private void CreateBossInit()
	{
		if (!m_NeedUpdate)
			return;

		m_NeedCreateBoss = false;
		m_BossState = Boss_State.NeedSpawn;

		m_BossSpawnPos = StageManager.RandomSpawnPos(m_Player);
		m_BossSpawnPos.y = StageManager.SpawnEffectPrefeb.transform.position.y;

		m_BossSpawnEffect = Utility.Instantiate(StageManager.SpawnEffectPrefeb, m_BossSpawnPos, m_BossSpawnRot).GetComponent<BossSpawnEffect>();
		m_BossSpawnEffect.OnAfterDestroy += CreateBoss;

		AudioManager.PlayNeedBossSpawnAudio(() => m_BossSpawnEffect.FadeOut());
	}

	// ref readonly 를 사용하여 m_AliveList 읽기전용, 참조로 내보낸다.
	public ref readonly LinkedList<Monster> GetActiveMonsters()
	{
		LinkedListNode<Monster> node = m_AliveList.First;

		while (node != null)
		{
			if (node.Value.IsDead())
				m_AliveList.Remove(node);

			node = node.Next;
		}

		return ref m_AliveList;
	}

	// 살아있는 몬스터 추가
	private void AddAliveList(Monster monster)
	{
		m_AliveList.AddLast(monster);
		++m_SpawnCount;
	}

	private void RemoveAllAliveList()
	{
		LinkedListNode<Monster> node = m_AliveList.First;

		while (node != null)
		{
			m_AliveList.Remove(node);

			node = node.Next;
		}

		m_SpawnCount = 0;
	}

	public void RespawnMonster()
	{
		++m_RespawnCount;
	}

	private void PlayAudio()
	{
		AudioManager.PlayStageBGM(false);
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

	private void OnPlayerDeath()
	{
		m_PlayerDeath = true;
		m_NeedUpdate = false;

		UIManager.ShowMenu(Menu_Type.Continue);
		// 보스 몬스터 소환 해제
		if (m_BossSpawnEffect)
			Destroy(m_BossSpawnEffect.gameObject);

		if (m_Boss)
		{
			m_AliveList.Remove(m_Boss);
			Destroy(m_Boss.gameObject);
			m_Boss = null;
		}

		RemoveAllAliveList();
		PoolManager.ReleaseAll();

		// 오디오 정지
		AudioManager.StopAllAudio();
	}

	private void WaveUpdate()
	{
		++m_Wave;
		StageManager.NeedExpUpdate = true;

		// 플레이어의 최대 경험치 증가는 레벨업, 웨이브 변경 시 진행되야 한다.
		StageManager.RefreshPlayerExpMax();
		DataManager.RefreshMonsterData();

		m_WaveSpawnCount = DataManager.MonsterCount();
		m_NextSpawnTime = DataManager.SpawnTime();

		m_WaveMonsterPrefeb = StageManager.GetWaveMonsterPrefeb();
	}

	protected override void Awake()
	{
		base.Awake();

		m_AliveList = new LinkedList<Monster>();

		m_BossSpawnRot = StageManager.SpawnEffectPrefeb.transform.rotation;
		m_SpawnCountMax = DataManager.SpawnCount;
	}

	protected override void Start()
	{
		base.Start();

		m_Player = StageManager.Player;
		m_Player.AddOnDeathEvent(OnPlayerDeath);

		PlayAudio();

		m_NeedCreateBoss = true;

		WaveUpdate();
	}

	protected override void Update()
	{
		if (!m_NeedUpdate || m_PlayerDeath || !m_Player.IsUpdate)
			return;

		base.Update();

		if (m_WaveSpawnCount > 0 || m_RespawnCount > 0)
		{
			if (m_SpawnCount < m_SpawnCountMax)
			{
				m_Timer += Time.deltaTime;

				if (m_Timer > m_NextSpawnTime)
				{
					m_Timer = 0f;

					if (m_WaveSpawnCount > 0)
						--m_WaveSpawnCount;

					else if (m_RespawnCount > 0)
						--m_RespawnCount;

					CreateMonster(m_WaveMonsterPrefeb);
				}
			}
		}

		else if (m_RespawnCount == 0 && m_WaveSpawnCount == 0 && m_SpawnCount == 0)
		{
			if (DataManager.WaveCount > m_Wave)
				WaveUpdate();

			else
			{
				// 여기서 보스를 생성시키고 보스가 죽었는지 판단한다
				// 만약 죽었다면 다음 스테이지로.
				if (m_NeedCreateBoss)
					CreateBossInit();

				// 보스 죽음 이벤트가 끝나고, Exp UI 애니메이션과 레벨업이 완전히 끝나면 다음스테이지로 넘어간다.
				else if (m_CompleteBossDeathEvent && !UIManager.NeedUpdate)
				{
					if (!UIManager.NeedLevelUp)
					{
						m_NeedUpdate = false;
						m_StageClear = true;

						StageManager.NextStage();
					}
				}
			}
		}
	}
}

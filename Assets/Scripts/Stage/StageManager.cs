
using System;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : Singleton<StageManager>
{
	[ReadOnly(true)][SerializeField] private GameObject m_StagePrefeb;
	[ReadOnly(true)][SerializeField][EnumArray(typeof(Char_Type))] private GameObject[] m_CharPrefeb = new GameObject[(int)Char_Type.Max];
	[ReadOnly(true)][SerializeField][EnumArray(typeof(Bullet_Type))] private GameObject[] m_BulletPrefeb = new GameObject[(int)Bullet_Type.Max];
	[ReadOnly(true)][SerializeField] private GameObject m_SpawnEffectPrefeb;
	[ReadOnly(true)][SerializeField] private LayerMask m_PlayerMask;
	[ReadOnly(true)][SerializeField] private LayerMask m_MonsterMask;
	[ReadOnly(true)][SerializeField] private LayerMask m_WallMask;
	[ReadOnly(true)][SerializeField] private Map m_Map;

	private Stage m_Stage;
	private int m_StageNum;
	private int m_BossIndex = -1; // 스테이지가 넘어갈때마다 증가하며 보스를 생성해낼때 사용
	private float m_TimeScale;
	private bool m_IsPause;
	private List<GameObject> m_MonsterPrefebList;
	private List<GameObject> m_BossPrefebList;
	private GameObject m_PlayerObj;
	private Player m_Player;
	private bool m_NeedExpUpdate;

	public static Vector3 RandomSpawnPos => Inst.m_Map.RandomSpawnPos;

	public static bool IsMonsterEmpty => Inst.m_Stage.IsMonsterEmpty;
	public static bool IsStageClear => Inst.m_Stage.IsStageClear;

	public static LayerMask PlayerMask => Inst.m_PlayerMask;
	public static LayerMask MonsterMask => Inst.m_MonsterMask;
	public static LayerMask WallMask => Inst.m_WallMask;
	public static Map Map => Inst.m_Map;
	public static bool IsPause => Inst.m_IsPause;

	public static int Stage => Inst.m_StageNum;
	public static int Wave => Inst.m_Stage.Wave;

	public static int MonsterCount => Inst.m_MonsterPrefebList.Count;
	public static int MonsterAliveCount => Inst.m_Stage.MonsterAliveCount;
	public static bool NeedExpUpdate { set => Inst.m_NeedExpUpdate = value; get => Inst.m_NeedExpUpdate; }
	public static GameObject SpawnEffectPrefeb => Inst.m_SpawnEffectPrefeb;
	public static Boss_State BossState
	{
		get
		{
			if (!Inst.m_Stage)
				return Boss_State.None;

			return Inst.m_Stage.BossState;
		}
	}

	public static bool IsPlayerDeath
	{
		get
		{
			if (!Inst.m_Stage)
				return true;
			
			return Inst.m_Stage.IsPlayerDeath;
		}
	}

	public static GameObject PlayerObj
	{
		get
		{
			Inst.CreatePlayer();

			return Inst.m_PlayerObj;
		}
	}

	public static Player Player
	{
		get
		{
			Inst.CreatePlayer();

			return Inst.m_Player;
		}
	}

	public static IReadOnlyList<GameObject> MonsterPrefebList => Inst.m_MonsterPrefebList;

	public static void RequestMonsterSpawn(GameObject monsterPrefeb, int count)
	{
		Inst.m_Stage.RequestMonsterSpawn(monsterPrefeb, count);
	}

	public static void RespawnMonster()
	{
		Inst.m_Stage.RespawnMonster();
	}

	public static void RemoveMonsterCount()
	{
		Inst.m_Stage.RemoveMonsterCount();
	}

	public static GameObject GetBulletPrefeb(Bullet_Type type)
	{
		return Inst.m_BulletPrefeb[(int)type];
	}

	public static GameObject GetWaveMonsterPrefeb()
	{
		return Inst.m_MonsterPrefebList[UnityEngine.Random.Range(0, Inst.m_MonsterPrefebList.Count)];
	}

	public static ref readonly LinkedList<Monster> GetActiveMonsters()
	{
		return ref Inst.m_Stage.GetActiveMonsters();
	}

	private void CreatePlayer()
	{
		if (!m_PlayerObj)
		{
			m_PlayerObj = Utility.Instantiate(m_CharPrefeb[(int)Char_Type.Player]);
			m_Player = m_PlayerObj.GetComponentInChildren<Player>();
			m_Player.transform.position = Vector3.zero;
		}
	}

	public static GameObject GetBoss()
	{
		if (!Inst)
			return null;

		return Inst.m_BossPrefebList[Inst.m_BossIndex].gameObject;
	}

	public static void Pause()
	{
		Time.timeScale = 0f;

		Inst.m_IsPause = true;
	}

	public static void Resume()
	{
		Time.timeScale = Inst.m_TimeScale;

		Inst.m_IsPause = false;
	}

	public static void Cheat(Cheat_Type type, bool isCheck)
	{
		switch (type)
		{
			case Cheat_Type.PowerUp:
			case Cheat_Type.NoHit:
				if (Player)
					Player.Cheat(type, isCheck);
				break;
			case Cheat_Type.Death:
				if (Player)
					Player.Cheat(type, isCheck);
				break;
			case Cheat_Type.AddExp:
				if (isCheck)
					UIManager.AddExp = 150f;
				break;
		}
	}

	public static void SetInvisibleTarget(Monster monster)
	{
		Inst.m_Stage.SetInvisibleTarget(monster);
	}

	public static void SetVisibleTarget(Monster monster)
	{
		Inst.m_Stage.SetVisibleTarget(monster);
	}

	public static void NextStage()
	{
		++Inst.m_StageNum;
		++Inst.m_BossIndex;

		if (Inst.m_Stage)
			Destroy(Inst.m_Stage.gameObject);

		// 보스 인덱스가 보스 프리팹 개수를 넘어가면 0으로 변경
		if (Inst.m_BossIndex == Inst.m_BossPrefebList.Count)
			Inst.m_BossIndex = 0;

		Inst.m_Stage = Utility.Instantiate(Inst.m_StagePrefeb).GetComponent<Stage>();

		if (Inst.m_StageNum == 1)
			DataManager.ResetMonsterData();
	}

	public static void ResetStage()
	{
		Inst.m_StageNum = 0;
		Inst.m_BossIndex = -1;
		DataManager.HealFull();
		UIManager.ResetUI();
		NextStage();
	}

	protected override void Awake()
	{
		base.Awake();

		Utility.CheckEmpty(m_StagePrefeb, "m_StagePrefeb");
		Utility.CheckEmpty(m_CharPrefeb, "m_CharPrefeb");
		Utility.CheckEmpty(m_Map, "m_Map");
		Utility.CheckEmpty(m_BulletPrefeb, "m_BulletPrefeb");
		Utility.CheckEmpty(m_SpawnEffectPrefeb, "m_SpawnEffectPrefeb");

		if (m_PlayerMask == 0 || m_MonsterMask == 0 || m_WallMask == 0)
			Utility.LogError("마스크 지정을 해주세요!");

		m_TimeScale = Time.timeScale;
		m_MonsterPrefebList = new List<GameObject>();
		m_BossPrefebList = new List<GameObject>();

		int max = m_CharPrefeb.Length;

		for (int i = (int)Char_Type.Player + 1; i < max; ++i)
		{
			if (i < (int)Char_Type.Boss1)
				m_MonsterPrefebList.Add(m_CharPrefeb[i]);

			else
				m_BossPrefebList.Add(m_CharPrefeb[i]);
		}
	}

	protected override void Start()
	{
		base.Start();

		int max = m_MonsterPrefebList.Count;

		for (int i = 0; i < max; ++i)
		{
			PoolManager.Create(m_MonsterPrefebList[i], "Monster");
		}

		max = m_BulletPrefeb.Length;

		for (int i = 0; i < max; ++i)
		{
			PoolManager.Create(m_BulletPrefeb[i], "Bullet");
		}

		NextStage();
	}
}
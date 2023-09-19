
using System;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : Singleton<StageManager>
{
	[ReadOnly(true)][SerializeField] private GameObject m_StagePrefeb;
	[ReadOnly(true)][SerializeField][EnumArray(typeof(Character_Type))] private GameObject[] m_CharObjPrefeb = new GameObject[(int)Character_Type.Max];
	[ReadOnly(true)][SerializeField] private LayerMask m_PlayerMask;
	[ReadOnly(true)][SerializeField] private LayerMask m_MonsterMask;
	[ReadOnly(true)][SerializeField] private LayerMask m_WallMask;
	[SerializeField] private Map m_Map;

	private Stage m_Stage;
	private int m_StageNum;

	public static Vector3 RandomSpawnPos => Inst.m_Map.RandomSpawnPos;

	public static bool IsEnemyEmpty => Inst.m_Stage.IsEnemyEmpty;
	public static bool IsPlayerDeath => Inst.m_Stage.IsPlayerDeath;
	public static bool IsStageClear => Inst.m_Stage.IsStageClear;

	public static GameObject CreatePlayerObject => Instantiate(Inst.m_CharObjPrefeb[(int)Character_Type.Player]);
	public static GameObject[] CharObjPrefeb => Inst.m_CharObjPrefeb;
	public static Player Player => Inst.m_Stage.Player;
	public static LayerMask PlayerMask => Inst.m_PlayerMask;
	public static LayerMask MonsterMask => Inst.m_MonsterMask;
	public static LayerMask WallMask => Inst.m_WallMask;
	public static Map Map => Inst.m_Map;

	public static int StageNum => Inst.m_StageNum;

	public static void Cheat(Cheat_Type type, bool isCheck)
	{
		switch (type)
		{
			case Cheat_Type.PowerUp:
			case Cheat_Type.NoHit:
				if (Player)
					Player.Cheat(type, isCheck);
				break;
			case Cheat_Type.StageClear:
				if (isCheck)
					NextStage();
				break;
			case Cheat_Type.Death:
				if (Player)
					Player.Cheat(type, isCheck);
				break;
		}
	}

	public static void NextWave()
	{
		Inst.m_Stage.NextWave();
	}

	public static void DeactiveList(Monster monster)
	{
		Inst.m_Stage.DeactiveList(monster);
	}

	public static void SetInvisibleTarget(Monster monster)
	{
		Inst.m_Stage.SetInvisibleTarget(monster);
	}

	public static void SetVisibleTarget(Monster monster)
	{
		Inst.m_Stage.SetVisibleTarget(monster);
	}

	public static void AddStageClear(Action onStageClear)
	{
		Inst.m_Stage.OnStageClear += onStageClear;
	}

	public static void NextStage()
	{
		++Inst.m_StageNum;

		if (Inst.m_Stage)
			Destroy(Inst.m_Stage.gameObject);

		Inst.m_Stage = Instantiate(Inst.m_StagePrefeb).GetComponent<Stage>();
		Inst.m_Stage.gameObject.name = "Stage " + Inst.m_StageNum;

		if (Inst.m_StageNum != 1)
			InfoManager.RefreshEnemyInfo();

		else
			InfoManager.ResetEnemyInfo();
	}

	public static void ResetStage()
	{
		Inst.m_StageNum = 0;
		InfoManager.HealFull();
		NextStage();
	}

	public static ref readonly LinkedList<Monster> GetActiveMonsters()
	{
		return ref Inst.m_Stage.GetActiveMonsters();
	}

	protected override void Awake()
	{
		base.Awake();

#if UNITY_EDITOR
		if (!m_StagePrefeb)
			Debug.LogError("if (!m_StagePrefeb)");

		Utility.CheckEmpty(m_CharObjPrefeb, "m_CharObjPrefeb");

		if (!m_Map)
			Debug.LogError("if (!m_Map)");
#endif
	}

	protected override void Start()
	{
		base.Start();

		NextStage();
	}
}
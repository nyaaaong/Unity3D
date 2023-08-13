
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
	[SerializeField] private MapGenerator m_Map;

	private Stage m_Stage;
	private int m_StageNum;

	public static bool IsEnemyEmpty { get { return Inst.m_Stage.IsEnemyEmpty; } }
	public static bool IsPlayerDeath { get { return Inst.m_Stage.IsPlayerDeath; } }
	public static bool IsStageClear { get { return Inst.m_Stage.IsStageClear; } }

	public static GameObject CreatePlayerObject { get { return Instantiate(Inst.m_CharObjPrefeb[(int)Character_Type.Player]); } }
	public static GameObject[] CharObjPrefeb { get { return Inst.m_CharObjPrefeb; } }
	public static Player Player { get { return Inst.m_Stage.Player; } }
	public static LayerMask PlayerMask { get { return Inst.m_PlayerMask; } }
	public static LayerMask MonsterMask { get { return Inst.m_MonsterMask; } }
	public static LayerMask WallMask { get { return Inst.m_WallMask; } }
	public static Vector2Int MapSize { get { return new Vector2Int(Inst.m_Map.MapSize.x, Inst.m_Map.MapSize.y); } }

	public static int StageNum { get { return Inst.m_StageNum; } }

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
					Inst.m_Stage.NextStage();
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

	public static Vector3 GetMonsterRandPos()
	{
		if (m_Quit)
			return Vector3.zero;

		if (Inst.m_Map.GetRandomOpenTile() == null)
		{
#if UNITY_EDITOR
			Debug.LogWarning("맵의 GetRandomOpenTile가 null을 반환합니다. 의도된 것입니까?");
#endif
			return Vector3.zero;
		}

		return Inst.m_Map.GetRandomOpenTile().position;
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

		if (Inst.m_StageNum == 1)
			UIManager.Score = 0;

		if (!Inst.m_Stage)
		{
			if (!Inst.m_Map.Init())
			{
				Debug.LogError("맵 초기화 실패");
				return;
			}
		}

		else
		{
			Destroy(Inst.m_Stage.gameObject);

			Inst.m_Map.CreateRandomMap();
		}

		Inst.m_Map.Generator();

		Inst.m_Stage = Instantiate(Inst.m_StagePrefeb).GetComponent<Stage>();
		Inst.m_Stage.gameObject.name = "Stage " + Inst.m_StageNum;

		UIManager.Stage = Inst.m_StageNum;

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
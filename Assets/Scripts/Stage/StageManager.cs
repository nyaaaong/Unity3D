
using System;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : Singleton<StageManager>
{
	[ReadOnly(true)][SerializeField] private Stage[] m_Stages;
	[SerializeField] private GameObject m_PlayerObjPrefeb;
	[SerializeField] private GameObject m_MeleeObjPrefeb;
	[SerializeField] private GameObject m_RangeObjPrefeb;
	[ReadOnly(true)][SerializeField] private LayerMask m_PlayerLayer;
	[ReadOnly(true)][SerializeField] private LayerMask m_MonsterLayer;
	[ReadOnly(true)][SerializeField] private LayerMask m_WallLayer;
	[SerializeField] private MapGenerator m_Map;

	private Stage m_Stage;
	private int m_StageNum = -1;
	private bool m_PowerUp = false;
	private bool m_NoHit = false;

	public static bool IsEnemyEmpty { get { return Inst.m_Stage.IsEnemyEmpty; } }
	public static bool IsPlayerDeath { get { return Inst.m_Stage.IsPlayerDeath; } }
	public static bool IsStageClear { get { return Inst.m_Stage.IsStageClear; } }

	public static GameObject CreatePlayerObject { get { return Instantiate(Inst.m_PlayerObjPrefeb); } }
	public static GameObject MeleeObjPrefeb { get { return Inst.m_MeleeObjPrefeb; } }
	public static GameObject RangeObjPrefeb { get { return Inst.m_RangeObjPrefeb; } }
	public static Player Player { get { return Inst.m_Stage.Player; } }
	public static LayerMask PlayerLayer { get { return Inst.m_PlayerLayer; } }
	public static LayerMask MonsterLayer { get { return Inst.m_MonsterLayer; } }
	public static LayerMask WallLayer { get { return Inst.m_WallLayer; } }
	public static Vector2Int MapSize { get { return new Vector2Int(Inst.m_Map.MapSize.x, Inst.m_Map.MapSize.y); } }

	public static void CheatRefresh()
	{
		Player.Cheat(Cheat_Type.PowerUp, Inst.m_PowerUp);
		Player.Cheat(Cheat_Type.NoHit, Inst.m_NoHit);
	}

	public static void PlayerCheat(Cheat_Type type, bool isChecked)
	{
		switch (type)
		{
			case Cheat_Type.PowerUp:
				if (Inst.m_PowerUp == isChecked)
					return;

				Inst.m_PowerUp = isChecked;
				break;
			case Cheat_Type.NoHit:
				if (Inst.m_NoHit == isChecked)
					return;

				Inst.m_NoHit = isChecked;
				break;
		}

		Player.Cheat(type, isChecked);
	}

	public static Vector3 GetMonsterRandPos()
	{
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
		if (UIManager.IsShowAbility)
			Debug.LogError("if (UIManager.IsShowAbility)");

		if (Inst.m_Stage)
			Destroy(Inst.m_Stage.gameObject);

		else if (!Inst.m_Map.Generator(++Inst.m_StageNum))
			Debug.Log("마지막 스테이지 도달");

		else
		{
			if (Inst.m_Stages.Length <= Inst.m_StageNum)
			{
				Debug.Log("마지막 스테이지 도달");
				return;
			}

			Inst.m_Stage = Instantiate(Inst.m_Stages[Inst.m_StageNum].gameObject).GetComponent<Stage>();
			Inst.m_Stage.gameObject.name = "Stage " + Inst.m_StageNum + 1;
		}
	}

	public static ref readonly LinkedList<Monster> GetActiveMonsters()
	{
		return ref Inst.m_Stage.GetActiveMonsters();
	}

	protected override void Awake()
	{
		base.Awake();

		if (m_Stages.Length == 0)
			Debug.LogError("if (m_Stages.Length == 0)");

		if (!m_PlayerObjPrefeb)
			Debug.LogError("if (!m_PlayerObjPrefeb)");

		if (!m_MeleeObjPrefeb)
			Debug.LogError("if (!m_MeleeObjPrefeb)");

		if (!m_RangeObjPrefeb)
			Debug.LogError("if (!m_RangeObjPrefeb)");

		if (!m_Map)
			Debug.LogError("if (!m_Map)");

		NextStage();
	}
}

using System;
using System.Collections.Generic;
using TMPro;
using UnityEditor.UIElements;
using UnityEngine;

public class StageManager : Singleton<StageManager>
{
	[SerializeField] private Stage[] m_Stages;
	[SerializeField] private GameObject m_PlayerObjPrefeb;
	[SerializeField] private GameObject m_MeleeObjPrefeb;
	[SerializeField] private GameObject m_RangeObjPrefeb;
	[SerializeField] private LayerMask m_PlayerLayer;
	[SerializeField] private LayerMask m_MonsterLayer;
	[SerializeField] private LayerMask m_WallLayer;

	private Stage m_Stage;

	public static bool IsEnemyEmpty { get { return GetInst().m_Stage.IsEnemyEmpty; } }
	public static bool IsPlayerDeath { get { return GetInst().m_Stage.IsPlayerDeath; } }
	public static bool IsStageClear { get { return GetInst().m_Stage.IsStageClear; } }

	public static GameObject CreatePlayerObject { get { return Instantiate(GetInst().m_PlayerObjPrefeb); } }
	public static GameObject MeleeObjPrefeb { get { return GetInst().m_MeleeObjPrefeb; } }
	public static GameObject RangeObjPrefeb { get { return GetInst().m_RangeObjPrefeb; } }
	public static Player Player { get { return GetInst().m_Stage.Player; } }
	public static LayerMask PlayerLayer { get { return GetInst().m_PlayerLayer; } }
	public static LayerMask MonsterLayer { get { return GetInst().m_MonsterLayer; } }
	public static LayerMask WallLayer { get { return GetInst().m_WallLayer; } }
	public static Vector2Int MapSize { get { return GetInst().m_Stage.MapSize; } }

	public static void SetVisibleTarget(Monster monster)
	{
		GetInst().m_Stage.SetVisibleTarget(monster);
	}

	public static void AddStageClear(Action onStageClear)
	{
		GetInst().m_Stage.OnStageClear += onStageClear;
	}

	public static void PlayStage(int stage)
	{
		if (GetInst().m_Stages.Length < stage || stage <= 0)
			Debug.LogError("if (m_Stages.Length < stage || stage <= 0)");

		if (GetInst().m_Stage)
			Destroy(GetInst().m_Stage.gameObject);

		GetInst().m_Stage = Instantiate(GetInst().m_Stages[stage - 1].gameObject).GetComponent<Stage>();
		GetInst().m_Stage.gameObject.name = "Stage " + stage;
	}

	public static ref readonly LinkedList<Monster> GetActiveMonsters()
	{
		return ref GetInst().m_Stage.GetActiveMonsters();
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

		PlayStage(1);
	}
}
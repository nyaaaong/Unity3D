
using System;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : BaseScript
{
	[SerializeField] private Stage[] m_Stages;
	[SerializeField] private GameObject m_PlayerObjPrefeb;
	[SerializeField] private GameObject m_MeleeObjPrefeb;
	[SerializeField] private GameObject m_RangeObjPrefeb;
	[SerializeField] private LayerMask m_PlayerLayer;
	[SerializeField] private LayerMask m_MonsterLayer;
	[SerializeField] private LayerMask m_WallLayer;

	private static StageManager m_Inst;
	private Stage m_Stage;

	public static bool IsEnemyEmpty { get { return m_Inst.m_Stage.IsEnemyEmpty; } }
	public static bool IsPlayerDeath { get { return m_Inst.m_Stage.IsPlayerDeath; } }
	public static bool IsStageClear { get { return m_Inst.m_Stage.IsStageClear; } }

	public static GameObject CreatePlayerObject { get { return Instantiate(m_Inst.m_PlayerObjPrefeb); } }
	public static GameObject MeleeObjPrefeb { get { return m_Inst.m_MeleeObjPrefeb; } }
	public static GameObject RangeObjPrefeb { get { return m_Inst.m_RangeObjPrefeb; } }
	public static Player Player { get { return m_Inst.m_Stage.Player; } }
	public static LayerMask PlayerLayer { get { return m_Inst.m_PlayerLayer; } }
	public static LayerMask MonsterLayer { get { return m_Inst.m_MonsterLayer; } }
	public static LayerMask WallLayer { get { return m_Inst.m_WallLayer; } }
	public static Vector2Int MapSize { get { return m_Inst.m_Stage.MapSize; } }

	public static void SetPlayerSpawnPoint(Transform tr)
	{
		m_Inst.m_Stage.SetPlayerSpawnPoint(tr);
	}

	public static void SetVisibleTarget(Monster monster)
	{
		m_Inst.m_Stage.SetVisibleTarget(monster);
	}

	public static void AddStageClear(Action onStageClear)
	{
		m_Inst.m_Stage.OnStageClear += onStageClear;
	}

	public static void PlayStage(int stage)
	{
		if (m_Inst.m_Stages.Length < stage || stage <= 0)
			Debug.LogError("if (m_Stages.Length < stage || stage <= 0)");

		if (m_Inst.m_Stage)
			Destroy(m_Inst.m_Stage.gameObject);

		m_Inst.m_Stage = Instantiate(m_Inst.m_Stages[stage - 1].gameObject).GetComponent<Stage>();
		m_Inst.m_Stage.gameObject.name = "Stage " + stage;
	}

	public static ref readonly LinkedList<Monster> GetActiveMonsters()
	{
		return ref m_Inst.m_Stage.GetActiveMonsters();
	}

	protected override void Awake()
	{
		base.Awake();

		m_Inst = this;

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
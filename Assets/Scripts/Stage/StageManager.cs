
using System;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : BaseScript
{
	[SerializeField] private Stage[] m_Stages;
	[SerializeField] private Player m_PlayerPrefeb;
	[SerializeField] private MeleeMonster m_MeleePrefeb;
	[SerializeField] private RangeMonster m_RangePrefeb;
	[SerializeField] private LayerMask m_PlayerLayer;
	[SerializeField] private LayerMask m_MonsterLayer;

	private static StageManager m_Inst;
	private Stage m_Stage;

	public static bool IsEnemyEmpty { get { return m_Inst.m_Stage.IsEnemyEmpty; } }

	public static Player CreatePlayer { get { return Instantiate(m_Inst.m_PlayerPrefeb); } }
	public static MeleeMonster GetMeleePrefeb { get { return m_Inst.m_MeleePrefeb; } }
	public static RangeMonster GetRangePrefeb { get { return m_Inst.m_RangePrefeb; } }
	public static Player GetPlayer { get { return m_Inst.m_Stage.GetPlayer; } }
	public static LayerMask PlayerLayer { get { return m_Inst.m_PlayerLayer; } }
	public static LayerMask MonsterLayer { get { return m_Inst.m_MonsterLayer; } }

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

		if (!m_PlayerPrefeb)
			Debug.LogError("if (!m_PlayerPrefeb)");

		if (!m_MeleePrefeb)
			Debug.LogError("if (!m_MeleePrefeb)");

		if (!m_RangePrefeb)
			Debug.LogError("if (!m_RangePrefeb)");

		PlayStage(1);
	}
}
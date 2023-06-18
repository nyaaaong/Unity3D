
using System;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : BaseScript
{
	[SerializeField] private Stage[] m_Stages;

	private static StageManager m_Inst;
	private Stage m_Stage;

	public static bool IsEnemyEmpty { get { return m_Inst.m_Stage.IsEnemyEmpty; } }

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

		PlayStage(1);
	}
}
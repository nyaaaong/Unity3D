
using System.Collections.Generic;
using UnityEngine;

public class StageManager : BaseScript
{
	[SerializeField] private Stage[] m_Stages;

	private static StageManager m_Inst;
	private Stage m_CurStage;

	public int CurEnemyCount { get { return m_CurStage.CurEnemyCount; } }

	public void PlayStage(int stage)
	{
		if (m_Stages.Length < stage || stage <= 0)
			Debug.LogError("if (m_Stages.Length < stage || stage <= 0)");

		if (m_CurStage)
			Destroy(m_CurStage.gameObject);

		m_CurStage = Instantiate(m_Stages[stage - 1].gameObject).GetComponent<Stage>();
		m_CurStage.gameObject.name = "Stage " + stage;
	}

	public ref readonly LinkedList<Monster> GetActiveMonsters()
	{
		return ref m_CurStage.GetActiveMonsters();
	}

	public static StageManager GetInst()
	{
		if (!m_Inst)
			m_Inst = new StageManager();

		return m_Inst;
	}

	protected override void Awake()
	{
		base.Awake();

		if (m_Stages.Length == 0)
			Debug.LogError("if (m_Stages.Length == 0)");

		PlayStage(1);
	}
}
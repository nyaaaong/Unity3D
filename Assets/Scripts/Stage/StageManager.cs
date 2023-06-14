
using System.Collections.Generic;

public class StageManager : BaseScript
{
	private static StageManager m_Inst;
	private Stage m_Stage;
	private Stage m_NextStage;

	public void GetActiveMonsters(List<Monster> list)
	{
		m_Stage.GetActiveMonsters(list);
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

		m_Stage = new Stage();
	}
}

using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class StaticStageInfo : BaseScript
{
	[ReadOnly(true)][SerializeField] private Text m_StageText;

	private WaitUntil m_WaitResume;
	private int m_Stage = int.MaxValue;
	private string m_StageMsg;

	private IEnumerator UpdateStage()
	{
		while (true)
		{
			yield return m_WaitResume;

			if (m_Stage != StageManager.Stage)
			{
				m_Stage = StageManager.Stage;

				m_StageText.text = string.Format(m_StageMsg, m_Stage);
			}
		}
	}

	protected override void Awake()
	{
		base.Awake();

		Utility.CheckEmpty(m_StageText, "m_StageText");

		m_StageMsg = m_StageText.text;

		m_WaitResume = new WaitUntil(() => !StageManager.IsPause);

		StartCoroutine(UpdateStage());
	}
}
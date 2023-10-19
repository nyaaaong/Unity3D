
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class StaticStageInfo : BaseScript
{
	[ReadOnly(true)][SerializeField] private Text m_StageText;
	[ReadOnly(true)][SerializeField] private Text m_CounterText;

	private WaitUntil m_WaitResume;
	private int m_Stage = int.MaxValue;
	private int m_Counter = int.MaxValue;
	private string m_StageMsg;
	private string m_CounterMsg;
	private string m_BossReadyMsg1;
	private string m_BossReadyMsg2;
	private string m_BossReadyMsg;
	private string m_BossMsg;
	private string m_BossClearMsg;
	private float m_ChangeColorTime;
	private float m_ChangeColorTimeMax = 0.4f;
	private bool m_ChangeColor;
	private Boss_State m_BossState = Boss_State.Max;
	private Boss_State m_StageBossState;

	private bool ChangeColor
	{
		get => m_ChangeColor;
		set
		{
			m_ChangeColor = value;

			if (m_ChangeColor)
				m_BossReadyMsg = m_BossReadyMsg2;

			else
				m_BossReadyMsg = m_BossReadyMsg1;

			m_CounterText.text = m_BossReadyMsg;
		}
	}

	private IEnumerator UpdateMsg()
	{
		while (true)
		{
			yield return m_WaitResume;

			if (m_Stage != StageManager.Stage)
			{
				m_Stage = StageManager.Stage;

				m_StageText.text = string.Format(m_StageMsg, m_Stage);
			}

			m_StageBossState = StageManager.BossState;

			if (m_StageBossState == Boss_State.None)
			{
				if (m_Counter != StageManager.MonsterAliveCount)
				{
					m_Counter = StageManager.MonsterAliveCount;

					m_CounterText.text = string.Format(m_CounterMsg, m_Counter);
				}
			}

			else if (m_BossState != m_StageBossState)		
			{
				m_BossState = m_StageBossState;

				if (m_BossState == Boss_State.Clear)
					m_CounterText.text = m_BossClearMsg;

				else if (m_BossState == Boss_State.NeedSpawn)
				{
					m_CounterText.text = m_BossReadyMsg;

					while (!StageManager.IsPlayerDeath)
					{
						m_BossState = StageManager.BossState;

						if (m_BossState == Boss_State.Spawn)
						{
							m_CounterText.text = m_BossMsg;
							break;
						}

						m_ChangeColorTime += Time.deltaTime;

						if (m_ChangeColorTime >= m_ChangeColorTimeMax)
						{
							m_ChangeColorTime = 0f;

							ChangeColor = !ChangeColor;
						}

						yield return null;
					}
				}
			}
		}
	}

	protected override void Awake()
	{
		base.Awake();

		Utility.CheckEmpty(m_StageText, "m_StageText");
		Utility.CheckEmpty(m_CounterText, "m_CounterText");

		m_StageMsg = m_StageText.text;
		m_CounterMsg = m_CounterText.text;
		m_BossReadyMsg1 = "<color=red>보스 출현 경고!</color>";
		m_BossReadyMsg2 = "<color=orange>보스 출현 경고!</color>";
		m_BossReadyMsg = m_BossReadyMsg1;
		m_BossMsg = "<color=red>보스 출현!</color>";
		m_BossClearMsg = "<color=cyan>보스 클리어!</color>";

		m_WaitResume = new WaitUntil(() => !StageManager.IsPause);

		m_CounterText.text = string.Format(m_CounterMsg, m_Counter);

		StartCoroutine(UpdateMsg());
	}
}
using UnityEngine;
using UnityEngine.UI;

public class StaticTextUI : BaseScript
{
	[ReadOnly(true)][SerializeField] private string m_FrontText = "";

	private bool m_Init;
	private int m_Number;
	private Text m_Text;

	private void Init()
	{
		if (!m_Init)
		{
			m_Init = true;

			m_Text = GetComponent<Text>();
		}
	}

	public int Number
	{
		get
		{
			Init();

			return m_Number;
		}

		set
		{
			Init();

			m_Number = value;

			m_Text.text = m_FrontText + m_Number.ToString();
		}
	}
}

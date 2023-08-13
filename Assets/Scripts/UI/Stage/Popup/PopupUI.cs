
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class PopupUI : BaseScript
{
	[ReadOnly(true)][SerializeField] private string m_WaveString = "Wave ";
	[ReadOnly(true)][SerializeField] private string m_StageClearString = "Stage Clear!";

	private Text m_Text;
	private Color m_WaveColor;
	private Color m_StageClearColor;
	private Color m_ResultColor;
	private string m_ResultString = "";

	public void SetAlpha(float alpha)
	{
		if (m_Text.text != m_ResultString)
			m_Text.text = m_ResultString;

		m_ResultColor.a = alpha;
		m_Text.color = m_ResultColor;
	}

	public void SetPopup(Popup_Type type, int waveNum = -1)
	{
		switch (type)
		{
			case Popup_Type.Wave:
				m_ResultString = m_WaveString + waveNum;
				m_ResultColor = m_WaveColor;
				break;
			case Popup_Type.StageClear:
				m_ResultString = m_StageClearString;
				m_ResultColor = m_StageClearColor;
				break;
		}
	}

	public void SetColor(Popup_Type type, Color color)
	{
		Color newColor = new Color(color.r, color.g, color.b, 0f);

		switch (type)
		{
			case Popup_Type.Wave:
				m_WaveColor = newColor;
				break;
			case Popup_Type.StageClear:
				m_StageClearColor = newColor;
				break;
		}
	}

	protected override void Awake()
	{
		base.Awake();

		m_Text = GetComponent<Text>();

#if UNITY_EDITOR
		if (m_Text == null)
			Debug.LogError("if (m_Text == null)");
#endif
	}
}
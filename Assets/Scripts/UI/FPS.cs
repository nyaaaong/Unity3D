using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class FPS : MonoBehaviour
{
	private Text m_Text;
	private string m_str;
	private float m_Time;
	private int m_FPS = 60;
	private int m_FPSMax = 60;

	private void OnGUI()
	{
		if (m_Time == 0)
			m_FPS = 60;

		else
			m_FPS = Mathf.RoundToInt(1.0f / m_Time);

		if (m_FPS > m_FPSMax)
			m_FPS = m_FPSMax;

		m_Text.text = string.Format(m_str, m_FPS);
	}

	private void Update()
	{
		m_Time = (Time.unscaledDeltaTime - m_Time) * 0.1f;
	}

	private void Awake()
	{
		m_Text = GetComponent<Text>();
		m_str = m_Text.text;

		if (Application.targetFrameRate < 60)
			Application.targetFrameRate = 60;

		m_FPSMax = Application.targetFrameRate;
	}
}

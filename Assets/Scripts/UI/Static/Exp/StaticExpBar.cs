
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class StaticExpBar : BaseScript
{
	private Image m_Bar;
	private StaticExpText m_Text;
	private float m_StartBar;
	private float m_EndBar;
	private float m_Time;
	private float m_LerpTime = 0.3f;
	private float m_ColorTime;
	private float m_ColorLerpTime = 2f;
	private float m_ColorChangeCurTime;
	private float m_ColorChangeMaxTime = 0.2f;
	private Color m_OriginalColor;
	private Color m_StartColor;
	private Color m_EndColor;
	private Color m_SecondColor = Color.yellow;
	private Coroutine m_Coroutine;
	private bool m_IsStopped;
	private bool m_IsChangeColor;

	public bool IsStopped => m_IsStopped;

	public void ChangeColor()
	{
		m_ColorChangeCurTime = 0f;
		m_StartColor = m_Bar.color;
		m_IsChangeColor = !m_IsChangeColor;
		m_EndColor = m_IsChangeColor ? m_SecondColor : m_OriginalColor;
	}

	public void SetText(StaticExpText text)
	{
		m_Text = text;
	}

	public void SetExp(float exp)
	{
		if (m_EndBar != exp)
		{
			if (m_Coroutine != null)
				StopCoroutine(m_Coroutine);

			if (m_EndBar > exp)
				m_Bar.fillAmount = 0f;

			m_StartBar = m_Bar.fillAmount;
			m_EndBar = exp;

			m_Coroutine = StartCoroutine(UpdateBar());
		}
	}

	private IEnumerator UpdateBar()
	{
		bool loop = true;

		m_IsStopped = false;
		m_Time = 0f;

		while (loop)
		{
			m_Time += Time.deltaTime;

			if (m_Time >= m_LerpTime)
			{
				m_Time = m_LerpTime;
				loop = false;
			}

			m_Bar.fillAmount = Mathf.Lerp(m_StartBar, m_EndBar, m_Time / m_LerpTime);
			m_Text.SetExp(Mathf.RoundToInt(m_Bar.fillAmount * 100f));

			yield return null;
		}

		if (m_Bar.fillAmount == 1f)
			StartCoroutine(UpdateColor());

		else
			m_IsStopped = true;
	}

	// 100%가 되면 일정시간 동안 바의 색깔이 변한다
	private IEnumerator UpdateColor()
	{
		bool loop = true;

		m_ColorTime = 0f;
		m_ColorChangeCurTime = 0f;

		while (loop)
		{
			m_ColorTime += Time.deltaTime;
			m_ColorChangeCurTime += Time.deltaTime;

			if (m_ColorTime >= m_ColorLerpTime)
				loop = false;

			else if (m_ColorChangeCurTime >= m_ColorChangeMaxTime)
				ChangeColor();

			m_Bar.color = Color.Lerp(m_StartColor, m_EndColor, m_ColorChangeCurTime / m_ColorChangeMaxTime);

			yield return null;
		}

		m_Bar.color = m_OriginalColor;
		m_IsStopped = true;
	}

	protected override void Awake()
	{
		base.Awake();

		m_Bar = GetComponent<Image>();
		m_OriginalColor = m_Bar.color;
		m_StartColor = m_OriginalColor;

		ChangeColor();
	}
}
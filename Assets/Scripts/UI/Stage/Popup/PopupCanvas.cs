using System.Collections;
using UnityEngine;

public class PopupCanvas : BaseScript
{
	private enum Fade_Type
	{
		Out,
		Between,
		In
	}

	[ReadOnly(true)][SerializeField] private PopupUI m_PopupUI;
	[ReadOnly(true)][SerializeField] private float m_FadeTime;
	[ReadOnly(true)][SerializeField] private float m_FadeBetweenTime;
	[ReadOnly(true)][SerializeField] private Color m_WaveColor;
	[ReadOnly(true)][SerializeField] private Color m_StageClearColor;

	private float m_Alpha;
	private float m_Time;
	private float m_Inter;
	private bool m_FadeProc;
	private StageUI m_StageUI;
	private Popup_Type m_PopupType;

	public void ShowPopup(Popup_Type type, int waveNum = -1)
	{
		if (m_FadeProc)
		{
			m_FadeProc = false;

			StopAllCoroutines();
		}

		m_PopupType = type;

		switch (type)
		{
			case Popup_Type.Wave:
				if (waveNum == -1)
				{
					Debug.LogError("if (waveNum == -1)");
					return;
				}
				m_PopupUI.SetPopup(Popup_Type.Wave, waveNum);
				break;
			case Popup_Type.StageClear:
				m_PopupUI.SetPopup(Popup_Type.StageClear);
				break;
		}

		m_StageUI.IsShowWave = true;
		StartCoroutine(Fade(Fade_Type.Out));
	}

	private IEnumerator Fade(Fade_Type type)
	{
		m_FadeProc = true;

		m_Time = 0f;

		if (type != Fade_Type.Between)
		{
			while (m_Time < m_FadeTime)
			{
				switch (type)
				{
					case Fade_Type.Out:
						Fade(0f, 1f);
						break;
					case Fade_Type.In:
						Fade(1f, 0f);
						break;
				}

				yield return null;
			}
		}

		else
		{
			while (m_Time < m_FadeBetweenTime)
			{
				m_Time += m_deltaTime;

				yield return null;
			}
		}

		switch (type)
		{
			case Fade_Type.Out:
				StartCoroutine(Fade(Fade_Type.Between));
				break;
			case Fade_Type.Between:
				StartCoroutine(Fade(Fade_Type.In));
				break;
			case Fade_Type.In:
				m_FadeProc = false;
				switch (m_PopupType)
				{
					case Popup_Type.Wave:
						m_StageUI.NextWave();
						break;
					case Popup_Type.StageClear:
						m_StageUI.ShowAbility();
						break;
				}
				m_StageUI.IsShowWave = false;
				break;
		}
	}

	private void Fade(float start, float end)
	{
		m_Time += m_deltaTime;

		m_Inter = m_Time > 0f ? m_Time / m_FadeTime : 0f;

		m_Alpha = Mathf.Lerp(start, end, m_Inter);
		m_PopupUI.SetAlpha(m_Alpha);
	}

	protected override void Awake()
	{
		base.Awake();

#if UNITY_EDITOR
		if (m_PopupUI == null)
			Debug.LogError("if (m_PopupUI == null)");

		if (m_FadeTime == 0f)
			Debug.LogError("if (m_FadeTime == 0f)");
#endif

		m_StageUI = transform.parent.GetComponent<StageUI>();

#if UNITY_EDITOR
		if (m_StageUI == null)
			Debug.LogError("if (m_StageUI == null)");
#endif

		m_PopupUI.SetColor(Popup_Type.Wave, m_WaveColor);
		m_PopupUI.SetColor(Popup_Type.StageClear, m_StageClearColor);
	}
}

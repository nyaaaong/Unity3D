
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Fadeable : BaseScript
{
	[ReadOnly(true)][SerializeField] private float m_FadeTimeMax;

	private Graphic[] m_Component;
	private Color[] m_Color;
	private float[] m_AlphaMax;
	private float m_FadeTime;
	private int m_Count;
	private float m_LerpTime;
	private Fadeable m_NextFadeable;

	public Fadeable NextFadeable { set => m_NextFadeable = value; }

	public event Action CallBack;

	protected override void Awake()
	{
		base.Awake();

		m_Component = GetComponentsInChildren<Graphic>();

		Utility.CheckEmpty(m_Component, "m_Component");

		m_Count = m_Component.Length;
		m_Color = new Color[m_Count];
		m_AlphaMax = new float[m_Count];

		for (int i = 0; i < m_Count; ++i)
		{
			m_Color[i] = m_Component[i].color;
			m_AlphaMax[i] = m_Color[i].a;

			m_Color[i].a = 0f;
			m_Component[i].color = m_Color[i];
		}
	}

	public void StartFade()
	{
		StartCoroutine(Fade());
	}

	private IEnumerator Fade()
	{
		m_FadeTime = 0f;

		while (m_FadeTime < m_FadeTimeMax)
		{
			m_FadeTime += Time.deltaTime;
			m_LerpTime = m_FadeTime / m_FadeTimeMax;

			for (int i = 0; i < m_Count; ++i)
			{
				m_Color[i].a = Mathf.Lerp(0f, m_AlphaMax[i], m_LerpTime);
				m_Component[i].color = m_Color[i];
			}

			yield return null;
		}

		for (int i = 0; i < m_Count; ++i)
		{
			m_Color[i].a = m_AlphaMax[i];
			m_Component[i].color = m_Color[i];
		}

		if (m_NextFadeable != null)
			m_NextFadeable.StartFade();

		if (CallBack != null)
			CallBack();
	}
}
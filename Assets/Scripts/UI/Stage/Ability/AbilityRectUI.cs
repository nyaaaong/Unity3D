﻿using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class AbilityRectUI : BaseScript
{
	private AbilityRectCanvas m_Canvas;
	private AbilityUI m_AbilityUI;
	private RectTransform m_AbilityUIRT;
	private Button m_Button;
	private AudioSource m_Audio;
	private WaitForSeconds m_Wait;
	private float m_AnimTime;
	private float m_AccTime;
	private float m_Time;
	private float m_AnimTimeHalf;
	private Vector2 m_AnimOrigSize;
	private Vector2 m_AnimMinSize;

	public AbilityUI AbilityUI
	{
		set
		{
			m_AbilityUI = value;

			m_AbilityUIRT = m_AbilityUI.GetComponent<RectTransform>();

#if UNITY_EDITOR
			if (m_AbilityUIRT == null)
				Debug.LogError("if (m_AbilityUIRT == null)");
#endif

			m_AnimOrigSize = m_AbilityUIRT.rect.size;
		}
	}

	private IEnumerator ClickAnimation()
	{
		m_AccTime = 0f;

		while (m_AccTime < m_AnimTime)
		{
			m_AccTime += m_deltaTime;

			m_Time = m_AccTime > 0f ? m_AccTime / m_AnimTimeHalf : 0f;

			if (m_AccTime < m_AnimTimeHalf)
				AnimChange();

			else
				AnimRevert();

			yield return null;
		}

		m_AbilityUIRT.sizeDelta = m_AnimOrigSize;
	}

	private void AnimChange()
	{
		m_AbilityUIRT.sizeDelta = Vector2.Lerp(m_AnimOrigSize, m_AnimMinSize, m_Time);
	}

	private void AnimRevert()
	{
		m_AbilityUIRT.sizeDelta = Vector2.Lerp(m_AnimMinSize, m_AnimOrigSize, m_Time);
	}

	private IEnumerator NextStageTimer()
	{
		yield return m_Wait;

		UIManager.HideAbility();
		StageManager.NextStage();

	}

	protected override void Awake()
	{
		base.Awake();

		m_Button = GetComponent<Button>();
		m_Button.onClick.AddListener(OnClickEvent);

		m_Audio = GetComponentInParent<AudioSource>();

#if UNITY_EDITOR
		if (m_Audio == null)
			Debug.LogError("if (m_Audio  == null)");
#endif

		m_Audio.volume = AudioManager.VolumeEffect;
		m_Audio.clip = AudioManager.EffectClip[(int)Effect_Audio.AbilityUI];

		m_Canvas = transform.parent.GetComponent<AbilityRectCanvas>();

#if UNITY_EDITOR
		if (m_Canvas == null)
			Debug.LogError("if (m_Canvas == null)");
#endif

		m_AnimTime = m_Canvas.AnimTime;
		m_AnimMinSize = m_Canvas.AnimMinSize;
		m_AnimTimeHalf = m_AnimTime * 0.5f;

		m_Wait = new WaitForSeconds(Mathf.Max(m_Audio.clip.length, m_AnimTime));
	}

	private void OnClickEvent()
	{
		if (!m_Canvas.Selected)
		{
			m_Canvas.Selected = true;

			m_AbilityUI.OnClickEvent();

			m_Audio.Play();
			StartCoroutine(ClickAnimation());
			StartCoroutine(NextStageTimer());
		}
	}

	protected override void OnEnable()
	{
		base.OnEnable();

		AudioManager.AddEffectAudio(m_Audio);
	}

	protected override void OnDisable()
	{
		base.OnDisable();

		if (m_AbilityUI != null)
		{
			Destroy(m_AbilityUI.gameObject);

			m_AbilityUI = null;
			m_AbilityUIRT = null;
		}

		m_Canvas.Selected = false;
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();

		if (!m_Quit)
			AudioManager.RemoveEffectAudio(m_Audio);
	}
}
using System.Collections;
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
	private WaitUntil m_WaitTime;
	private float m_Time;
	private float m_LerpTime; // 버튼 애니메이션이 동작될 총 시간이며 효과음의 길이만큼 지정된다.
	private float m_HalfTime; // Lerp Time의 절반
	private bool m_PlayAnim; // 위에서 시간이 다다르면 false로 변할것이다.
	private Vector2 m_OriginalSize;
	private Vector2 m_ClickedSize;

	public AbilityUI AbilityUI
	{
		set
		{
			m_AbilityUI = value;
			m_AbilityUIRT = m_AbilityUI.GetComponent<RectTransform>();

			Utility.CheckEmpty(m_AbilityUIRT, "m_AbilityUIRT");

			m_OriginalSize = m_AbilityUIRT.sizeDelta;
			m_ClickedSize = m_OriginalSize * m_Canvas.ClickedSize;
		}
	}

	private IEnumerator ClickAnimation()
	{
		m_PlayAnim = true;
		m_Time = 0f;

		while (m_Time < m_LerpTime)
		{
			m_Time += Time.unscaledDeltaTime;

			if (m_Time < m_HalfTime)
				m_AbilityUIRT.sizeDelta = Vector2.Lerp(m_OriginalSize, m_ClickedSize, m_Time / m_HalfTime);

			else
				m_AbilityUIRT.sizeDelta = Vector2.Lerp(m_ClickedSize, m_OriginalSize, m_Time / m_LerpTime);

			yield return null;
		}

		m_AbilityUIRT.sizeDelta = m_OriginalSize;
		m_PlayAnim = false;
	}

	private IEnumerator ResumeGame()
	{
		yield return m_WaitTime;

		StageManager.Resume();

		UIManager.HideAbility();
		AudioManager.PlayStageBGM(true);
	}

	protected override void Awake()
	{
		base.Awake();

		m_Button = GetComponent<Button>();
		m_Button.onClick.AddListener(OnClickEvent);

		m_Audio = GetComponentInParent<AudioSource>();
		m_Audio.volume = AudioManager.VolumeEffect;
		m_Audio.clip = AudioManager.EffectClip.AbilitySelect;

		m_Canvas = transform.parent.GetComponent<AbilityRectCanvas>();

		Utility.CheckEmpty(m_Audio, "m_Audio");
		Utility.CheckEmpty(m_Canvas, "m_Canvas");

		m_LerpTime = m_Audio.clip.length;
		m_HalfTime = m_LerpTime * 0.5f;

		m_WaitTime = new WaitUntil(() => !m_PlayAnim);
	}

	private void OnClickEvent()
	{
		if (!m_PlayAnim)
		{
			m_AbilityUI.OnClickEvent();

			m_Audio.Play();
			StartCoroutine(ClickAnimation());
			StartCoroutine(ResumeGame());
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
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();

		AudioManager.RemoveEffectAudio(m_Audio);
	}
}
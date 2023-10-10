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
	private Coroutine m_ClickAnim;
	private float m_Time;
	private float m_LerpTime; // 버튼 애니메이션이 동작될 총 시간이며 효과음의 길이만큼 지정된다.
	private float m_AnimTime;
	private float m_AnimLerpTime; // 버튼 애니메이션의 작아질때와 커질때의 시간이며 m_LerpTime의 절반이 지정된다.
	private bool m_Loop; // 코루틴이 계속 돌 것인지
	private bool m_IsPlayAnim; // 위에서 시간이 다다르면 false로 변할것이다.
	private bool m_ChangeAnim; // true false에 따라 애니메이션이 작아질지 커질지 판단한다.
	private Vector2 m_OriginalSize;
	private Vector2 m_ClickedSize;
	private Vector2 m_StartSize;
	private Vector2 m_EndSize;

	private void StopClickAnim()
	{
		if (m_IsPlayAnim)
			StopCoroutine(m_ClickAnim);

		m_AbilityUIRT.sizeDelta = m_OriginalSize;
		m_IsPlayAnim = false;
	}

	private void SetAnimSize(bool reverse)
	{
		if (!reverse)
		{
			m_StartSize = m_OriginalSize;
			m_EndSize = m_ClickedSize;
		}

		else
		{
			m_StartSize = m_ClickedSize;
			m_EndSize = m_OriginalSize;
		}
	}

	private void ChangeAnim()
	{
		m_AnimTime = 0f;
		m_ChangeAnim = !m_ChangeAnim;

		SetAnimSize(m_ChangeAnim);
	}

	public AbilityUI AbilityUI
	{
		set
		{
			m_AbilityUI = value;
			m_AbilityUIRT = m_AbilityUI.GetComponent<RectTransform>();

			Utility.CheckEmpty(m_AbilityUIRT, "m_AbilityUIRT");

			m_OriginalSize = m_AbilityUIRT.sizeDelta;
			m_ClickedSize = m_OriginalSize * m_Canvas.ClickedSize;

			SetAnimSize(false);
		}
	}

	private IEnumerator ClickAnimation()
	{
		m_Loop = true;
		m_IsPlayAnim = true;
		m_Time = 0f;
		m_AnimTime = 0f;

		while (m_Loop)
		{
			m_Time += Time.unscaledDeltaTime;
			m_AnimTime += Time.unscaledDeltaTime;

			if (m_Time >= m_LerpTime)
				m_Loop = false;

			if (m_AnimTime >= m_AnimLerpTime)
				ChangeAnim();

			m_AbilityUIRT.sizeDelta = Vector2.Lerp(m_StartSize, m_EndSize, m_AnimTime / m_AnimLerpTime);

			yield return null;
		}

		m_AbilityUIRT.sizeDelta = m_OriginalSize;
		m_IsPlayAnim = false;
	}

	private IEnumerator ResumeGame()
	{
		yield return m_WaitTime;

		StageManager.Resume();

		UIManager.HideAbility();
	}

	protected override void Awake()
	{
		base.Awake();

		m_Button = GetComponent<Button>();
		m_Button.onClick.AddListener(OnClickEvent);

		m_Audio = GetComponentInParent<AudioSource>();
		m_Audio.volume = AudioManager.VolumeEffect;
		m_Audio.clip = AudioManager.EffectClip[(int)Audio_Effect.AbilityUI];

		m_Canvas = transform.parent.GetComponent<AbilityRectCanvas>();

		Utility.CheckEmpty(m_Audio, "m_Audio");
		Utility.CheckEmpty(m_Canvas, "m_Canvas");

		m_LerpTime = m_Audio.clip.length;
		m_AnimLerpTime = m_LerpTime * 0.5f;

		m_WaitTime = new WaitUntil(() => !m_IsPlayAnim);
	}

	private void OnClickEvent()
	{
		if (!m_Canvas.Selected)
		{
			StopClickAnim();

			m_Canvas.Selected = true;

			m_AbilityUI.OnClickEvent();

			m_Audio.Play();
			m_ClickAnim = StartCoroutine(ClickAnimation());
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

		m_Canvas.Selected = false;
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();

		if (!m_Quit)
			AudioManager.RemoveEffectAudio(m_Audio);
	}
}
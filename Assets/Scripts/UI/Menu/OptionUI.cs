
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class OptionUI : BaseScript
{
	[ReadOnly(true)][SerializeField] private Button m_OKButton;
	[ReadOnly(true)][SerializeField] private Button m_QuitButton;
	[ReadOnly(true)][SerializeField] private Slider m_BGMSlider;
	[ReadOnly(true)][SerializeField] private Slider m_EffectSlider;

	private void OnOKClickEvent()
	{
		AudioManager.VolumeBGM = m_BGMSlider.value;
		AudioManager.VolumeEffect = m_EffectSlider.value;

		UIManager.HideMenu(Menu_Type.Option);

		AudioManager.RefreshVolume();
		AudioManager.SaveAudioData();
	}

	private void OnQuitClickEvent()
	{
#if UNITY_EDITOR
		EditorApplication.isPlaying = false;
#else
		Application.Quit();
#endif
	}

	protected override void Awake()
	{
		base.Awake();

#if UNITY_EDITOR
		if (!m_OKButton || !m_QuitButton || !m_BGMSlider || !m_EffectSlider)
			Debug.LogError("if (!m_OKButton || !m_QuitButton || !m_BGMSlider || !m_EffectSlider)");
#endif

		m_BGMSlider.value = AudioManager.VolumeBGM;
		m_EffectSlider.value = AudioManager.VolumeEffect;

		m_OKButton.onClick.AddListener(OnOKClickEvent);
		m_QuitButton.onClick.AddListener(OnQuitClickEvent);
	}
}
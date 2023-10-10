
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

		Utility.CheckEmpty(m_OKButton, "m_OKButton");
		Utility.CheckEmpty(m_QuitButton, "m_QuitButton");
		Utility.CheckEmpty(m_BGMSlider, "m_BGMSlider");
		Utility.CheckEmpty(m_EffectSlider, "m_EffectSlider");

		m_BGMSlider.value = AudioManager.VolumeBGM;
		m_EffectSlider.value = AudioManager.VolumeEffect;

		m_OKButton.onClick.AddListener(OnOKClickEvent);
		m_QuitButton.onClick.AddListener(OnQuitClickEvent);
	}
}
using UnityEngine;
using UnityEngine.UI;

public class MenuUI : BaseScript
{
	[ReadOnly(true)][SerializeField] private Button m_CloseButton;
	[ReadOnly(true)][SerializeField] private Slider m_BGMSlider;
	[ReadOnly(true)][SerializeField] private Slider m_EffectSlider;
	[ReadOnly(true)][SerializeField][EnumArray(typeof(Ability_Type))] private AbilityInfoUI[] m_AbilityInfoUI = new AbilityInfoUI[(int)Ability_Type.Max];

	private int m_AbilityCount;
	private int m_HealAbilityIndex;
	private float m_BGMValue = float.MinValue;
	private float m_EffectValue = float.MinValue;

	// 슬라이더의 핸들을 눌렀다 떼었을 때 진입
	public void OnPointerUp()
	{
		if (m_BGMValue != m_BGMSlider.value ||
			m_EffectValue != m_EffectSlider.value)
		{
			m_BGMValue = m_BGMSlider.value;
			m_EffectValue = m_EffectSlider.value;

			AudioManager.VolumeBGM = m_BGMSlider.value;
			AudioManager.VolumeEffect = m_EffectSlider.value;

			AudioManager.RefreshVolume();
			AudioManager.SaveAudioData();
		}
	}

	protected override void OnEnable()
	{
		base.OnEnable();

		for (int i = 0; i < m_AbilityCount; ++i)
		{
			if (i == m_HealAbilityIndex)
				continue;

			m_AbilityInfoUI[i].SetActive(DataManager.GetBuffStack(i) > 0);
		}
	}

	private void OnCloseClickEvent()
	{
		UIManager.HideMenu(Menu_Type.Menu);
		AudioManager.ResumeNeedBossSpawnAudio();
	}

	protected override void Awake()
	{
		base.Awake();

		Utility.CheckEmpty(m_CloseButton, "m_CloseButton");
		Utility.CheckEmpty(m_BGMSlider, "m_BGMSlider");
		Utility.CheckEmpty(m_EffectSlider, "m_EffectSlider");

		m_BGMSlider.value = AudioManager.VolumeBGM;
		m_EffectSlider.value = AudioManager.VolumeEffect;

		m_CloseButton.onClick.AddListener(OnCloseClickEvent);

		m_AbilityCount = (int)Ability_Type.Max;
		m_HealAbilityIndex = (int)Ability_Type.Heal;
	}
}
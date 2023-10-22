using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

namespace DevionGames.UIWidgets
{
	[RequireComponent(typeof(Slider))]
	public class AudioVolume : MonoBehaviour
	{
		[SerializeField]
		private AudioMixer m_MixerGroup = null;
		[SerializeField]
		private string m_ExposedParameter = "MusicVolume";

		private Slider m_Slider;

		private void Start()
		{
			m_Slider = GetComponent<Slider>();
			m_Slider.minValue = 0.0001f;
			m_Slider.maxValue = 1.0f;

			m_MixerGroup.GetFloat(m_ExposedParameter, out float defaultValue);

			float volume = PlayerPrefs.GetFloat(m_ExposedParameter, Mathf.Pow(10, defaultValue / 20));
			m_Slider.value = volume;
			SetVolume(volume);
			m_Slider.onValueChanged.AddListener(SetVolume);
		}

		public void SetVolume(float volume)
		{
			m_MixerGroup.SetFloat(m_ExposedParameter, Mathf.Log10(volume) * 20);
			PlayerPrefs.SetFloat(m_ExposedParameter, volume);
		}
	}
}
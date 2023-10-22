using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

namespace DevionGames
{
	public class PlayAudioClip : MonoBehaviour
	{
		[SerializeField]
		private AudioClip m_AudioClip = null;
		[SerializeField]
		private AudioMixerGroup m_AudioMixerGroup = null;
		[SerializeField]
		private float m_Volume = 1f;
		[SerializeField]
		private float m_Delay = 0f;

		private IEnumerator Start()
		{
			yield return new WaitForSeconds(m_Delay);
			AudioSource audioSource = GetComponent<AudioSource>();
			if (audioSource == null)
			{
				audioSource = gameObject.AddComponent<AudioSource>();
			}

			audioSource.outputAudioMixerGroup = m_AudioMixerGroup;
			audioSource.volume = m_Volume;
			audioSource.PlayOneShot(m_AudioClip);
		}
	}
}
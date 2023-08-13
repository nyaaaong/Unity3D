using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class AudioVolume
{
	[ReadOnly(true)][SerializeField][Range(0f, 1f)] private float m_VolumeBGM = 0.3f;
	[ReadOnly(true)][SerializeField][Range(0f, 1f)] private float m_VolumeEffect = 1f;

	public float VolumeBGM { get { return m_VolumeBGM; } set { m_VolumeBGM = value; } }
	public float VolumeEffect { get { return m_VolumeEffect; } set { m_VolumeEffect = value; } }
}

[RequireComponent(typeof(AudioSource))]
public class AudioManager : Singleton<AudioManager>
{
	[SerializeField] private AudioVolume m_Volume;
	[ReadOnly(true)][SerializeField][EnumArray(typeof(Character_Audio))] private AudioClip[] m_PlayerClip = new AudioClip[(int)Character_Audio.Max];
	[ReadOnly(true)][SerializeField][EnumArray(typeof(Character_Audio))] private AudioClip[] m_MonsterClip = new AudioClip[(int)Character_Audio.Max];
	[ReadOnly(true)][SerializeField][EnumArray(typeof(Effect_Audio))] private AudioClip[] m_EffectClip = new AudioClip[(int)Effect_Audio.Max];
	[ReadOnly(true)][SerializeField] private AudioClip[] m_StageClip;
	[ReadOnly(true)][SerializeField] private AudioClip m_AbilityClip;

	private AudioSource m_BGMAudio;
	private LinkedList<AudioSource> m_EffectAudioLinkedList;
	private int m_StageClipLength;

	public static float VolumeBGM { get { return Inst.m_Volume.VolumeBGM; } set { Inst.m_Volume.VolumeBGM = value; } }
	public static float VolumeEffect { get { return Inst.m_Volume.VolumeEffect; } set { Inst.m_Volume.VolumeEffect = value; } }
	public static AudioClip[] PlayerClip { get { return Inst.m_PlayerClip; } }
	public static AudioClip[] MonsterClip { get { return Inst.m_MonsterClip; } }
	public static AudioClip[] EffectClip { get { return Inst.m_EffectClip; } }

	public static void SaveAudioData()
	{
		DataManager.SaveData(Inst.m_Volume, Data_Type.Audio);
	}

	public static void RefreshVolume()
	{
		Inst.m_BGMAudio.volume = VolumeBGM;

		foreach (AudioSource audio in Inst.m_EffectAudioLinkedList)
		{
			if (audio == null)
				Inst.m_EffectAudioLinkedList.Remove(audio);

			audio.volume = VolumeEffect;
		}
	}

	public static void AddEffectAudio(AudioSource audioSource)
	{
		if (Inst == null)
			return;

		if (Inst.m_EffectAudioLinkedList.Find(audioSource) != null)
			return;

		Inst.m_EffectAudioLinkedList.AddLast(audioSource);
	}

	public static void RemoveEffectAudio(AudioSource audioSource)
	{
		if (Inst == null)
			return;

		LinkedListNode<AudioSource> find = Inst.m_EffectAudioLinkedList.Find(audioSource);

		if (find == null)
			return;

		Inst.m_EffectAudioLinkedList.Remove(find);
	}

	public static void PlayStageBGM()
	{
		if (Inst.m_BGMAudio.isPlaying)
			Inst.m_BGMAudio.Stop();

		Inst.m_BGMAudio.clip = Inst.m_StageClip[UnityEngine.Random.Range(0, Inst.m_StageClipLength)];
		Inst.m_BGMAudio.Play();
	}

	public static void PlayAbilityBGM()
	{
		if (Inst.m_BGMAudio.isPlaying)
			Inst.m_BGMAudio.Stop();

		Inst.m_BGMAudio.clip = Inst.m_AbilityClip;
		Inst.m_BGMAudio.Play();
	}

	protected override void Awake()
	{
		base.Awake();

		m_Volume = DataManager.LoadData<AudioVolume>(Data_Type.Audio);

		if (m_Volume == null)
			m_Volume = new AudioVolume();

		Inst.m_BGMAudio = GetComponent<AudioSource>();
		Inst.m_BGMAudio.volume = VolumeBGM;

		m_EffectAudioLinkedList = new LinkedList<AudioSource>();

		m_StageClipLength = m_StageClip.Length;

#if UNITY_EDITOR
		Utility.CheckEmpty(m_StageClip, "m_StageClip");

		if (!m_AbilityClip)
			Debug.LogError("if (!m_AbilityClip)");
#endif
	}
}

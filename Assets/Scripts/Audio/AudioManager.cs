using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class AudioVolume
{
	[HideInInspector][SerializeField] private float m_VolumeBGM = 0.3f;
	[HideInInspector][SerializeField] private float m_VolumeEffect = 1f;

	public float VolumeBGM { get => m_VolumeBGM; set => m_VolumeBGM = value; }
	public float VolumeEffect { get => m_VolumeEffect; set => m_VolumeEffect = value; }
}

[Serializable]
public class CharClip
{
	public AudioClip[] AttackClip;
	public AudioClip DeathClip;
}

[Serializable]
public class EffectClip
{
	public AudioClip[] PlayerHit;
	public AudioClip[] MonsterHit;
	public AudioClip[] MeleeHit;
	public AudioClip AbilitySelect;
}

[RequireComponent(typeof(AudioSource))]
public class AudioManager : Singleton<AudioManager>
{
	[Serializable]
	public class Audio
	{
		[SerializeField] private AudioSource m_Audio;
		private bool m_Pause;

		public bool IsPause => m_Pause;
		public bool IsPlaying => m_Audio.isPlaying;

		public float Length()
		{
			if (m_Audio.clip == null)
				Utility.LogError("클립이 업습니다!");

			return m_Audio.clip.length;
		}

		public void Pause()
		{
			m_Pause = true;
			m_Audio.Pause();
		}

		public void Resume()
		{
			if (m_Pause)
				m_Audio.UnPause();
		}

		public void Play()
		{
			m_Audio.Play();
		}

		public void Stop()
		{
			if (IsPlaying)
				m_Audio.Stop();
		}

		public void ChangeBGM(AudioClip clip, bool isLoop = true)
		{
			Stop();
			m_Audio.clip = clip;
			m_Audio.loop = isLoop;
		}

		public void ChangeVolume(float volume)
		{
			m_Audio.volume = volume;
		}
	}

	[SerializeField] private AudioVolume m_Volume;
	[ReadOnly(true)][SerializeField][EnumArray(typeof(CharClip_Type))] private CharClip[] m_CharClip = new CharClip[(int)CharClip_Type.Max];
	[ReadOnly(true)][SerializeField] private EffectClip m_EffectClip = new EffectClip();
	[ReadOnly(true)][SerializeField][EnumArray(typeof(Audio_Type))] private Audio[] m_Audio = new Audio[(int)Audio_Type.Max];
	[ReadOnly(true)][SerializeField] private AudioClip[] m_StageClip;
	[ReadOnly(true)][SerializeField] private AudioClip[] m_BossClip;
	[ReadOnly(true)][SerializeField] private AudioClip[] m_BossClearClip;

	private LinkedList<AudioSource> m_EffectAudioLinkedList;
	private WaitForSeconds m_WaitBossSpawn;

	public static float VolumeBGM { get => Inst.m_Volume.VolumeBGM; set => Inst.m_Volume.VolumeBGM = value; }
	public static float VolumeEffect { get => Inst.m_Volume.VolumeEffect; set => Inst.m_Volume.VolumeEffect = value; }
	public static ref readonly CharClip PlayerClip => ref Inst.m_CharClip[(int)CharClip_Type.Player];
	public static ref readonly CharClip MonsterClip => ref Inst.m_CharClip[(int)CharClip_Type.Monster];
	public static ref readonly EffectClip EffectClip => ref Inst.m_EffectClip;

	private static Audio MusicAudio => Inst.m_Audio[(int)Audio_Type.Music];
	public static Audio NeedBossSpawnAudio => Inst.m_Audio[(int)Audio_Type.NeedBossSpawn];

	public static bool IsPlayingMusic => Inst.m_Audio[(int)Audio_Type.Music].IsPlaying;

	public delegate void AfterEvent();

	public static void StopAllAudio()
	{
		Inst.StopAllCoroutines();

		int count = Inst.m_Audio.Length;

		for (int i = 0; i < count; ++i)
		{
			Inst.m_Audio[i].Stop();
		}
	}

	public static void SaveAudioData()
	{
		FileManager.SaveData(Inst.m_Volume, Data_Type.Audio);
	}

	public static void RefreshVolume()
	{
		int count = Inst.m_Audio.Length;

		for (int i = 0; i < count; ++i)
		{
			Inst.m_Audio[i].ChangeVolume(VolumeBGM);
		}

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

		else if (Inst.m_EffectAudioLinkedList == null)
			return;

		else if (Inst.m_EffectAudioLinkedList.Find(audioSource) != null)
			return;

		Inst.m_EffectAudioLinkedList.AddLast(audioSource);
	}

	public static void RemoveEffectAudio(AudioSource audioSource)
	{
		if (Inst == null)
			return;

		else if (Inst.m_EffectAudioLinkedList == null)
			return;

		LinkedListNode<AudioSource> find = Inst.m_EffectAudioLinkedList.Find(audioSource);

		if (find == null)
			return;

		Inst.m_EffectAudioLinkedList.Remove(find);
	}

	public static void PlayNeedBossSpawnAudio(AfterEvent afterEvent)
	{
		Inst.StartCoroutine(Inst.StartBossSpawnAudio(afterEvent));
	}

	private IEnumerator StartBossSpawnAudio(AfterEvent afterEvent)
	{
		if (MusicAudio.IsPlaying)
			MusicAudio.Pause();

		NeedBossSpawnAudio.Play();

		yield return m_WaitBossSpawn;

		afterEvent();
	}

	public static void PauseNeedBossSpawnAudio()
	{
		NeedBossSpawnAudio.Pause();
	}

	public static void ResumeNeedBossSpawnAudio()
	{
		NeedBossSpawnAudio.Resume();
	}

	public static void ResumeBGM()
	{
		if (!Inst)
			return;

		MusicAudio.Resume();
	}

	private static AudioClip GetRandomClip(AudioClip[] clips)
	{
		return clips[UnityEngine.Random.Range(0, clips.Length)];
	}

	public static void PlayStageBGM(bool isResume)
	{
		if (!isResume)
		{
			MusicAudio.ChangeBGM(GetRandomClip(Inst.m_StageClip));
			MusicAudio.Play();
		}

		else
			MusicAudio.Resume();
	}

	public static void PlayBossBGM()
	{
		if (!Inst)
			return;

		MusicAudio.ChangeBGM(GetRandomClip(Inst.m_BossClip));
		MusicAudio.Play();
	}

	public static void PlayBossClearBGM()
	{
		MusicAudio.ChangeBGM(GetRandomClip(Inst.m_BossClearClip), false);
		MusicAudio.Play();
	}

	protected override void Awake()
	{
		base.Awake();

		m_Volume = FileManager.LoadData<AudioVolume>(Data_Type.Audio);

		if (m_Volume == null)
			m_Volume = new AudioVolume();

		Utility.CheckEmpty(m_StageClip, "m_StageClip");

		Inst.m_Audio[(int)Audio_Type.Music].ChangeVolume(VolumeBGM);
		Inst.m_Audio[(int)Audio_Type.NeedBossSpawn].ChangeVolume(VolumeEffect);

		m_EffectAudioLinkedList = new LinkedList<AudioSource>();

		m_WaitBossSpawn = new WaitForSeconds(NeedBossSpawnAudio.Length());
	}
}

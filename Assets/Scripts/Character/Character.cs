using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(AudioSource))]
public class Character : BaseScript, IDamageable
{
	[ReadOnly(true)][SerializeField] protected Char_Type m_Type = Char_Type.Max;

	protected Spawner m_Spawner;
	protected Rigidbody m_Rig;
	protected Animator m_Anim;
	protected CharData m_CharData;
	protected AudioSource m_Audio;
	protected AudioClip[] m_CharClip;
	protected HPBar m_HPBar;
	protected float m_RotSpeed = 7f;
	protected bool m_Dead;
	protected string[] m_AnimName = new string[(int)Anim_Type.Max];
	protected GameObject m_RootObject;

	public event Action OnDeath;

	public Rigidbody Rigidbody => m_Rig;
	public Vector3 Pos { get => m_Rig.position; set => m_Rig.position = value; }
	public Vector3 SpawnerPos => m_Spawner.transform.position;
	public bool HasOnDeath => OnDeath != null;
	public float FireRateTime => m_CharData.FireRateTime;
	public float HP => m_CharData.HP;
	public float HPMax => m_CharData.HPMax;
	public float Damage => m_CharData.Damage;
	public float Range => m_CharData.Range;
	public int Exp => m_CharData.Exp;
	public CharData CharData { set { if (m_CharData == null) m_CharData = value; } }
	public int BulletCount => m_CharData.BulletCount;
	public AudioClip AttackClip => m_CharClip[(int)Audio_Char.Attack];
	public Char_Type Type => m_Type;

	public void Kill()
	{
		TakeDamage(999999f, true);
	}

	protected void PlayAudioDeath()
	{
		if (m_CharClip != null && m_CharClip[(int)Audio_Char.Death])
			m_Audio.PlayOneShot(m_CharClip[(int)Audio_Char.Death]);
	}

	public void Heal(float scale)
	{
		m_CharData.Heal(scale);
	}

	public bool IsDead()
	{
		return m_Dead;
	}

	private bool HasAnim(string animName)
	{
		AnimatorControllerParameter[] param = m_Anim.parameters;

		foreach (AnimatorControllerParameter item in param)
		{
			if (item.name == animName)
				return true;
		}

		return false;
	}

	protected void SetAnimType(Anim_Type type)
	{
		Utility.CheckEmpty(m_Anim, "m_Anim");

		if (m_Dead && type != Anim_Type.Death)
			return;

		int typeIdx = (int)type;
		int count = (int)Anim_Type.Max;

		for (int i = 0; i < count; ++i)
		{
			if (HasAnim(m_AnimName[i]))
			{
				if (i == typeIdx)
					m_Anim.SetBool(m_AnimName[i], true);

				else
					m_Anim.SetBool(m_AnimName[i], false);
			}
		}
	}

	protected override void Awake()
	{
		base.Awake();

		m_RootObject = transform.parent.gameObject;

		m_Rig = GetComponent<Rigidbody>();
		m_Anim = GetComponent<Animator>();
		m_Audio = GetComponent<AudioSource>();
		m_Audio.volume = AudioManager.VolumeEffect;
		m_Spawner = GetComponentInChildren<Spawner>();

		if (m_Type < Char_Type.Boss1)
		{
			m_HPBar = transform.root.GetComponentInChildren<HPBar>();

			Utility.CheckEmpty(m_HPBar, "m_HPBar");
		}

		int count = m_AnimName.Length;

		for (int i = 0; i < count; ++i)
		{
			m_AnimName[i] = ((Anim_Type)i).ToString();
		}

		if (m_Type == Char_Type.Max)
		{ Utility.LogError("m_Type를 제대로 지정하세요!"); }

		else
			m_CharData = DataManager.Clone(m_Type);
	}

	protected override void OnEnable()
	{
		base.OnEnable();

		AudioManager.AddEffectAudio(m_Audio);
	}

	protected override void OnDisable()
	{
		base.OnDisable();

		transform.parent.gameObject.SetActive(false);
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();

		if (!m_Quit)
			AudioManager.RemoveEffectAudio(m_Audio);

		Destroy(transform.parent.gameObject);
	}

	public void TakeDamage(float dmg, bool isCheat = false)
	{
		m_CharData.TakeDamage(dmg, isCheat);

		PlayAudioHit();

		if (m_CharData.HP <= 0f && !m_Dead)
		{
			if (m_Anim)
				DieAnim();

			else
				Die();

			if (m_Type != Char_Type.Player)
				UIManager.AddExp = m_CharData.Exp;
		}
	}

	protected virtual void PlayAudioHit() { }

	public virtual void DieAnim()
	{
		m_Dead = true;

		SetAnimType(Anim_Type.Death);

		PlayAudioDeath();
	}

	public virtual void Die()
	{
		m_Dead = true;

		if (OnDeath != null)
			OnDeath();

		Destroy();
	}

	public virtual void Destroy() { }
}

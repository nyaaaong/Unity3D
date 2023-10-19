using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(AudioSource))]
public class Character : BaseScript, IDamageable
{
	[ReadOnly(true)][SerializeField] protected Char_Type m_Type = Char_Type.Max;

	private List<int> m_BulletAngleList;
	private GameObject m_HitParticlePrefeb;
	protected Spawner m_Spawner;
	protected Rigidbody m_Rig;
	protected Animator m_Anim;
	protected CharData m_CharData;
	protected AudioSource m_Audio;
	protected CharClip m_CharClip;
	protected HPBar m_HPBar;
	protected float m_RotSpeed = 7f;
	protected bool m_Dead;
	protected string[] m_AnimName = new string[(int)Anim_Type.Max];
	protected GameObject m_RootObject;
	protected AudioClip[] m_HitClip;

	protected Action OnDeath;
	protected Action OnDeathInstant;

	public Rigidbody Rigidbody => m_Rig;
	public Vector3 Pos { get => m_Rig.position; set => m_Rig.position = value; }
	public Vector3 SpawnerPos => m_Spawner.transform.position;
	public bool HasOnDeath => OnDeath != null;
	public float FireRateTime => m_CharData.FireRateTime;
	public float FireSpeed => m_CharData.FireSpeed;
	public float HP => m_CharData.HP;
	public float HPMax => m_CharData.HPMax;
	public float Damage => m_CharData.Damage;
	public float Range => m_CharData.Range;
	public float MoveSpeed { get => m_CharData.MoveSpeed; set => m_CharData.MoveSpeed = value; }
	public int Exp => m_CharData.Exp;
	public CharData CharData { get => m_CharData; set { if (m_CharData == null) m_CharData = value; } }
	public int BulletCount => m_CharData.BulletCount;
	public ref readonly AudioClip[] AttackClip => ref m_CharClip.AttackClip;
	public Char_Type Type => m_Type;
	public IReadOnlyList<int> BulletAngleList => m_BulletAngleList;

	private void CreateParticle(Vector3 hitPoint)
	{
		ParticleSystem particle = PoolManager.Get(m_HitParticlePrefeb, hitPoint, Quaternion.identity).GetComponent<ParticleSystem>();
		particle.Play();
	}

	public void AddOnDeathInstantEvent(Action action)
	{
		if (OnDeathInstant != null)
		{
			if (OnDeathInstant.GetInvocationList().Contains(action))
				return;
		}

		OnDeathInstant += action;
	}

	public void AddOnDeathEvent(Action action)
	{
		if (OnDeath != null)
		{
			if (OnDeath.GetInvocationList().Contains(action))
				return;
		}

		OnDeath += action;
	}

	protected void AddBulletAngle(int angle)
	{
		if (!m_BulletAngleList.Contains(angle))
			m_BulletAngleList.Add(angle);
	}

	protected void RemoveBulletAngle(int angle)
	{
		if (m_BulletAngleList.Contains(angle))
			m_BulletAngleList.Remove(angle);
	}

	protected void RemoveAllBulletAngle()
	{
		m_BulletAngleList.Clear();
	}

	public void Kill()
	{
		TakeDamage(999999f, default, false);
	}

	protected void PlayAudioDeath()
	{
		if (m_CharClip != null && m_CharClip.DeathClip)
			m_Audio.PlayOneShot(m_CharClip.DeathClip);
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
		m_BulletAngleList = new List<int>();

		if (m_Type < Char_Type.Boss1)
		{
			m_HPBar = m_RootObject.GetComponentInChildren<HPBar>();

			Utility.CheckEmpty(m_HPBar, "m_HPBar");
		}

		int count = m_AnimName.Length;

		for (int i = 0; i < count; ++i)
		{
			m_AnimName[i] = $"{(Anim_Type)i}";
		}

		if (m_Type == Char_Type.Max)
			Utility.LogError("m_Type를 제대로 지정하세요!");

		else
			m_CharData = DataManager.Clone(m_Type);

		m_HitParticlePrefeb = m_Type == Char_Type.Player ? ParticleManager.GetParticlePrefeb(Particle_Type.PlayerHit) : ParticleManager.GetParticlePrefeb(Particle_Type.MonsterHit);
	}

	protected override void OnEnable()
	{
		base.OnEnable();

		AudioManager.AddEffectAudio(m_Audio);

		if (m_Spawner)
			m_Spawner.gameObject.SetActive(true);
	}

	protected override void OnDisable()
	{
		base.OnDisable();

		if (m_Spawner)
			m_Spawner.gameObject.SetActive(false);
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();

		AudioManager.RemoveEffectAudio(m_Audio);

		Destroy(m_RootObject);
	}

	public void TakeDamage(float dmg, Vector3 hitPoint, bool isMelee, bool isCheat = false)
	{
		m_CharData.TakeDamage(dmg, isCheat);

		if (!isMelee)
			PlayAudioHit();

		if (!isCheat)
			CreateParticle(hitPoint);

		if (m_CharData.HP <= 0f && !m_Dead)
		{
			if (m_Anim)
				DieAnim();

			else
				Die();

			if (m_Type > Char_Type.Player)
				UIManager.AddExp = m_CharData.Exp;

			if (OnDeathInstant != null)
				OnDeathInstant();
		}
	}

	protected virtual void PlayAudioHit()
	{
		m_Audio.PlayOneShot(m_HitClip[UnityEngine.Random.Range(0, m_HitClip.Length)]);
	}

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

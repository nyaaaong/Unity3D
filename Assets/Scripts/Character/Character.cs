using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(AudioSource))]
public class Character : BaseScript, IDamageable
{
	[ReadOnly(true)][SerializeField] protected Char_Type m_Type = Char_Type.Max;
	[ReadOnly(true)][SerializeField] private Renderer[] m_Renderers;

	private List<float> m_BulletAngleList;
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
	protected AudioClip[] m_MeleeHitClip;
	private List<Material> m_Materials;
	private float m_HitEffectTime;
	private float m_HitEffectTimeMax = 0.1f;
	private Coroutine m_HitCoroutine;
	private const string m_EmissionKeyword = "_EMISSION";
	private const string m_EmissionColor = "_EmissionColor";
	private Color m_HitEffectColor;

	protected Action OnDeath;
	protected Action OnDeathInstant;

	public Rigidbody Rigidbody => m_Rig;
	public Vector3 Pos { get => m_Rig.position; set => m_Rig.position = value; }
	public Vector3 SpawnerPos => m_Spawner.transform.position;
	public bool HasOnDeath => OnDeath != null;
	public float FireRateTime => m_CharData.FireRateTime;
	public float FireSpeed { get => m_CharData.FireSpeed; set => m_CharData.FireSpeed = value; }
	public float HP => m_CharData.HP;
	public float HPMax => m_CharData.HPMax;
	public float Damage => m_CharData.Damage;
	public float Range => m_CharData.Range;
	public float MoveSpeed { get => m_CharData.MoveSpeed; set => m_CharData.MoveSpeed = value; }
	public int Exp => m_CharData.Exp;
	public CharData CharData
	{
		get => m_CharData; set
		{
			if (m_CharData == null)
				m_CharData = value;
		}
	}
	public int BulletCount => m_CharData.BulletCount;
	public ref readonly AudioClip[] AttackClip => ref m_CharClip.AttackClip;
	public Char_Type Type => m_Type;
	public IReadOnlyList<float> BulletAngleList => m_BulletAngleList;

	private void BlinkEffect(bool use)
	{
		if (use)
		{
			foreach (Material material in m_Materials)
			{
				material.EnableKeyword(m_EmissionKeyword);
				material.SetColor(m_EmissionColor, m_HitEffectColor);
			}
		}

		else
		{
			foreach (Material material in m_Materials)
			{
				material.DisableKeyword(m_EmissionKeyword);
			}
		}
	}

	private IEnumerator UpdateHitEffect()
	{
		BlinkEffect(true);

		while (true)
		{
			m_HitEffectTime += Time.deltaTime;

			if (m_HitEffectTime >= m_HitEffectTimeMax)
			{
				m_HitEffectTime = 0f;
				break;
			}

			yield return null;
		}

		BlinkEffect(false);
	}

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

	protected void GetBulletAngles(List<float> container)
	{
		foreach (float angle in m_BulletAngleList)
		{
			container.Add(angle);
		}
	}

	protected void SetBulletAngles(in List<float> angles)
	{
		m_BulletAngleList.Clear();
		m_BulletAngleList.AddRange(angles);
	}

	protected int GetAttackCount()
	{
		if (m_Spawner)
			return m_Spawner.GetAttackCount();

		return 0;
	}

	protected void SetAttackCount(int count)
	{
		if (m_Spawner)
			m_Spawner.SetAttackCount(count);
	}

	protected void AddBulletAngle(float angle)
	{
		if (!m_BulletAngleList.Contains(angle))
			m_BulletAngleList.Add(angle);
	}

	protected void RemoveBulletAngle(float angle)
	{
		if (m_BulletAngleList.Contains(angle))
			m_BulletAngleList.Remove(angle);
	}

	protected void RemoveAllBulletAngle()
	{
		m_BulletAngleList.Clear();
	}

	protected void AddAttackCount(int count = 1)
	{
		if (m_Spawner)
			m_Spawner.AddAttackCount(count);
	}

	protected void RemoveAttackCount(int count = 1)
	{
		if (m_Spawner)
			m_Spawner.RemoveAttackCount(count);
	}

	protected void ResetAttackCount()
	{
		if (m_Spawner)
			m_Spawner.ResetAttackCount();
	}

	public void Kill()
	{
		if (m_Dead)
			return;

		TakeDamage(999999f, default, false, true);
	}

	protected void PlayAudioDeath()
	{
		if (m_Audio != null && m_CharClip != null && m_CharClip.DeathClip)
			m_Audio.PlayOneShot(m_CharClip.DeathClip, m_Audio.volume);
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
		if (!m_Anim)
			return false;

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
		if (!m_Anim)
			return;

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
		m_Materials = new List<Material>();
		m_HitEffectColor = RendererManager.HitEffectColor;

		Utility.CheckEmpty(m_Renderers, "m_Renderers");

		const string property = "_EmissionColor";

		foreach (Renderer renderer in m_Renderers)
		{
			foreach (Material material in renderer.materials)
			{
				// 머티리얼 중에 _EmissionColor 플래그가 있는 경우만 추가한다.
				if (material.HasColor(property))
					m_Materials.Add(material);
			}
		}

		m_BulletAngleList = new List<float>();

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

		PlayAudioHit(isMelee);

		if (!isCheat)
			CreateParticle(hitPoint);

		if (m_CharData.HP <= 0f && !m_Dead)
		{
			DieAnim();

			if (m_Type > Char_Type.Player)
				UIManager.AddExp = m_CharData.Exp;

			if (OnDeathInstant != null)
				OnDeathInstant();
		}

		if (m_Materials.Count > 0 && m_Type != Char_Type.Player)
		{
			if (m_HitCoroutine != null)
				StopCoroutine(m_HitCoroutine);

			m_HitCoroutine = StartCoroutine(UpdateHitEffect());
		}
	}

	protected virtual void PlayAudioHit(bool isMelee)
	{
		if (m_Audio != null)
		{
			if (!isMelee)
				m_Audio.PlayOneShot(m_HitClip[UnityEngine.Random.Range(0, m_HitClip.Length)], m_Audio.volume);

			else
				m_Audio.PlayOneShot(m_MeleeHitClip[UnityEngine.Random.Range(0, m_MeleeHitClip.Length)], m_Audio.volume);
		}
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

	protected override void Start()
	{
		base.Start();

		BlinkEffect(false);
	}
}

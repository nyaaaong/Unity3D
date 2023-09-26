using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(AudioSource))]
public class Character : BaseScript, IDamageable
{
	[ReadOnly(true)][SerializeField] protected bool m_IsPlayer;
	[ReadOnly(true)][SerializeField] protected Spawner m_Spawner;

	protected LayerMask m_WallMask;
	protected Rigidbody m_Rig;
	protected Animator m_Anim;
	protected CharInfo m_CharInfo;
	protected AudioSource m_Audio;
	protected AudioClip[] m_AudioClip;
	protected HPBar m_HPBar;
	protected float m_RotSpeed = 7f;
	protected bool m_Dead;
	protected bool m_SetOnDeath;
	protected bool m_Boss;
	protected string[] m_AnimName = new string[(int)Animation_Type.Max];

	public event Action OnDeath;

	public Rigidbody Rigidbody { get { return m_Rig; } }
	public Vector3 Pos { get { return m_Rig.position; } set { m_Rig.position = value; } }
	public Vector3 SpawnerPos { get { return m_Spawner.transform.position; } }
	public bool IsUseOnDeath { get { return OnDeath != null; } }
	public float FireRateTime { get { return m_CharInfo.FireRateTime; } }
	public float HP { get { return m_CharInfo.HP; } }
	public float HPMax { get { return m_CharInfo.HPMax; } }
	public float Damage { get { return m_CharInfo.Damage; } }
	public CharInfo CharInfo { set { if (m_CharInfo == null) m_CharInfo = value; } }
	public int BulletCount { get { return m_CharInfo.BulletCount; } }
	public AudioClip AttackClip { get { return m_AudioClip[(int)Character_Audio.Attack]; } }

	public void Kill()
	{
		TakeDamage(999999f, true);
	}

	protected void PlayDeathAudio()
	{
		if (m_AudioClip != null && m_AudioClip[(int)Character_Audio.Death])
			m_Audio.PlayOneShot(m_AudioClip[(int)Character_Audio.Death]);
	}

	public void Heal(float scale)
	{
		m_CharInfo.Heal(scale);
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

	protected void SetAnimType(Animation_Type type)
	{
#if UNITY_EDITOR
		if (!m_Anim)
			Debug.LogError("if (!m_Anim)");
#endif

		if (m_Dead && type != Animation_Type.Death)
			return;

		int typeIdx = (int)type;
		int count = (int)Animation_Type.Max;

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

		m_WallMask = StageManager.WallMask;

		m_Rig = GetComponent<Rigidbody>();
		m_Anim = GetComponent<Animator>();
		m_Audio = GetComponent<AudioSource>();
		m_Audio.volume = AudioManager.VolumeEffect;

		if (!m_Boss)
		{
			m_HPBar = transform.root.GetComponentInChildren<HPBar>();

#if UNITY_EDITOR
			if (m_HPBar == null)
				Debug.LogError("if (m_HPBar == null)");
#endif
		}

		int count = m_AnimName.Length;

		for (int i = 0; i < count; ++i)
		{
			m_AnimName[i] = ((Animation_Type)i).ToString();
		}
	}

	protected override void OnEnable()
	{
		base.OnEnable();

		AudioManager.AddEffectAudio(m_Audio);
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();

		if (!m_Quit)
			AudioManager.RemoveEffectAudio(m_Audio);
	}

	public virtual void TakeDamage(float dmg, bool isCheat = false)
	{
		m_CharInfo.TakeDamage(dmg, isCheat);

		if (m_CharInfo.HP <= 0f && !m_Dead)
		{
			if (m_Anim)
				DieAnim();

			else
				Die();

			if (!m_IsPlayer)
				UIManager.AddExp = m_CharInfo.Exp;
		}
	}

	public virtual void DieAnim()
	{
		m_Dead = true;

		SetAnimType(Animation_Type.Death);

		PlayDeathAudio();
	}

	public virtual void Die()
	{
		m_Dead = true;

		if (OnDeath != null)
			OnDeath();

		Destroy();
	}

	protected virtual void Destroy() { }
}

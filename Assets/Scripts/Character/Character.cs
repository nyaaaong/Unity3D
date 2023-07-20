using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Character : BaseScript, IDamageable
{
	public enum Animation_Type
	{
		Idle,
		Move,
		Attack,
		Death
	}

	[ReadOnly(true)][SerializeField] protected bool m_Boss;
	[ReadOnly(true)][SerializeField] protected HPBar m_HPBarCanvas;
	[ReadOnly(true)][SerializeField] protected Spawner m_Spawner;

	protected LayerMask m_WallMask;
	protected Rigidbody m_Rig;
	protected Animator m_Anim;
	protected CharInfo m_CharInfo;
	protected float m_RotSpeed = 7f;
	protected bool m_Dead;
	protected bool m_SetOnDeath;
	private List<string> m_AnimType = null;

	public event Action OnDeath;

	public Rigidbody Rigidbody { get { return m_Rig; } }
	public Vector3 Pos { get { return m_Rig.position; } }
	public Vector3 SpawnerPos { get { return m_Spawner.transform.position; } }
	public bool IsUseOnDeath { get { return OnDeath != null; } }
	public float FireRateTime { get { return m_CharInfo.FireRateTime; } }
	public float HP { get { return m_CharInfo.HP; } }
	public float HPMax { get { return m_CharInfo.HPMax; } }
	public float Damage { get { return m_CharInfo.Damage; } }
	public CharInfo CharInfo { set { if (m_CharInfo == null) m_CharInfo = value; } }

	public void Cheat(Cheat_Type type, bool isChecked)
	{
		if (!m_Dead)
		{
			switch (type)
			{
				case Cheat_Type.PowerUp:
					m_CharInfo.PowerUp = isChecked;
					break;
				case Cheat_Type.NoHit:
					m_CharInfo.NoHit = isChecked;
					break;
			}
		}
	}

	public void Heal(float scale)
	{
		m_CharInfo.Heal(scale);
	}

	public bool IsDead()
	{
		return m_Dead;
	}

	private bool HasAnimBoolParam(string animName)
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
		if (!m_Anim)
			Debug.LogError("if (!m_Anim)");

		else if (m_Dead && type != Animation_Type.Death)
			return;

		string text = "";

		switch (type)
		{
			case Animation_Type.Idle:
				text = "Idle";
				break;
			case Animation_Type.Move:
				text = "Move";
				break;
			case Animation_Type.Attack:
				text = "Attack";
				break;
			case Animation_Type.Death:
				text = "Death";
				break;
		}

		foreach (var item in m_AnimType)
		{
			if (!HasAnimBoolParam(item))
				continue;

			if (item == text)
				m_Anim.SetBool(item, true);

			else
				m_Anim.SetBool(item, false);
		}
	}

	protected override void Awake()
	{
		base.Awake();

		m_WallMask = StageManager.WallLayer;

		m_Rig = GetComponent<Rigidbody>();
		m_Anim = GetComponent<Animator>();

		if (!m_Spawner)
			Debug.LogError("if (!m_Spawner)");

		if (!m_HPBarCanvas)
			Debug.LogError("if (!m_HPBarCanvas)");

		if (m_Anim)
		{
			m_AnimType = new List<string>();

			m_AnimType.Add("Idle");
			m_AnimType.Add("Attack");
			m_AnimType.Add("Death");
		}
	}

	protected override void OnEnable()
	{
		base.OnEnable();

		m_Dead = false;
		m_CharInfo.Heal(1f);

		m_HPBarCanvas.gameObject.SetActive(true);
	}

	public virtual void TakeDamage(float dmg)
	{
		m_CharInfo.TakeDamage(dmg);

		if (m_CharInfo.HP <= 0f && !m_Dead)
		{
			if (m_Anim)
				DieAnim();

			else
				Die();
		}
	}

	// hit 부분에 이펙트 출력
	public virtual void TakeHit(float dmg, RaycastHit hit)
	{
		m_CharInfo.TakeDamage(dmg);

		if (m_CharInfo.HP <= 0f && !m_Dead)
		{
			if (m_Anim)
				DieAnim();

			else
				Die();
		}
	}

	public virtual void DieAnim()
	{
		m_Dead = true;

		SetAnimType(Animation_Type.Death);
	}

	public virtual void Die()
	{
		if (OnDeath != null)
			OnDeath();

		Destroy();
	}

	protected virtual void Destroy() { }
}

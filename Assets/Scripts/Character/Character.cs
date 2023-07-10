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

	[SerializeField] private Bullet_Type m_BulletType;
	[SerializeField] protected float m_HPMax = 100f;
	[SerializeField] protected float m_HP = 100f;
	[SerializeField] protected float m_FireRateTime = 1f;
	[SerializeField] protected float m_Damage = 1f;
	[SerializeField] protected float m_MoveSpeed = 5f;
	[SerializeField] protected bool m_Boss;
	[SerializeField] protected HPBar m_HPBarCanvas;
	[SerializeField] protected Spawner m_Spawner;

	protected LayerMask m_WallMask;
	protected Rigidbody m_Rig;
	protected Animator m_Anim;
	protected float m_RotSpeed = 7f;
	protected bool m_Dead;
	protected bool m_SetOnDeath;
	private List<string> m_AnimType = null;

	public event Action OnDeath;

	public Rigidbody Rigidbody { get { return m_Rig; } }
	public Vector3 Pos { get { return m_Rig.position; } }
	public Vector3 SpawnerPos { get { return m_Spawner.transform.position; } }
	public bool IsUseOnDeath { get { return OnDeath != null; } }
	public float FireRateTime { get { return m_FireRateTime; } }
	public float HP { get { return m_HP; } }
	public float HPMax { get { return m_HPMax; } }

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
		m_HP = m_HPMax;

		m_HPBarCanvas.gameObject.SetActive(true);
	}

	public virtual void TakeDamage(float dmg)
	{
		m_HP -= dmg;

		if (0 == m_HP && !m_Dead)
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
		m_HP -= dmg;

		if (0 == m_HP && !m_Dead)
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

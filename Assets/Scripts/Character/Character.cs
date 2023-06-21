using System;
using UnityEngine;

public class Character : BaseScript, IDamageable
{
	[SerializeField] protected float m_HPMax = 100f;

	[SerializeField] protected float m_HP = 100f;
	protected float m_MoveSpeed = 5f;
	protected float m_RotSpeed = 7f;
	protected bool m_Dead;
	protected bool m_SetOnDeath;

	public event Action OnDeath;

	public bool IsUseOnDeath { get { return OnDeath != null; } }

	public void SetInfo(Vector3 pos, Quaternion rot)
	{
		transform.position = pos;
		transform.rotation = rot;
	}

	protected override void OnEnable()
	{
		base.OnEnable();

		m_Dead = false;
		m_HP = m_HPMax;
	}

	public virtual void TakeHit(float dmg, RaycastHit hit)
	{
		m_HP -= dmg;

		if (0 >= m_HP && !m_Dead)
			Die();
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

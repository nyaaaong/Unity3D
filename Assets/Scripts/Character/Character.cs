using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Character : BaseScript, IDamageable
{
	[SerializeField] private Bullet_Type m_BulletType;
	[SerializeField] protected float m_HPMax = 100f;
	[SerializeField] protected float m_HP = 100f;
	[SerializeField] protected float m_FireRateTime = 1f;
	[SerializeField] protected float m_Damage = 1f;
	[SerializeField] protected float m_MoveSpeed = 5f;
	[SerializeField] protected bool m_Boss;
	[SerializeField] protected HPBar m_HPBarCanvas;
	[SerializeField] protected Spawner m_Spawner;

	protected Rigidbody m_Rig;
	protected float m_RotSpeed = 7f;
	protected bool m_Dead;
	protected bool m_SetOnDeath;

	public event Action OnDeath;

	public Rigidbody Rigidbody { get { return m_Rig; } }
	public Vector3 Pos { get { return m_Rig.position; } }
	public bool IsUseOnDeath { get { return OnDeath != null; } }
	public float FireRateTime { get { return m_FireRateTime; } }
	public float HP { get { return m_HP; } }
	public float HPMax { get { return m_HPMax; } }

	protected override void Awake()
	{
		base.Awake();

		m_Rig = GetComponent<Rigidbody>();

		if (!m_Spawner)
			Debug.LogError("if (!m_Spawner)");

		if (!m_HPBarCanvas)
			Debug.LogError("if (!m_HPBarCanvas)");
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
			Die();
	}

	// hit 부분에 이펙트 출력
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

	protected virtual void Destroy() { }
}

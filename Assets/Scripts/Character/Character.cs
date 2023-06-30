using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody))]
public class Character : BaseScript, IDamageable
{
	[SerializeField] protected Gun m_NormalGun;
	[SerializeField] private Transform m_Hand;
	[SerializeField] protected float m_HPMax = 100f;
	[SerializeField] protected float m_HP = 100f;
	[SerializeField] protected float m_FireRateTime = 1f;
	[SerializeField] protected float m_Damage = 1f;
	[SerializeField] protected bool m_Boss;
	[SerializeField] protected HPBar m_HPBarCanvas;

	protected Rigidbody m_Rig;
	protected float m_MoveSpeed = 5f;
	protected float m_RotSpeed = 7f;
	protected bool m_Dead;
	protected bool m_SetOnDeath;
	protected Gun m_Gun;

	public event Action OnDeath;

	public Rigidbody Rigidbody { get { return m_Rig; } }
	public Vector3 Pos { get { return m_Rig.position; } }
	public bool IsUseOnDeath { get { return OnDeath != null; } }
	public float FireRateTime { get { return m_FireRateTime; } }
	public float HP { get { return m_HP; } }
	public float HPMax { get { return m_HPMax; } }

	protected void EquipGun(Gun gun)
	{
		if (m_Gun)
			Destroy(m_Gun.gameObject);

		m_Gun = Instantiate(gun, m_Hand.position, m_Hand.rotation);
		m_Gun.transform.parent = m_Hand;
	}

	public void SetCharacterInfo(Vector3 pos, Quaternion rot)
	{
		transform.position = pos;
		transform.rotation = rot;
	}

	protected override void Awake()
	{
		base.Awake();

		m_Rig = GetComponent<Rigidbody>();

		if (m_NormalGun)
		{
			if (!m_Hand)
				Debug.LogError("if (!m_Hand)");

			EquipGun(m_NormalGun);
		}

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

	protected override void OnDisable()
	{
		base.OnDisable();

		//m_HPBarCanvas.gameObject.SetActive(false);
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

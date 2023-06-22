using System.Collections;
using UnityEngine;
using UnityEngine.Pool;

public class Bullet : BaseScript
{
	private Bullet_Owner m_Owner = Bullet_Owner.Player;
	private LayerMask m_CollisionMask;
	private IObjectPool<Bullet> m_Pool;
	private Vector3 m_InitPos;
	private Quaternion m_InitRot;
	private float m_Speed = 10f;
	private float m_Dist = 10f;
	private float m_AccDist = 0f;
	private float m_MaxDist = 5f;
	private float m_Damage = 1f;
	private bool m_Destroy;

	private void CheckCollisions(float dist)
	{
		Ray ray = new Ray(transform.position, transform.forward);
		RaycastHit hit;

		if (Physics.Raycast(ray, out hit, m_Dist, m_CollisionMask, QueryTriggerInteraction.Collide))
			OnHit(hit);
	}

	private void OnHit(RaycastHit hit)
	{
		IDamageable damageableObj = hit.collider.GetComponent<IDamageable>();

		if (damageableObj != null)
			damageableObj.TakeHit(m_Damage, hit);

		Destroy();
	}

	public void SetPool(IObjectPool<Bullet> pool)
	{
		m_Pool = pool;
	}

	public void Destroy()
	{
		m_Destroy = true;

		m_Pool.Release(this);
	}

	public void SetSpeed(float speed)
	{
		m_Speed = speed;
	}

	private IEnumerator CheckDist()
	{
		while (m_AccDist < m_MaxDist)
		{
			m_Dist = m_deltaTime * m_Speed;
			m_AccDist += m_Dist;

			yield return null;
		}

		Destroy();
	}

	public void SetWeaponInfo(Transform tr, float maxDist, Bullet_Owner owner, float dmg)
	{
		m_InitPos = tr.position;
		m_InitRot = tr.rotation;

		transform.position = m_InitPos;
		transform.rotation = m_InitRot;

		m_MaxDist = maxDist;
		m_Owner = owner;
		m_Damage = dmg;

		switch (owner)
		{
			case Bullet_Owner.Player:
				m_CollisionMask = StageManager.MonsterLayer;
				break;
			case Bullet_Owner.Monster:
				m_CollisionMask = StageManager.PlayerLayer;
				break;
		}
	}

	protected override void OnEnable()
	{
		base.OnEnable();

		m_AccDist = 0f;

		m_Destroy = false;

		StartCoroutine(CheckDist());
	}

	protected override void BeforeUpdate()
	{
		if (!m_Destroy)
		{
			base.BeforeUpdate();

			CheckCollisions(m_Dist);

			transform.Translate(Vector3.forward * m_Dist);
		}
	}
}

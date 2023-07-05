using System.Collections;
using UnityEngine;
using UnityEngine.Pool;

public class Bullet : BaseScript
{
	private LayerMask m_CollisionMask;
	private LayerMask m_WallMask;
	private IObjectPool<Bullet> m_Pool;
	private Vector3 m_InitPos;
	private Quaternion m_InitRot;
	private Ray m_Ray = new Ray();
	private RaycastHit m_Hit;
	private float m_Speed = 10f;
	private float m_Dist = 10f;
	private float m_Damage = 1f;
	private bool m_Destroy;

	private void CheckCollisions(float dist)
	{
		m_Ray.origin = transform.position;
		m_Ray.direction = transform.forward;

		if (Physics.Raycast(m_Ray, out m_Hit, m_Dist, m_WallMask, QueryTriggerInteraction.Collide))
			Destroy();

		else if (Physics.Raycast(m_Ray, out m_Hit, m_Dist, m_CollisionMask, QueryTriggerInteraction.Collide))
			OnHit(m_Hit);
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
		while (!m_Destroy)
		{
			m_Dist = m_deltaTime * m_Speed;

			yield return null;
		}
	}

	public void SetSpawnInfo(Transform tr, Bullet_Type type, float dmg)
	{
		m_InitPos = tr.position;
		m_InitRot = tr.rotation;

		transform.position = m_InitPos;
		transform.rotation = m_InitRot;

		m_Damage = dmg;

		switch (type)
		{
			case Bullet_Type.Player:
				m_CollisionMask = StageManager.MonsterLayer;
				break;
			case Bullet_Type.Range:
				m_CollisionMask = StageManager.PlayerLayer;
				break;
		}

		m_WallMask = StageManager.WallLayer;
	}

	protected override void OnEnable()
	{
		base.OnEnable();

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

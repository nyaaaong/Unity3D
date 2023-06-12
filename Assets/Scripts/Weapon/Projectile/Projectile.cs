using UnityEngine;
using UnityEngine.Pool;

public class Projectile : BaseScript
{
	private float m_Speed = 10f;
	private IObjectPool<Projectile> m_Pool;

	public void SetPool(IObjectPool<Projectile> pool)
	{
		m_Pool = pool;
	}

	public void DestroyProjectile()
	{
		m_Pool.Release(this);
	}

	public void SetSpeed(float speed)
	{
		m_Speed = speed;
	}

	protected override void Start()
	{
		Invoke("DestroyProjectile", 5f);
	}

	protected override void BeforeUpdate()
	{
		base.BeforeUpdate();

		transform.Translate(Vector3.forward * m_deltaTime * m_Speed);
	}
}

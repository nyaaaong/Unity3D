using UnityEngine;
using UnityEngine.Pool;

public class Gun : BaseScript
{
	[SerializeField] private Transform m_Muzzle;
	[SerializeField] private Projectile m_Projectile;
	[SerializeField] private float m_msFireRate = 1000f;
	[SerializeField] private float m_FireVelocity = 35f;

	private float m_NextShotTime;
	private IObjectPool<Projectile> m_Pool;

	protected override void Awake()
	{
		base.Awake();

		m_Pool = new ObjectPool<Projectile>(CreateProjectile, OnGetProjectile, OnReleaseProjectile, OnDestroyProjectile, maxSize:20);
	}

	public void Shoot()
	{
		if (Time.time > m_NextShotTime)
		{
			m_NextShotTime = Time.time + m_msFireRate * 0.001f; // 1000À» ³ª´²¼­ ms·Î ¹Ù²ãÁØ´Ù.
			Projectile newProj = m_Pool.Get();
		}
	}

	private Projectile CreateProjectile()
	{
		Projectile proj = Instantiate(m_Projectile, m_Muzzle.position, m_Muzzle.rotation).GetComponent<Projectile>();
		proj.SetSpeed(m_FireVelocity);
		proj.SetPool(m_Pool);

		return proj;
	}

	private void OnGetProjectile(Projectile proj)
	{
		proj.gameObject.SetActive(true);
	}

	private void OnReleaseProjectile(Projectile proj)
	{
		proj.gameObject.SetActive(false);
	}

	private void OnDestroyProjectile(Projectile proj)
	{
		Destroy(proj.gameObject);
	}
}

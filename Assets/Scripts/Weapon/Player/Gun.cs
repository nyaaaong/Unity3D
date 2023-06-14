using UnityEngine;
using UnityEngine.Pool;

public class Gun : BaseScript
{
	[SerializeField] private Transform m_Muzzle;
	[SerializeField] private Bullet m_Bullet;
	[SerializeField] private float m_msFireRate = 1000f;
	[SerializeField] private float m_FireVelocity = 35f;

	private float m_NextShotTime;
	private IObjectPool<Bullet> m_Pool;

	protected override void Awake()
	{
		base.Awake();

		m_Pool = new ObjectPool<Bullet>(CreateBullet, OnGetBullet, OnReleaseBullet, OnDestroyBullet, maxSize:20);
	}

	public void Shoot()
	{
		if (Time.time > m_NextShotTime)
		{
			m_NextShotTime = Time.time + m_msFireRate * 0.001f; // 1000을 나눠서 ms로 바꿔준다.
			m_Pool.Get();
		}
	}

	protected Bullet CreateBullet()
	{
		Bullet bullet = Instantiate(m_Bullet).GetComponent<Bullet>();
		bullet.SetInfo(m_Muzzle);
		bullet.SetSpeed(m_FireVelocity);
		bullet.SetPool(m_Pool);

		return bullet;
	}

	protected void OnGetBullet(Bullet bullet)
	{
		bullet.gameObject.SetActive(true);
		bullet.SetInfo(m_Muzzle); // 새로 활성화 될 때마다 위치, 회전 정보를 갱신 시켜줘야 한다.
	}

	protected void OnReleaseBullet(Bullet bullet)
	{
		bullet.gameObject.SetActive(false);
	}

	protected void OnDestroyBullet(Bullet bullet)
	{
		Destroy(bullet.gameObject);
	}
}

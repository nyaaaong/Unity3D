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
			m_NextShotTime = Time.time + m_msFireRate * 0.001f; // 1000�� ������ ms�� �ٲ��ش�.
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
		bullet.SetInfo(m_Muzzle); // ���� Ȱ��ȭ �� ������ ��ġ, ȸ�� ������ ���� ������� �Ѵ�.
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

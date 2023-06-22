using UnityEngine;
using UnityEngine.Pool;

public class Gun : BaseScript
{
	[SerializeField] private Transform m_Muzzle;
	[SerializeField] private Bullet m_Bullet;

	private float m_Timer;
	private float m_MaxDist;
	private float m_FireRateTime = 1f;
	private float m_FireVelocity = 35f;
	private Bullet_Owner m_Owner;
	private IObjectPool<Bullet> m_Pool;

	public void SetInfo(float maxDist, Bullet_Owner owner, float fireRateTime)
	{
		m_MaxDist = maxDist;
		m_Owner = owner;
		m_FireRateTime = fireRateTime;
	}

	public void Shoot(bool shoot)
	{
		if (shoot)
		{
			m_Timer += m_deltaTime;

			if (m_Timer >= m_FireRateTime)
			{
				m_Timer = 0f;
				m_Pool.Get();
			}
		}

		else
			m_Timer = m_FireRateTime;
	}

	public void StageClear()
	{
		m_Pool.Clear();
	}

	protected override void Awake()
	{
		base.Awake();

		m_Pool = new ObjectPool<Bullet>(CreateBullet, OnGetBullet, OnReleaseBullet, OnDestroyBullet, maxSize:20);
	}

	protected override void OnEnable()
	{
		base.OnEnable();

		m_Timer = m_FireRateTime;
	}

	protected override void Start()
	{
		base.Start();

		StageManager.AddStageClear(StageClear);
	}

	protected Bullet CreateBullet()
	{
		Bullet bullet = Instantiate(m_Bullet).GetComponent<Bullet>();
		bullet.SetInfo(m_Muzzle, m_MaxDist, m_Owner);
		bullet.SetSpeed(m_FireVelocity);
		bullet.SetPool(m_Pool);

		return bullet;
	}

	protected void OnGetBullet(Bullet bullet)
	{
		bullet.gameObject.SetActive(true);
		bullet.SetInfo(m_Muzzle, m_MaxDist, m_Owner); // 새로 활성화 될 때마다 위치, 회전 정보를 갱신 시켜줘야 한다.
	}

	protected void OnReleaseBullet(Bullet bullet)
	{
		bullet.gameObject.SetActive(false);
	}

	protected void OnDestroyBullet(Bullet bullet)
	{
		if (bullet)
		{
			if (bullet.gameObject)
				Destroy(bullet.gameObject);
		}
	}
}

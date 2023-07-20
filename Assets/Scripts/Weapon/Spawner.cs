using UnityEngine;
using UnityEngine.Pool;

public class Spawner : BaseScript
{
	[ReadOnly(true)][SerializeField] private Bullet m_Bullet;

	private Character m_Owner;
	private float m_FireVelocity = 35f;
	private Character_Type m_Type;
	private IObjectPool<Bullet> m_Pool;

	public void SetSpawnerInfo(Character owner, Character_Type type)
	{
		m_Owner = owner;
		m_Type = type;
	}

	public void Attack()
	{
		m_Pool.Get();
	}

	public void StageClear()
	{
		m_Pool.Clear();
	}

	protected override void Awake()
	{
		base.Awake();

		m_Pool = new ObjectPool<Bullet>(CreateBullet, OnGetBullet, OnReleaseBullet, OnDestroyBullet, maxSize: 20);
	}

	protected override void Start()
	{
		base.Start();

		StageManager.AddStageClear(StageClear);
	}

	protected Bullet CreateBullet()
	{
		Bullet bullet = Instantiate(m_Bullet).GetComponent<Bullet>();
		bullet.SetSpawnerInfo(transform, m_Type, m_Owner.Damage);
		bullet.SetSpeed(m_FireVelocity);
		bullet.SetPool(m_Pool);

		return bullet;
	}

	protected void OnGetBullet(Bullet bullet)
	{
		bullet.gameObject.SetActive(true);
		bullet.SetSpawnerInfo(transform, m_Type, m_Owner.Damage); // 새로 활성화 될 때마다 위치, 회전 정보를 갱신 시켜줘야 한다.
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

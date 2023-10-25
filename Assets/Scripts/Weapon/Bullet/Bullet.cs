
using System.Collections;
using UnityEngine;

public class Bullet : BaseScript
{
	[ReadOnly(true)][SerializeField] private Transform m_RadiusObject; // 반지름을 알게 하기 위해 아무 방향이나 가장자리에 오브젝트를 놓아준다.

	private GameObject m_HitWallParticlePrefeb;
	private Character m_Owner;
	private Char_Type m_OwnerType;
	private LayerMask m_TargetMask;
	private LayerMask m_WallMask;
	private Vector3 m_Dir;
	private float m_Radius;
	private float m_Speed;
	private float m_Range; // 누적 거리
	private float m_RangeMax; // 최대 거리
	private float m_Damage;
	private bool m_Update;
	private bool m_Hit; // 총알으로 데미지를 입혔다면 활성화한다.
	private Collider[] m_Colliders = new Collider[5];
	private int m_CollidersCount;
	private Collider m_Collider;
	private float m_ColliderDist;
	private float m_NearDist;
	private Vector3 m_HitPos;

	private void CreateHitWallParticle()
	{
		ParticleSystem particle = PoolManager.Get(m_HitWallParticlePrefeb, m_HitPos, Quaternion.identity).GetComponent<ParticleSystem>();
		particle.Play();
	}

	protected override void OnEnable()
	{
		base.OnEnable();

		m_Hit = false;

		StartCoroutine(CheckOutRange());
	}

	private IEnumerator CheckOutRange()
	{
		m_Range = 0f;

		while (true)
		{
			if (m_Update)
			{
				m_Range += Time.deltaTime * m_Speed;

				if (m_Range >= m_RangeMax)
					Destroy();
			}

			yield return null;
		}
	}

	private bool IsCollision(LayerMask layer)
	{
		m_CollidersCount = Physics.OverlapSphereNonAlloc(transform.position, m_Radius, m_Colliders, layer, QueryTriggerInteraction.Collide);

		if (m_CollidersCount > 0)
		{
			CheckNearCollider();
			m_HitPos = transform.position;
			return true;
		}

		return false;
	}

	private void CheckNearCollider()
	{
		m_NearDist = Mathf.Infinity;
		
		for (int i = 0; i < m_CollidersCount; ++i)
		{
			m_ColliderDist = Vector3.Distance(transform.position, m_Colliders[i].transform.position);

			if (m_NearDist > m_ColliderDist)
			{
				m_NearDist = m_ColliderDist;
				m_Collider = m_Colliders[i];
			}
		}
	}

	// 총알의 꼬리를 기준으로 머리부분까지 광선을 쏴주고 그 안에 충돌체가 들어있는지 판단한다.
	private void CheckCollision()
	{
		if (IsCollision(m_WallMask))
		{
			CreateHitWallParticle();
			Destroy();
		}

		else if (IsCollision(m_TargetMask))
			OnHit();
	}

	private void OnHit()
	{
		IDamageable damageableObj = m_Collider.transform.GetComponent<IDamageable>();

		if (damageableObj.IsDead() || m_Hit)
			return;

		m_Hit = true;

		damageableObj.TakeDamage(m_Damage, m_HitPos, false);
		Destroy();
	}

	public void Destroy()
	{
		if (m_Update)
		{
			m_Update = false;

			PoolManager.Release(gameObject);
		}
	}

	public void SetDetailData(Character owner, Vector3 dir)
	{
		m_Owner = owner;
		m_OwnerType = owner.Type;
		m_Speed = owner.FireSpeed;
		m_Damage = m_Owner.Damage;
		// 최대 거리에 총알의 앞부분을 빼줘서 실제 최대 거리를 구한다.
		m_RangeMax = m_Owner.Range - m_Radius;
		m_Dir = dir;

		m_TargetMask = m_OwnerType == Char_Type.Player ? StageManager.MonsterMask : StageManager.PlayerMask;

		m_WallMask = StageManager.WallMask;

		m_Update = true;
	}

	protected override void Awake()
	{
		base.Awake();

		m_HitWallParticlePrefeb = ParticleManager.GetParticlePrefeb(Particle_Type.HitWall);

		Utility.CheckEmpty(m_RadiusObject, "m_RadiusObject");

		m_Radius = Vector3.Distance(transform.position, m_RadiusObject.position);
	}

	protected override void Update()
	{
		if (m_Update)
		{
			base.Update();

			CheckCollision();

			transform.Translate(m_Dir * Time.deltaTime * m_Speed);
		}
	}
}


using System.Collections;
using UnityEngine;

public class Bullet : BaseScript
{
	[ReadOnly(true)][SerializeField] private Transform m_RadiusObject; // 반지름을 알게 하기 위해 아무 방향이나 가장자리에 오브젝트를 놓아준다.

	private GameObject m_HitWallParticlePrefeb;
	private Character m_Owner;
	private Char_Type m_OwnerType;
	private Vector3 m_Dir;
	private float m_Radius;
	private float m_Speed;
	private float m_Range; // 누적 거리
	private float m_RangeMax; // 최대 거리
	private float m_Damage;
	private bool m_Update;
	private bool m_Hit; // 총알으로 데미지를 입혔다면 활성화한다.
	private string m_TargetTag;
	private string m_WallTag;

	public float FireSpeed { get => m_Speed; set => m_Speed = value; }

	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag(m_WallTag))
		{
			CreateHitWallParticle();
			Destroy();
		}

		else if (other.CompareTag(m_TargetTag))
			OnHit(other);
	}

	private void CreateHitWallParticle()
	{
		ParticleSystem particle = PoolManager.Get(m_HitWallParticlePrefeb, transform.position, Quaternion.identity).GetComponent<ParticleSystem>();
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

	private void OnHit(Collider other)
	{
		IDamageable damageableObj = other.GetComponent<IDamageable>();

		if (damageableObj.IsDead() || m_Hit)
			return;

		m_Hit = true;

		damageableObj.TakeDamage(m_Damage, transform.position, false);
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
		m_Damage = m_Owner.Damage;
		// 최대 거리에 총알의 앞부분을 빼줘서 실제 최대 거리를 구한다.
		m_RangeMax = m_Owner.Range - m_Radius;
		m_Dir = dir;

		m_TargetTag = m_OwnerType == Char_Type.Player ? StageManager.MonsterTag : StageManager.PlayerTag;

		m_Update = true;
	}

	protected override void Awake()
	{
		base.Awake();

		m_HitWallParticlePrefeb = ParticleManager.GetParticlePrefeb(Particle_Type.HitWall);

		Utility.CheckEmpty(m_RadiusObject, "m_RadiusObject");

		m_Radius = Vector3.Distance(transform.position, m_RadiusObject.position);

		m_WallTag = StageManager.WallTag;
	}

	protected override void Update()
	{
		if (m_Update)
		{
			base.Update();

			//CheckCollision();

			transform.Translate(m_Dir * Time.deltaTime * m_Speed);
		}
	}
}

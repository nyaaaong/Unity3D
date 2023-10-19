
using System.Collections;
using UnityEngine;

public class Bullet : BaseScript
{
	[EnumArray(typeof(Bullet_Position))][ReadOnly(true)][SerializeField] private Transform[] m_BulletPos = new Transform[(int)Bullet_Position.Max];

	private GameObject m_HitWallParticlePrefeb;
	private Character m_Owner;
	private Char_Type m_OwnerType;
	private LayerMask m_TargetMask;
	private LayerMask m_WallMask;
	private Ray m_Ray;
	private Ray m_RaySide;
	private RaycastHit m_RaycastHit;
	private float m_Speed = 1f;
	private float m_Range; // ���� �Ÿ�
	private float m_RangeMax; // �ִ� �Ÿ�
	private float m_Damage = 1f;
	private float m_LengthHT; // �Ѿ��� �� ���� (�Ӹ�~����)
	private float m_LengthLR; // �Ѿ��� �� ���� (�� ��)
	private float m_HeadLength; // �Ѿ��� �պκ� ���� (�Ӹ�~�߽�)
	private bool m_Update;
	private bool m_Hit; // �Ѿ����� �������� �����ٸ� Ȱ��ȭ�Ѵ�.

	private void CreateHitWallParticle()
	{
		ParticleSystem particle = PoolManager.Get(m_HitWallParticlePrefeb, m_RaycastHit.point, Quaternion.identity).GetComponent<ParticleSystem>();
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

	// �Ѿ��� ������ �������� �Ӹ��κб��� ������ ���ְ� �� �ȿ� �浹ü�� ����ִ��� �Ǵ��Ѵ�.
	private void CheckCollision()
	{
		m_Ray.direction = transform.forward;
		m_Ray.origin = m_BulletPos[(int)Bullet_Position.Tail].position;

		if (Physics.Raycast(m_Ray, out m_RaycastHit, m_LengthHT, m_WallMask, QueryTriggerInteraction.Collide) ||
			Physics.Raycast(m_RaySide, out m_RaycastHit, m_LengthLR, m_WallMask, QueryTriggerInteraction.Collide))
		{
			CreateHitWallParticle();
			Destroy();
		}

		else if (Physics.Raycast(m_Ray, out m_RaycastHit, m_LengthHT, m_TargetMask, QueryTriggerInteraction.Collide) ||
			Physics.Raycast(m_RaySide, out m_RaycastHit, m_LengthLR, m_TargetMask, QueryTriggerInteraction.Collide))
			OnHit();
	}

	private void OnHit()
	{
		IDamageable damageableObj = m_RaycastHit.transform.GetComponent<IDamageable>();

		if (damageableObj.IsDead() || m_Hit)
			return;

		m_Hit = true;

		damageableObj.TakeDamage(m_Damage, m_RaycastHit.point, false);
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

	public void SetDetailData(Character owner)
	{
		m_Owner = owner;
		m_OwnerType = owner.Type;
		m_Speed = owner.FireSpeed;
		m_Damage = m_Owner.Damage;
		// �ִ� �Ÿ��� �Ѿ��� �պκ��� ���༭ ���� �ִ� �Ÿ��� ���Ѵ�.
		m_RangeMax = m_Owner.Range - m_HeadLength;

		if (m_OwnerType == Char_Type.Player)
			m_TargetMask = StageManager.MonsterMask;

		else
			m_TargetMask = StageManager.PlayerMask;

		m_WallMask = StageManager.WallMask;

		m_Update = true;
	}

	protected override void Awake()
	{
		base.Awake();

		m_Ray = new Ray();
		m_RaySide = new Ray();

		Utility.CheckEmpty(m_BulletPos, "m_BulletPos");

		m_LengthHT = Vector3.Distance(m_BulletPos[(int)Bullet_Position.Head].position, m_BulletPos[(int)Bullet_Position.Tail].position);
		m_LengthLR = Vector3.Distance(m_BulletPos[(int)Bullet_Position.Left].position, m_BulletPos[(int)Bullet_Position.Right].position);
		m_HeadLength = Vector3.Distance(transform.position, m_BulletPos[(int)Bullet_Position.Head].position);
		m_HitWallParticlePrefeb = ParticleManager.GetParticlePrefeb(Particle_Type.HitWall);
	}

	protected override void Update()
	{
		if (m_Update)
		{
			base.Update();

			CheckCollision();

			transform.Translate(Vector3.forward * Time.deltaTime * m_Speed);
		}
	}
}

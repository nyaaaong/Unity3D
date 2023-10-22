
using System.Collections;
using UnityEngine;

public class Bullet : BaseScript
{
	[ReadOnly(true)][SerializeField] private Transform m_RadiusObject; // �������� �˰� �ϱ� ���� �ƹ� �����̳� �����ڸ��� ������Ʈ�� �����ش�.

	private GameObject m_HitWallParticlePrefeb;
	private Character m_Owner;
	private Char_Type m_OwnerType;
	private LayerMask m_TargetMask;
	private LayerMask m_WallMask;
	private RaycastHit m_RaycastHit;
	private Vector3 m_Dir;
	private float m_Radius;
	private float m_Speed;
	private float m_Range; // ���� �Ÿ�
	private float m_RangeMax; // �ִ� �Ÿ�
	private float m_Damage;
	private float m_Distance = 0.6f;
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

	private bool IsCollision(LayerMask layer)
	{
		return Physics.SphereCast(transform.position, m_Radius, m_Dir, out m_RaycastHit, m_Distance, layer, QueryTriggerInteraction.Collide);
	}

	// �Ѿ��� ������ �������� �Ӹ��κб��� ������ ���ְ� �� �ȿ� �浹ü�� ����ִ��� �Ǵ��Ѵ�.
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

	public void SetDetailData(Character owner, Vector3 dir)
	{
		m_Owner = owner;
		m_OwnerType = owner.Type;
		m_Speed = owner.FireSpeed;
		m_Damage = m_Owner.Damage;
		// �ִ� �Ÿ��� �Ѿ��� �պκ��� ���༭ ���� �ִ� �Ÿ��� ���Ѵ�.
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

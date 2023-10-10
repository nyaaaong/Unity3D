
using System.Collections;
using UnityEngine;

public class Bullet : BaseScript
{
	[EnumArray(typeof(Bullet_Position))][ReadOnly(true)][SerializeField] private Transform[] m_BulletPos = new Transform[(int)Bullet_Position.Max];

	private Character m_Owner;
	private Char_Type m_OwnerType;
	private LayerMask m_TargetMask;
	private LayerMask m_WallMask;
	private AudioSource m_Audio;
	private Ray m_Ray;
	private RaycastHit m_RaycastHit;
	private float m_Speed = 1f;
	private float m_Range; // ���� �Ÿ�
	private float m_RangeMax; // �ִ� �Ÿ�
	private float m_Damage = 1f;
	private float m_Length; // �Ѿ��� �� ���� (�Ӹ�~����)
	private float m_HeadLength; // �Ѿ��� �պκ� ���� (�Ӹ�~�߽�)
	private WaitUntil m_WaitStoppedAudio;
	private bool m_Update;
	private bool m_Hit; // �Ѿ����� �������� �����ٸ� Ȱ��ȭ�Ѵ�.

	protected override void OnEnable()
	{
		base.OnEnable();

		m_Hit = false;

		AudioManager.AddEffectAudio(m_Audio);

		StartCoroutine(CheckOutRange());
	}

	protected override void OnDisable()
	{
		base.OnDisable();

		if (!m_Quit)
			AudioManager.RemoveEffectAudio(m_Audio);
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

		if (Physics.Raycast(m_Ray, out m_RaycastHit, m_Length, m_WallMask, QueryTriggerInteraction.Collide))
			Destroy();

		else if (Physics.Raycast(m_Ray, out m_RaycastHit, m_Length, m_TargetMask, QueryTriggerInteraction.Collide))
			OnHit();
	}

	private void OnHit()
	{
		IDamageable damageableObj = m_RaycastHit.transform.GetComponent<IDamageable>();

		if (damageableObj.IsDead() || m_Hit)
			return;

		m_Hit = true;

		damageableObj.TakeDamage(m_Damage, false);

		if (m_OwnerType == Char_Type.Player)
		{
			m_Audio.enabled = true;
			m_Audio.Play();

			StartCoroutine(PlayAudioAfterDestroy());
		}

		else
			Destroy();
	}

	private IEnumerator PlayAudioAfterDestroy()
	{
		yield return m_WaitStoppedAudio;

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

	public void SetSpeed(float speed)
	{
		m_Speed = speed;
	}

	public void SetDetailData(Character owner, Char_Type type)
	{
		m_Owner = owner;
		m_OwnerType = type;

		if (type == Char_Type.Player)
			m_TargetMask = StageManager.MonsterMask;

		else
			m_TargetMask = StageManager.PlayerMask;

		m_WallMask = StageManager.WallMask;

		m_Damage = m_Owner.Damage;
		// �ִ� �Ÿ��� �Ѿ��� �պκ��� ���༭ ���� �ִ� �Ÿ��� ���Ѵ�.
		m_RangeMax = m_Owner.Range - m_HeadLength;

		m_Update = true;
	}

	protected override void Awake()
	{
		base.Awake();

		m_Audio = GetComponent<AudioSource>();

		Utility.CheckEmpty(m_Audio, "m_Audio");

		m_Audio.volume = AudioManager.VolumeEffect;
		m_Audio.clip = AudioManager.EffectClip[(int)Audio_Effect.BulletHit];
		m_WaitStoppedAudio = new WaitUntil(() => !m_Audio.isPlaying);

		m_Ray = new Ray();

		Utility.CheckEmpty(m_BulletPos, "m_BulletPos");

		m_Length = Vector3.Distance(m_BulletPos[0].position, m_BulletPos[1].position);
		m_HeadLength = Vector3.Distance(transform.position, m_BulletPos[(int)Bullet_Position.Head].position);
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

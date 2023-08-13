using System.Collections;
using UnityEngine;
using UnityEngine.Pool;

public class Bullet : BaseScript
{
	[EnumArray(typeof(Bullet_Position))][ReadOnly(true)][SerializeField] private Transform[] m_BulletPos = new Transform[(int)Bullet_Position.Max];

	private Character_Type m_Owner;
	private LayerMask m_TargetMask;
	private LayerMask m_WallMask;
	private IObjectPool<Bullet> m_Pool;
	private AudioSource m_Audio;
	private AudioClip m_AudioClip;
	private Ray m_Ray;
	private RaycastHit m_Hit;
	private float m_Speed = 10f;
	private float m_Dist = 10f;
	private float m_Damage = 1f;
	private WaitForSeconds m_ClipLength;
	private bool m_Destroy;

	private void CheckCollision()
	{
		m_Ray.direction = transform.forward;

		foreach (Transform tr in m_BulletPos)
		{
			m_Ray.origin = tr.position;

			if (Physics.Raycast(m_Ray, out m_Hit, m_Dist, m_WallMask, QueryTriggerInteraction.Collide))
				Destroy();

			else if (Physics.Raycast(m_Ray, out m_Hit, m_Dist, m_TargetMask, QueryTriggerInteraction.Collide))
				OnHit();
		}
	}

	private void OnHit()
	{
		IDamageable damageableObj = m_Hit.transform.GetComponent<IDamageable>();

		if (damageableObj.IsDead())
			return;

		damageableObj.TakeDamage(m_Damage);

		if (m_Owner == Character_Type.Player)
		{
			m_Audio.enabled = true;
			m_Audio.PlayOneShot(m_AudioClip);

			StartCoroutine(PlayAudioAfterDestroy());
		}

		else
			Destroy();
	}

	private IEnumerator PlayAudioAfterDestroy()
	{
		yield return m_ClipLength;

		Destroy();
	}

	public void SetPool(IObjectPool<Bullet> pool)
	{
		m_Pool = pool;
	}

	public void Destroy()
	{
		if (!m_Destroy)
		{
			m_Destroy = true;

			if (m_Pool != null)
				m_Pool.Release(this);
		}
	}

	public void SetSpeed(float speed)
	{
		m_Speed = speed;
	}

	private IEnumerator CheckDist()
	{
		while (!m_Destroy)
		{
			m_Dist = m_deltaTime * m_Speed;

			yield return null;
		}
	}

	public void SetSpawnerInfo(Transform tr, Character_Type type, float dmg)
	{
		transform.position = tr.position;
		transform.rotation = tr.rotation;

		m_Damage = dmg;
		m_Owner = type;

		if (type == Character_Type.Player)
			m_TargetMask = StageManager.MonsterMask;

		else
			m_TargetMask = StageManager.PlayerMask;

		m_WallMask = StageManager.WallMask;
	}

	protected override void Awake()
	{
		base.Awake();

		m_Audio = GetComponent<AudioSource>();

#if UNITY_EDITOR
		if (m_Audio == null)
			Debug.LogError("if (m_Audio == null)");
#endif

		m_Audio.volume = AudioManager.VolumeEffect;
		m_AudioClip = AudioManager.EffectClip[(int)Effect_Audio.BulletHit];
		m_ClipLength = new WaitForSeconds(m_AudioClip.length);

		m_Ray = new Ray();

#if UNITY_EDITOR
		Utility.CheckEmpty(m_BulletPos, "m_BulletPos");
#endif
	}

	protected override void OnEnable()
	{
		base.OnEnable();

		m_Destroy = false;

		StartCoroutine(CheckDist());

		AudioManager.AddEffectAudio(m_Audio);
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();

		if (!m_Quit)
			AudioManager.RemoveEffectAudio(m_Audio);
	}

	protected override void BeforeUpdate()
	{
		if (!m_Destroy)
		{
			base.BeforeUpdate();

			CheckCollision();

			transform.Translate(Vector3.forward * m_Dist);
		}
	}
}

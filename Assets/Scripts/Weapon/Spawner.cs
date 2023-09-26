using System.Collections;
using UnityEngine;
using UnityEngine.Pool;

[RequireComponent(typeof(AudioSource))]
public class Spawner : BaseScript
{
	[ReadOnly(true)][SerializeField] private GameObject m_BulletPrefeb;

	private Character m_Owner;
	private float m_FireVelocity = 20f;
	private int m_BulletCount;
	private int m_BulletCountMax;
	private WaitForSeconds m_MultiShotDelay = new WaitForSeconds(.15f);
	private AudioSource m_Audio;
	private AudioClip m_AudioClip;
	private Character_Type m_Type;
	private IObjectPool<Bullet> m_Pool;
	private bool m_AttackProc;

	private void PlayAudio()
	{
		m_Audio.PlayOneShot(m_AudioClip);
	}

	public void SetSpawnerInfo(Character owner, Character_Type type)
	{
		m_Owner = owner;
		m_Type = type;

		m_BulletCountMax = m_Owner.BulletCount;
		m_AudioClip = m_Owner.AttackClip;

#if UNITY_EDITOR
		if (m_AudioClip == null)
			Debug.LogError("if (m_AudioClip == null)");
#endif
	}

	private IEnumerator AttackTimer()
	{
		if (m_Pool == null || m_AttackProc)
			yield break;

		m_BulletCount = 0;
		m_AttackProc = true;

		while (m_BulletCount < m_BulletCountMax)
		{
			++m_BulletCount;

			m_Pool.Get();

			PlayAudio();

			yield return m_MultiShotDelay;
		}

		m_AttackProc = false;
	}

	public void Attack()
	{
		if (m_BulletCountMax > 0)
			StartCoroutine(AttackTimer());
	}

	public void StageClear()
	{
		m_Pool.Clear();
	}

	protected override void Awake()
	{
		base.Awake();

		m_Audio = GetComponent<AudioSource>();
		m_Audio.volume = AudioManager.VolumeEffect;

		m_Pool = new ObjectPool<Bullet>(CreateBullet, OnGetBullet, OnReleaseBullet, OnDestroyBullet, maxSize: 20);
	}

	protected override void Start()
	{
		base.Start();

		StageManager.AddStageClear(StageClear);
	}

	protected Bullet CreateBullet()
	{
		Bullet bullet = Instantiate(m_BulletPrefeb).GetComponent<Bullet>();

		bullet.SetSpawnerInfo(transform, m_Type, m_Owner.Damage, m_Owner.Range);

		bullet.SetSpeed(m_FireVelocity);
		bullet.SetPool(m_Pool);

		return bullet;
	}

	protected void OnGetBullet(Bullet bullet)
	{
		bullet.gameObject.SetActive(true);
		bullet.SetSpawnerInfo(transform, m_Type, m_Owner.Damage, m_Owner.Range); // 새로 활성화 될 때마다 위치, 회전 정보를 갱신 시켜줘야 한다.
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

	protected override void OnEnable()
	{
		base.OnEnable();

		if (m_Quit)
			return;

		AudioManager.AddEffectAudio(m_Audio);
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();

		if (!m_Quit)
			AudioManager.RemoveEffectAudio(m_Audio);
	}
}

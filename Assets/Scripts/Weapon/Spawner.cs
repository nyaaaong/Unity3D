using System.Collections;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class Spawner : BaseScript
{
	private Character m_Owner;
	private Char_Type m_Type;
	private float m_FireVelocity = 20f;
	private int m_BulletCount;
	private int m_BulletCountMax;
	private WaitForSeconds m_MultiShotDelay = new WaitForSeconds(.15f);
	private AudioSource m_Audio;
	private AudioClip m_AudioClip;
	private bool m_AttackProc;

	private void CreateBullet()
	{
		GameObject prefeb;

		if (m_Type == Char_Type.Player)
			prefeb = StageManager.GetBulletPrefeb(Bullet_Type.Player);

		else
			prefeb = StageManager.GetBulletPrefeb(Bullet_Type.Monster);

		Bullet bullet = PoolManager.Get(prefeb, transform.position, transform.rotation).GetComponent<Bullet>();

		bullet.SetDetailData(m_Owner, m_Type);
		bullet.SetSpeed(m_FireVelocity);
	}

	private void PlayAudio()
	{
		m_Audio.PlayOneShot(m_AudioClip);
	}

	private IEnumerator AttackTimer()
	{
		if (m_AttackProc)
			yield break;

		m_BulletCount = 0;
		m_AttackProc = true;

		while (m_BulletCount < m_BulletCountMax)
		{
			++m_BulletCount;

			CreateBullet();

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

	protected override void Awake()
	{
		base.Awake();

		m_Audio = GetComponent<AudioSource>();
		m_Audio.volume = AudioManager.VolumeEffect;

		m_Owner = transform.parent.GetComponent<Character>();

		m_AudioClip = m_Owner.AttackClip;

		Utility.CheckEmpty(m_Owner, "m_Owner");
		Utility.CheckEmpty(m_AudioClip, "m_AudioClip");

		m_Type = m_Owner.Type;
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

	protected override void Update()
	{
		base.Update();

		if (m_Owner)
			m_BulletCountMax = m_Owner.BulletCount;
	}
}

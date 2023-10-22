using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class Spawner : BaseScript
{
	private IReadOnlyList<float> m_BulletAngleList;
	private Character m_Owner;
	private AudioClip[] m_AttackClip;
	private GameObject m_BulletPrefeb; // 나중에 캐릭별로 총알 유형이 많아지면 배열로도 생각해보기
	private WaitForSeconds m_WaitMultiAttackDelay = new WaitForSeconds(0.1f);
	private AudioSource m_Audio;
	private int m_AttackCount;
	private int m_AttackCountOwner;

	public event Action OnAttackEnd;

	protected override void Awake()
	{
		base.Awake();

		m_Audio = GetComponent<AudioSource>();
		m_Audio.volume = AudioManager.VolumeEffect;

		m_Owner = transform.parent.GetComponent<Character>();
		m_AttackClip = m_Owner.AttackClip;
		m_BulletAngleList = m_Owner.BulletAngleList;
		m_BulletPrefeb = m_Owner.Type == Char_Type.Player ? StageManager.GetBulletPrefeb(Bullet_Type.Player) : StageManager.GetBulletPrefeb(Bullet_Type.Monster);
	}

	private IEnumerator AttackTimer()
	{
		m_AttackCount = 0;

		while (m_AttackCount < m_AttackCountOwner)
		{
			++m_AttackCount;

			Attack();

			PlayAudio();

			yield return m_WaitMultiAttackDelay;
		}

		if (OnAttackEnd != null)
			OnAttackEnd();
	}

	private void Attack()
	{
		Bullet bullet;

		if (m_BulletAngleList.Count == 0)
		{
			bullet = PoolManager.Get(m_BulletPrefeb, transform.position, Quaternion.identity).GetComponent<Bullet>();
			bullet.SetDetailData(m_Owner, transform.forward);
		}

		else
		{
			foreach (float angle in m_BulletAngleList)
			{
				// info의 각도 정보를 이용해 현재 방향 기준으로 y축으로 회전
				bullet = PoolManager.Get(m_BulletPrefeb, transform.position, Quaternion.identity).GetComponent<Bullet>();
				bullet.SetDetailData(m_Owner, Quaternion.Euler(0f, angle, 0f) * transform.forward);
			}
		}
	}

	public void AttackEvent()
	{
		m_AttackCountOwner = m_Owner.BulletCount;

		if (m_AttackCountOwner == 0)
			Utility.LogError("m_AttackCountOwner가 0입니다!");

		StartCoroutine(AttackTimer());
	}

	private void PlayAudio()
	{
		m_Audio.PlayOneShot(m_AttackClip[UnityEngine.Random.Range(0, m_AttackClip.Length)]);
	}

	protected override void OnEnable()
	{
		base.OnEnable();

		AudioManager.AddEffectAudio(m_Audio);
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();

		AudioManager.RemoveEffectAudio(m_Audio);
	}
}

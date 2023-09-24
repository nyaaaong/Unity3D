
using System;
using UnityEngine;

[Serializable]
public class CharInfo
{
	[ReadOnly(true)][SerializeField] private float m_HP;
	[ReadOnly(true)][SerializeField] private float m_HPMax;
	[ReadOnly(true)][SerializeField] private float m_FireRateTime;
	[ReadOnly(true)][SerializeField] private float m_Damage;
	[ReadOnly][SerializeField] private float m_OrigDamage;
	[ReadOnly(true)][SerializeField] private float m_MoveSpeed;
	[ReadOnly(true)][SerializeField] private int m_BulletCount = 1;
	[ReadOnly(true)][SerializeField] private int m_Exp;

	private bool m_PowerUp;
	private bool m_NoHit;

	public float FireRateTime { get { return m_FireRateTime; } set { m_FireRateTime = value; } }
	public float HP { get { return m_HP; } set { m_HP = value; } }
	public float HPMax { get { return m_HPMax; } set { m_HPMax = value; } }
	public float MoveSpeed { get { return m_MoveSpeed; } }
	public float Damage { get { return m_Damage; } set { m_Damage = value; } }
	public int BulletCount { get { return m_BulletCount; } }
	public int Exp { set { m_Exp = value; } get { return m_Exp; } }
	public bool PowerUp
	{
		get { return m_PowerUp; }

		set
		{
			m_PowerUp = value;

			if (m_PowerUp)
			{
				m_OrigDamage = m_Damage;
				m_Damage = 9999f;
			}

			else
				m_Damage = m_OrigDamage;
		}
	}
	public bool NoHit { get { return m_NoHit; } set { m_NoHit = value; } }

	public void TakeDamage(float dmg, bool isCheat = false)
	{
		if (m_NoHit && !isCheat)
			return;

		m_HP -= dmg;

		if (m_HP < 0f)
			m_HP = 0f;
	}

	public void AddDamage(float dmg)
	{
		if (m_PowerUp)
			m_OrigDamage *= dmg;

		else
			m_Damage *= dmg;
	}

	public void AddFireRate(float rate)
	{
		m_FireRateTime /= rate;

		if (m_FireRateTime < 0.1f)
			m_FireRateTime = 0.1f;
	}

	public void Heal(float scale)
	{
		m_HP += m_HPMax * scale;

		if (m_HP > m_HPMax)
			m_HP = m_HPMax;
	}

	public void Speed(float scale)
	{
		m_MoveSpeed *= scale;
	}

	public void MultiShot()
	{
		++m_BulletCount;
	}

	public void Copy(CharInfo other)
	{
		m_HP = other.m_HP;
		m_HPMax = other.m_HPMax;
		m_FireRateTime = other.m_FireRateTime;
		m_Damage = other.m_Damage;
		m_OrigDamage = m_Damage < 9995f ? m_Damage : m_OrigDamage;
		m_MoveSpeed = other.m_MoveSpeed;
		m_BulletCount = other.m_BulletCount;
		m_Exp = other.m_Exp;
	}

	public CharInfo(CharInfo other = null)
	{
		if (other != null)
		{
			m_HP = other.m_HP;
			m_HPMax = other.m_HPMax;
			m_FireRateTime = other.m_FireRateTime;
			m_Damage = other.m_Damage;
			m_OrigDamage = m_Damage < 9995f ? m_Damage : m_OrigDamage;
			m_MoveSpeed = other.m_MoveSpeed;
			m_BulletCount = other.m_BulletCount;
			m_Exp = other.m_Exp;
		}
	}
}
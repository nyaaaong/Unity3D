
using System;
using UnityEngine;

[Serializable]
public class CharInfo
{
	[ReadOnly(true)][SerializeField] private string m_Name;

	[ReadOnly][SerializeField] private float m_HP;
	[ReadOnly(true)][SerializeField] private float m_HPMax;
	[ReadOnly(true)][SerializeField] private float m_FireRateTime;
	[ReadOnly(true)][SerializeField] private float m_FireRateMaxTime;
	[ReadOnly(true)][SerializeField] private float m_DamageMin;
	[ReadOnly(true)][SerializeField] private float m_Damage;
	[ReadOnly(true)][SerializeField] private float m_MoveSpeedMin;
	[ReadOnly(true)][SerializeField] private float m_MoveSpeed;
	[ReadOnly(true)][SerializeField] private float m_MoveSpeedMax;

	private float m_OrigDamage;
	private bool m_PowerUp;
	private bool m_NoHit;

	public float FireRateTime { get { return m_FireRateTime; } }
	public float HP { get { return m_HP; } }
	public float HPMax { get { return m_HPMax; } }
	public float MoveSpeed { get { return m_MoveSpeed; } }
	public float Damage { get { return m_Damage; } }
	public bool PowerUp
	{
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
	public bool NoHit { set { m_NoHit = value; } }

	public void TakeDamage(float dmg)
	{
		if (m_NoHit)
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

		if (m_Damage < m_DamageMin)
			m_Damage = m_DamageMin;
	}

	public void AddFireRate(float rate)
	{
		m_FireRateTime /= 1f + rate;

		if (m_FireRateTime > m_FireRateMaxTime)
			m_FireRateTime = m_FireRateMaxTime;
	}

	public void AddHPMax(float scale)
	{
		m_HPMax += m_HPMax * scale;
	}

	public void Heal(float scale)
	{
		m_HP += m_HPMax * scale;

		if (m_HP > m_HPMax)
			m_HP = m_HPMax;
	}

	public void Copy(CharInfo other)
	{
		m_HPMax = other.m_HPMax;
		m_HP = m_HPMax;
		m_FireRateTime = other.m_FireRateTime;
		m_FireRateMaxTime = other.m_FireRateMaxTime;
		m_Damage = other.m_Damage;
		m_OrigDamage = m_Damage;
		m_DamageMin = other.m_DamageMin;
		m_MoveSpeedMin = other.m_MoveSpeedMin;
		m_MoveSpeed = other.m_MoveSpeed;
		m_MoveSpeedMax = other.m_MoveSpeedMax;
	}

	public CharInfo(CharInfo other = null)
	{
		if (other != null)
		{
			m_HPMax = other.m_HPMax;
			m_HP = m_HPMax;
			m_FireRateTime = other.m_FireRateTime;
			m_FireRateMaxTime = other.m_FireRateMaxTime;
			m_Damage = other.m_Damage;
			m_OrigDamage = m_Damage;
			m_DamageMin = other.m_DamageMin;
			m_MoveSpeedMin = other.m_MoveSpeedMin;
			m_MoveSpeed = other.m_MoveSpeed;
			m_MoveSpeedMax = other.m_MoveSpeedMax;
		}
	}
}
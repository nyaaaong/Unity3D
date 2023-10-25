
using System;
using UnityEngine;

[Serializable]
public class CharData
{
	[ReadOnly(true)][SerializeField] private float m_HP;
	[ReadOnly(true)][SerializeField] private float m_HPMax;
	[ReadOnly(true)][SerializeField] private float m_FireRateTime;
	[ReadOnly(true)][SerializeField] private float m_FireSpeed = 20;
	[ReadOnly(true)][SerializeField] private float m_Damage;
	[ReadOnly(true)][SerializeField] private float m_Range;
	[ReadOnly][SerializeField] private float m_OrigDamage;
	[ReadOnly(true)][SerializeField] private float m_MoveSpeed;
	[ReadOnly(true)][SerializeField] private int m_BulletCount = 1;
	[ReadOnly(true)][SerializeField] private int m_Exp;

	private bool m_PowerUp;
	private bool m_NoHit;
	private int m_DynamicExp;
	private int m_Level = 1;

	public float MoveSpeed { get => m_MoveSpeed; set => m_MoveSpeed = value; }
	public int BulletCount => m_BulletCount;
	public float FireRateTime { get => m_FireRateTime; set => m_FireRateTime = value; }
	public float FireSpeed { get => m_FireSpeed; set => m_FireSpeed = value; }
	public float HP { get => m_HP; set => m_HP = value; }
	public float HPMax { get => m_HPMax; set => m_HPMax = value; }
	public float Damage { get => m_Damage; set => m_Damage = value; }
	public float Range { get => m_Range; set => m_Range = value; }
	public int Exp => m_Exp;
	public int DynamicExp { get => m_DynamicExp; set => m_DynamicExp = value; }
	public int Level { get => m_Level; set => m_Level = value; }
	public bool PowerUp
	{
		get => m_PowerUp;

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
	public bool NoHit { get => m_NoHit; set => m_NoHit = value; }

	public void TakeDamage(float dmg, bool isCheat = false)
	{
		if (m_NoHit && !isCheat)
			return;

		m_HP -= dmg;

		if (m_HP < 0f)
			m_HP = 0f;
	}

	public void AddDamage(float value)
	{
		if (m_PowerUp)
			m_OrigDamage *= value;

		else
			m_Damage *= value;
	}

	public void AddFireRate(float value)
	{
		if (value < 1f)
			value = 1f - value + 1f;

		m_FireRateTime *= value;

		float max = DataManager.PlayerFireRateTimeMax;

		if (m_FireRateTime > max)
			m_FireRateTime = max;
	}

	public void Heal(float value)
	{
		m_HP += m_HPMax * value;

		if (m_HP > m_HPMax)
			m_HP = m_HPMax;
	}

	public void AddMoveSpeed(float value)
	{
		m_MoveSpeed *= value;
	}

	public void MultiShot(int value)
	{
		m_BulletCount += value;
	}

	public void Copy(CharData other)
	{
		m_HP = other.m_HP;
		m_HPMax = other.m_HPMax;
		m_FireRateTime = other.m_FireRateTime;
		m_FireSpeed = other.m_FireSpeed;
		m_Damage = other.m_Damage;
		m_Range = other.m_Range;
		m_OrigDamage = m_Damage < 9995f ? m_Damage : m_OrigDamage;
		m_MoveSpeed = other.m_MoveSpeed;
		m_BulletCount = other.m_BulletCount;
		m_Exp = other.m_Exp;
		m_DynamicExp = m_Exp;
	}

	public CharData(CharData other = null)
	{
		if (other != null)
		{
			m_HP = other.m_HP;
			m_HPMax = other.m_HPMax;
			m_FireRateTime = other.m_FireRateTime;
			m_FireSpeed = other.m_FireSpeed;
			m_Damage = other.m_Damage;
			m_Range = other.m_Range;
			m_OrigDamage = m_Damage < 9995f ? m_Range : m_OrigDamage;
			m_MoveSpeed = other.m_MoveSpeed;
			m_BulletCount = other.m_BulletCount;
			m_Exp = other.m_Exp;
			m_DynamicExp = m_Exp;
		}
	}
}
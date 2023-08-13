
using System;
using UnityEngine;

[Serializable]
public class AbilityInfo
{
	[ReadOnly(true)][SerializeField] private float m_Damage = 1.3f;
	[ReadOnly(true)][SerializeField] private float m_FireRate = 1.15f;
	[ReadOnly(true)][SerializeField] private float m_Heal = 0.5f;
	[ReadOnly(true)][SerializeField] private float m_Speed = 1.2f;

	public float Damage { get { return m_Damage; } }
	public float FireRate { get { return m_FireRate; } }
	public float Heal { get { return m_Heal; } }
	public float Speed { get { return m_Speed; } }
}
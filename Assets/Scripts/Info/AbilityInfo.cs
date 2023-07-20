
using System;
using UnityEngine;

[Serializable]
public class AbilityInfo
{
	[ReadOnly(true)][SerializeField] private float m_Damage = 0.3f;
	[ReadOnly(true)][SerializeField] private float m_FireRate = 0.15f;
	[ReadOnly(true)][SerializeField] private float m_HPMax = 0.5f;

	public float Damage { get { return m_Damage; } }
	public float FireRate { get { return m_FireRate; } }
	public float HPMax { get { return m_HPMax; } }
}

using System;
using UnityEngine;

[Serializable]
public class AbilityData
{
	[ReadOnly(true)][SerializeField][EnumArray(typeof(Ability_Type))] private float[] m_Ability;

	public float GetAbility(int idx)
	{
		return m_Ability[idx];
	}

	public float GetAbility(Ability_Type type)
	{
		return m_Ability[(int)type];
	}

	public int GetAbilityToInt(int idx)
	{
		return Mathf.CeilToInt(m_Ability[idx]);
	}

	public int GetAbilityToInt(Ability_Type type)
	{
		return Mathf.CeilToInt(m_Ability[(int)type]);
	}
}

using System;
using UnityEngine;

[Serializable]
public class EnumArrayAttribute : PropertyAttribute
{
	private string[] m_Names;

	public string[] Names { get { return m_Names; } }

	public EnumArrayAttribute(Type enumType)
	{
		m_Names = Enum.GetNames(enumType);
	}
}
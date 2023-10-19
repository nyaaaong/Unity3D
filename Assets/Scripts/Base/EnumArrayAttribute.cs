
using System;
using UnityEngine;

[Serializable]
public class EnumArrayAttribute : PropertyAttribute
{
	private string[] m_Names;

	public string[] Names => m_Names;

	public EnumArrayAttribute(Type enumType)
	{
		if (!enumType.IsEnum)
		{
			Utility.LogError($"{enumType}는 Enum이 아닙니다!");
			return;
		}

		m_Names = Enum.GetNames(enumType);
	}
}
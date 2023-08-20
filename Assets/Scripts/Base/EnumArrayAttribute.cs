
using System;
using UnityEngine;

[Serializable]
public class EnumArrayAttribute : PropertyAttribute
{
	private string[] m_Names;

	public string[] Names { get { return m_Names; } }

	public EnumArrayAttribute(Type enumType)
	{
		if (enumType.IsEnum)
			m_Names = Enum.GetNames(enumType);

#if UNITY_EDITOR
		else
			Debug.LogError(enumType + "는 Enum이 아닙니다!");
#endif
	}
}
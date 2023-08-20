
using System;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(EnumArrayAttribute))]
public class EnumArrayDrawer : PropertyDrawer
{
	private EnumArrayAttribute m_EnumArray;
	private SerializedProperty m_Array;
	private string m_Path;
	private string m_Name;
	private string m_LastName;
	private int m_Length;
	private int m_Index;

	// 높이 설정
	public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
	{
		return EditorGUI.GetPropertyHeight(property, label);
	}

	// 인스팩터에 그려준다
	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		if (property == null)
			return;

		// EnumArray가 사용된 개체의 경로를 가져온다.
		// 예를 들어 m_Rect.Array.data[0] 의 형식을 가진 string을 출력한다.
		m_Path = property.propertyPath;

		// m_Path 문자열의 맨 처음부터 맨 뒤의 .의 바로 전까지를 넣어준다.
		// m_Rect.Array.data[0]를 예를 들면 m부터 y까지인 즉, m_Rect.Array를 말한다.
		// 이 경로를 이용하여 FindProperty를 이용해 SerializedProperty 타입인 Array를 반환한다.
		m_Array = property.serializedObject.FindProperty(m_Path.Substring(0, m_Path.LastIndexOf('.')));

		if (m_Array == null)
		{
			EditorGUI.LabelField(position, "배열에 EnumArray를 사용하세요");
			return;
		}

		m_EnumArray = attribute as EnumArrayAttribute;
		m_Length = m_EnumArray.Names.Length;
		m_LastName = m_EnumArray.Names[m_Length - 1];

		// 마지막 항목 이름이 Max인경우 그 부분을 제외시킨다
		if (m_LastName == "Max")
			--m_Length;

		// Enum 항목의 개수보다 작거나 클 경우 무조건 개수로 고정하게 한다.
		if (m_Array.arraySize != m_Length)
			m_Array.arraySize = m_Length;

		// m_Path를 이용하여 [ 이후 문자열을 받아온 후, 마지막 ]를 제거하여 최종적으로 인덱스가 몇 번인지 받아오게 한다.
		m_Index = Convert.ToInt32(m_Path.Substring(m_Path.IndexOf('[') + 1).Replace("]", ""));

		// 받아온 인덱스를 이용해 항목의 이름을 가져온다.
		m_Name = m_EnumArray.Names[m_Index];

		// Max일 때는 무시한다.
		if (m_Name == "Max")
			return;

		// 배열명을 enum의 index 항목의 이름으로 바꿔준다. 그리고 _는 공백으로 바꿔주게 한다.
		label.text = m_Name.Replace('_', ' ');

		// GUI 갱신
		EditorGUI.PropertyField(position, property, label, true);
	}
}
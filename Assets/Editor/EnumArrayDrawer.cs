
using System;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(EnumArrayAttribute))]
public class EnumArrayDrawer : PropertyDrawer
{
	private EnumArrayAttribute m_EnumArray;
	private SerializedProperty m_Array;
	private string m_Path;
	private string m_PathFront = "";
	private const string m_ErrorMsg = "는 올바른 배열이 아닙니다!";
	private int m_LastIndexDot;
	private string m_Name;
	private int m_Length;
	private int m_Index;

	private void InitArray(SerializedProperty property)
	{
		// 만약 m_Array를 처음 초기화 하는 경우, 예기치 않은 오류를 막기 위하여 예외 처리 후 m_Array에 올바른 값을 할당한다.
		if (m_Array == null)
		{
			// m_Path에 . 문자가 있는지 확인한다.
			m_LastIndexDot = m_Path.LastIndexOf('.');

			// 만약 .이 없다면 오류 출력
			if (m_LastIndexDot == -1)
			{
				Debug.LogError($"{m_Path}{m_ErrorMsg}");
				return;
			}

			// 맨 처음 ~ . 문자 바로 전까지를 구해준다. 예를 들어 34번째 줄에서 구해준 m_Path가 m_Rect.Array.data[0] 라고 한다면, m_PathFront는 m_Rect.Array가 될 것이다.
			m_PathFront = m_Path.Substring(0, m_LastIndexDot);

			// 드물겠지만 만약 m_PathFront가 비어있는 경우 오류 출력
			if (m_PathFront == "")
			{
				Debug.LogError($"{m_Path}{m_ErrorMsg}");
				return;
			}

			else
			{
				// 구해준 m_PathFront를 이용하여 m_Array에 Property를 넣어준다.
				m_Array = property.serializedObject.FindProperty(m_PathFront);

				// 만약 Property를 못 찾았거나 찾았는데 배열이 아닌 경우 오류 출력
				if (m_Array == null || !m_Array.isArray)
				{
					Debug.LogError($"{m_PathFront}{m_ErrorMsg}");
					return;
				}
			}
		}
	}

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

		// m_Array 초기화
		InitArray(property);

		// 현재 Attribute를 EnumArrayAttribute 클래스로 형변환 후 m_EnumArray에 넣어준다.
		m_EnumArray = attribute as EnumArrayAttribute;

		// Enum문의 총 개수를 가져오고
		m_Length = m_EnumArray.Names.Length;

		// Enum문의 맨 마지막 항목 이름이 Max인경우 그 부분을 제외시킨다
		if (m_EnumArray.Names[m_Length - 1] == "Max")
			--m_Length;

		// Enum 항목의 개수보다 작거나 클 경우 무조건 개수로 고정하게 한다.
		if (m_Array.arraySize != m_Length)
			m_Array.arraySize = m_Length;

		// m_Path를 이용하여 [ 이후 문자열을 받아온 후, 마지막 ]를 제거하여 최종적으로 인덱스가 몇 번인지 받아오게 한다.
		m_Index = Convert.ToInt32(m_Path.Substring(m_Path.IndexOf('[') + 1).Replace("]", ""));

		// 받아온 인덱스를 이용해 항목의 이름을 가져온다.
		m_Name = m_EnumArray.Names[m_Index];

		// 배열명을 enum의 index 항목의 이름으로 바꿔준다. 그리고 _는 공백으로 바꿔주게 한다.
		label.text = m_Name.Replace('_', ' ');

		// GUI 갱신
		EditorGUI.PropertyField(position, property, label, true);
	}
}
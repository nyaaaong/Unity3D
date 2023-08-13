using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MapGenerator))]
public class MapEditor : Editor
{
	private bool m_Init;

	public override void OnInspectorGUI()
	{
		MapGenerator map = target as MapGenerator;

		DrawDefaultInspector();

		m_Init = map.Init();

		if (!m_Init)
			Debug.LogError("맵 초기화 실패");

		if (GUILayout.Button("맵 생성 테스트"))
			map.Generator();
	}
}

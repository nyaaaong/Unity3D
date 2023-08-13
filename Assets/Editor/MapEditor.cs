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
			Debug.LogError("�� �ʱ�ȭ ����");

		if (GUILayout.Button("�� ���� �׽�Ʈ"))
			map.Generator();
	}
}

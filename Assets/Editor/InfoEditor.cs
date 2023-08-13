
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(InfoManager))]
public class InfoEditor : Editor
{
	public override void OnInspectorGUI()
	{
		InfoManager info = target as InfoManager;

		DrawDefaultInspector();

		if (GUILayout.Button("Save"))
			info.SaveData();

		if (GUILayout.Button("Load"))
			info.LoadData();

	}
}
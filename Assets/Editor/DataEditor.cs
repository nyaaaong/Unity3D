
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DataManager))]
public class DataEditor : Editor
{
	public override void OnInspectorGUI()
	{
		DataManager data = target as DataManager;

		DrawDefaultInspector();

		if (GUILayout.Button("Save"))
			data.SaveData();

		if (GUILayout.Button("Load"))
			data.LoadData();

	}
}
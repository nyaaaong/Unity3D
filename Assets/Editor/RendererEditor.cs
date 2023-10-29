
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(RendererManager))]
public class RendererEditor : Editor
{
	private SerializedProperty m_Preview;
	private SerializedProperty m_HitEffectColor;
	private GUIContent m_ColorContent;
	private bool m_HasDummy;
	private const string m_ScriptName = "m_Script";
	private const string m_ColorName = "m_HitEffectColor";

	private void OnEnable()
	{
		if (Application.isPlaying)
		{
			RendererManager.ShowDummy(false);
			return;
		}

		RendererManager.Init();

		if (m_Preview == null)
			m_Preview = serializedObject.FindProperty("m_Preview");

		if (m_HitEffectColor == null)
			m_HitEffectColor = serializedObject.FindProperty(m_ColorName);

		if (m_ColorContent == null)
			m_ColorContent = new GUIContent("Hit Effect Color");
	}

	public override void OnInspectorGUI()
	{
		if (Application.isPlaying)
			return;

		// 최신 데이터 받아오기
		serializedObject.Update();

		// 인스펙터에서 기본으로 생성되는 Script, SerializeField로 지정한 Color을 제외한 나머지를 그린다.
		DrawPropertiesExcluding(serializedObject, m_ScriptName, m_ColorName);

		m_HasDummy = RendererManager.HasDummy();

		// 알파를 제외한 Color 지정 창을 그린다
		m_HitEffectColor.colorValue = EditorGUILayout.ColorField(m_ColorContent, m_HitEffectColor.colorValue, true, false, true);

		// 만약 플레이 모드인 경우 더미가 게임 중에 생성되면 안되기 때문에 bool을 false로 둔다
		if (Application.isPlaying)
			m_Preview.boolValue = false;

		// 더미 오브젝트가 존재하는 경우 더미를 보여줄지 말지 결정하고, 색상도 칠해준다.
		if (m_HasDummy)
		{
			RendererManager.ShowDummy(m_Preview.boolValue);
			RendererManager.SetColorDummy();
		}

		// 최신 데이터 적용
		serializedObject.ApplyModifiedProperties();
	}
}
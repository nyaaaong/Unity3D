
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

		// �ֽ� ������ �޾ƿ���
		serializedObject.Update();

		// �ν����Ϳ��� �⺻���� �����Ǵ� Script, SerializeField�� ������ Color�� ������ �������� �׸���.
		DrawPropertiesExcluding(serializedObject, m_ScriptName, m_ColorName);

		m_HasDummy = RendererManager.HasDummy();

		// ���ĸ� ������ Color ���� â�� �׸���
		m_HitEffectColor.colorValue = EditorGUILayout.ColorField(m_ColorContent, m_HitEffectColor.colorValue, true, false, true);

		// ���� �÷��� ����� ��� ���̰� ���� �߿� �����Ǹ� �ȵǱ� ������ bool�� false�� �д�
		if (Application.isPlaying)
			m_Preview.boolValue = false;

		// ���� ������Ʈ�� �����ϴ� ��� ���̸� �������� ���� �����ϰ�, ���� ĥ���ش�.
		if (m_HasDummy)
		{
			RendererManager.ShowDummy(m_Preview.boolValue);
			RendererManager.SetColorDummy();
		}

		// �ֽ� ������ ����
		serializedObject.ApplyModifiedProperties();
	}
}
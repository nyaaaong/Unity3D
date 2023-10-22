using UnityEditor;
using UnityEngine;

namespace GabrielBissonnette.SAD
{
	[CustomEditor(typeof(MainMenuManager))]
	public class MainMenuEditor : Editor
	{
		#region SerializedProperty
		private SerializedProperty mainColor;
		#endregion

		#region Private
		private SerializedObject soTarget;
		private MainMenuManager mainMenuManager;
		private Texture2D texturePanel1;
		#endregion

		private void OnEnable()
		{
			mainMenuManager = (MainMenuManager)target;
			soTarget = new SerializedObject(target);

			texturePanel1 = Resources.Load<Texture2D>("img/InspectorBanner1");

			mainColor = soTarget.FindProperty("mainColor");
		}

		public override void OnInspectorGUI()
		{
			// Image
			EditorGUI.DrawPreviewTexture(new Rect(18, 10, 520, 80), texturePanel1, mat: null, ScaleMode.ScaleToFit);
			EditorGUILayout.Space(91);

			base.OnInspectorGUI();
			mainMenuManager.UIEditorUpdate();
		}

		private void Start()
		{

		}

		private void Update()
		{

		}
	}
}

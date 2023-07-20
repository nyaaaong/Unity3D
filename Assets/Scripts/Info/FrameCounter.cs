using UnityEngine;

public class FrameCounter : BaseScript
{
	[SerializeField, Range(1, 100)] private int m_FontSize = 25;
	[SerializeField] private Color m_Color = Color.green;

	private bool m_Show = true;
	private float m_Time;

	protected override void Awake()
	{
		base.Awake();

		Application.targetFrameRate = 75;
	}

	protected override void BeforeUpdate()
	{
		base.BeforeUpdate();

		m_Time += (m_unscaleDeltaTime - m_Time) * 0.1f;

		if (Input.GetKeyDown(KeyCode.F1))
			m_Show = !m_Show;
	}

	private void OnGUI()
	{
		if (m_Show)
		{
			GUIStyle newStyle = new GUIStyle();

			Rect rc = new Rect(30f, 30f, Screen.width, Screen.height);
			newStyle.alignment = TextAnchor.UpperLeft;
			newStyle.fontSize = m_FontSize;
			newStyle.normal.textColor = m_Color;

			float fps = 1f / m_Time;
			string text = string.Format("{0:0.} FPS", fps);

			GUI.Label(rc, text, newStyle);
		}
	}
}

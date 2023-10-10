using UnityEngine;

public class BaseScript : MonoBehaviour
{
	public class Vertex
	{
		private float m_Left;
		private float m_Top;
		private float m_Right;
		private float m_Bottom;

		public float Left { get => m_Left; set => m_Left = value; }
		public float Top { get => m_Top; set => m_Top = value; }
		public float Right { get => m_Right; set => m_Right = value; }
		public float Bottom { get => m_Bottom; set => m_Bottom = value; }

		public Vertex(Transform trs)
		{
			Vector3 worldLTPos = trs.TransformPoint(-0.5f, 0f, 0.5f);

			m_Left = worldLTPos.x;
			m_Right = -worldLTPos.x;
			m_Top = worldLTPos.z;
			m_Bottom = -worldLTPos.z;
		}

		public Vertex(Vertex v)
		{
			m_Left = v.m_Left;
			m_Top = v.m_Top;
			m_Right = v.m_Right;
			m_Bottom = v.m_Bottom;
		}
	}

	protected static bool m_Quit;

	protected virtual void Awake() { }

	protected virtual void OnEnable() { }

	protected virtual void Start() { }

	protected virtual void FixedUpdate() { }

	protected virtual void Update() { }

	protected virtual void LateUpdate() { }

	protected virtual void OnDisable() { }

	private void OnApplicationQuit()
	{
		m_Quit = true;
	}

	protected virtual void OnDestroy() { }
}
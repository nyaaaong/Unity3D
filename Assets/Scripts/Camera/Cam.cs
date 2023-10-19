using UnityEngine;

public class Cam : BaseScript
{
	[SerializeField] private float m_PlayerOffsetY = 4f;
	[SerializeField] private float m_TopOffset = 0.92f;
	[SerializeField] private float m_BottomOffset = 8.9f;

	private Vector3 m_CamPos;

	private float m_CamXHalf;
	private float m_CamYHalf;

	private Vertex m_FieldVertex;

	protected override void Awake()
	{
		base.Awake();

		Camera cam = GetComponent<Camera>();

		// aspect는 width / height이므로 height을 곱해줘서 width를 구한다
		m_CamXHalf = cam.aspect * cam.orthographicSize;
		m_CamYHalf = cam.orthographicSize;
	}

	protected override void Start()
	{
		base.Start();

		CreateFieldVertex();
	}

	private void CreateFieldVertex()
	{
		if (m_FieldVertex == null)
		{
			m_FieldVertex = new Vertex(StageManager.Map.MapVertex);
			m_FieldVertex.Left += m_CamXHalf;
			m_FieldVertex.Right -= m_CamXHalf;
			m_FieldVertex.Top -= m_CamYHalf;
			m_FieldVertex.Bottom += m_CamYHalf;
		}
	}

	protected override void LateUpdate()
	{
		base.LateUpdate();

		if (StageManager.Player)
		{
			if (!StageManager.IsPlayerDeath &&
				!StageManager.IsStageClear)
			{
				m_CamPos = StageManager.Player.Pos;
				m_CamPos.y = transform.position.y;
				m_CamPos.z -= m_PlayerOffsetY;

				CreateFieldVertex();

				m_CamPos.x = Mathf.Clamp(m_CamPos.x, m_FieldVertex.Left, m_FieldVertex.Right);
				m_CamPos.z = Mathf.Clamp(m_CamPos.z, m_FieldVertex.Top - m_BottomOffset, m_FieldVertex.Bottom - m_TopOffset);

				transform.position = m_CamPos;
			}
		}
	}
}

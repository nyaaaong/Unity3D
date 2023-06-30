using UnityEngine;

public class Cam : BaseScript
{
	[SerializeField] private float m_ZoomScale = 5f;

	private Vector3 m_Pos;

	protected override void Awake()
	{
		base.Awake();

		m_Pos = transform.position;

		Vector2Int mapSize = StageManager.MapSize;

		int maxSize = mapSize.x > mapSize.y ? mapSize.x : mapSize.y;

		m_Pos.y *= maxSize / m_ZoomScale;

		transform.position = m_Pos;
	}

	protected override void FixedUpdate()
	{
		base.FixedUpdate();

		if (!StageManager.IsPlayerDeath && 
			!StageManager.IsStageClear)
		{
			m_Pos = transform.position;
			m_Pos.z += StageManager.Player.Pos.z - m_Pos.z;

			transform.position = m_Pos;
		}
	}
}

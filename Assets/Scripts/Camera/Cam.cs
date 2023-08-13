using UnityEngine;

public class Cam : BaseScript
{
	[SerializeField] private float m_SmoothSpeed = .2f;

	private Vector3 m_PlayerPos;
	private Vector3 m_ResultPos;

	protected override void LateUpdate()
	{
		base.LateUpdate();

		if (StageManager.Player)
		{
			if (!StageManager.IsPlayerDeath &&
				!StageManager.IsStageClear)
			{
				m_PlayerPos = StageManager.Player.Pos;
				m_PlayerPos.y = transform.position.y;

				m_ResultPos = Vector3.Lerp(transform.position, m_PlayerPos, m_SmoothSpeed);

				transform.position = m_ResultPos;
			}
		}
	}
}

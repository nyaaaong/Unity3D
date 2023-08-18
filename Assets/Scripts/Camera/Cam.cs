using UnityEngine;

public class Cam : BaseScript
{
	private Vector3 m_PlayerPos;

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

				transform.position = m_PlayerPos;
			}
		}
	}
}

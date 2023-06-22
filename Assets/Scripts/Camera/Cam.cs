using UnityEngine;

public class Cam : BaseScript
{
	private Vector3 m_Pos;

	protected override void FixedUpdate()
	{
		base.FixedUpdate();

		if (!StageManager.IsPlayerDeath && 
			!StageManager.IsStageClear)
		{
			m_Pos = transform.position;
			m_Pos.z += StageManager.GetPlayer.Pos.z - m_Pos.z;

			transform.position = m_Pos;
		}
	}
}

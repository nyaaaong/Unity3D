using UnityEngine;

[RequireComponent (typeof(PlayerController))]
public class Player : Character
{
	private Vector3 m_Dir;
	private Vector3 m_Velocity;
	private PlayerController m_PlayerController;

	protected override void Awake()
	{
		m_PlayerController = GetComponent<PlayerController>();
	}

	protected override void FixedUpdate()
	{
		base.FixedUpdate();

		// 방향
		m_PlayerController.Rotation(m_RotSpeed, m_Dir);
	}

	protected override void BeforeUpdate()
	{
		// 이동
		m_Dir = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
		m_Velocity = m_Dir * m_MoveSpeed;

		m_PlayerController.Move(m_Velocity);
		m_PlayerController.Shoot();
	}
}

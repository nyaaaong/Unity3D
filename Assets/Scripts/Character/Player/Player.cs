using UnityEngine;

[RequireComponent (typeof(PlayerController))]
[RequireComponent (typeof(GunController))]
public class Player : Character
{
	private Vector3 m_Input;
	private Vector3 m_Velocity;
	private PlayerController m_Controller;
	private GunController m_GunController;
	private bool m_Move = false;

	protected override void Awake()
	{
		m_Controller = GetComponent<PlayerController>();
		m_GunController = GetComponent<GunController>();
	}

	protected override void FixedUpdate()
	{
		base.FixedUpdate();

		// ����
		m_Controller.Forward(m_RotSpeed, m_Input);
	}

	protected override void BeforeUpdate()
	{
		// �̵�
		m_Input = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
		m_Velocity = m_Input.normalized * m_MoveSpeed;

		m_Controller.Move(m_Velocity);

		m_Move = m_Velocity != Vector3.zero ? true : false;

		// ����
		if (!m_Move)
			m_GunController.Shoot();
	}
}

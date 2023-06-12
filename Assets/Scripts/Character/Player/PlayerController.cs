using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : BaseScript
{
	private Rigidbody m_Rig;
	private Vector3 m_Velocity;

	public void Forward(float rotSpeed, Vector3 dir)
	{
		if (dir != Vector3.zero)
			transform.forward = Vector3.Lerp(transform.forward, dir, rotSpeed * m_fixedDeltaTime);
	}

	public void Move(Vector3 velocity)
	{
		m_Velocity = velocity;
	}

	protected override void Awake()
	{
		m_Rig = GetComponent<Rigidbody>();
	}

	protected override void FixedUpdate()
	{
		base.FixedUpdate();

		m_Rig.MovePosition(m_Rig.position + m_Velocity * m_fixedDeltaTime);
	}
}

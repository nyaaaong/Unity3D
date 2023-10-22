using UnityEngine;

namespace DevionGames
{
	public class MoveForward : MonoBehaviour
	{

		[SerializeField]
		private float m_Speed = 5f;
		[SerializeField]
		private bool m_LookAtCameraForward = true;
		[SerializeField]
		private bool m_AutoDestruct = true;
		[SerializeField]
		private float m_DestructDelay = 10f;
		private Rigidbody m_Rigidbody;

		private void Start()
		{
			m_Rigidbody = GetComponent<Rigidbody>();

			transform.parent = null;
			if (m_AutoDestruct)
				Destroy(gameObject, m_DestructDelay);

			if (m_LookAtCameraForward)
			{
				Vector3 forward = Camera.main.transform.forward;
				if (forward.sqrMagnitude != 0.0f)
				{
					forward.Normalize();
					transform.LookAt(transform.position + forward);
				}
			}
		}

		private void FixedUpdate()
		{
			m_Rigidbody.velocity = transform.forward * m_Speed;
		}
	}
}
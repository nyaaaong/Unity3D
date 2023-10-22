using UnityEngine;

namespace DevionGames
{
	public class SimpleMinimap : MonoBehaviour
	{
		[SerializeField]
		private string m_PlayerTag = "Player";
		[SerializeField]
		private bool m_RotateWithPlayer = false;

		private Transform m_CameraTransform;
		private Transform m_PlayerTransform;

		private void Start()
		{
			m_CameraTransform = transform;
			m_PlayerTransform = GameObject.FindGameObjectWithTag(m_PlayerTag).transform;
		}

		private void Update()
		{
			Vector3 position = m_PlayerTransform.position;
			position.y = m_CameraTransform.position.y;
			m_CameraTransform.position = position;
			if (m_RotateWithPlayer)
			{
				m_CameraTransform.rotation = m_PlayerTransform.rotation;
			}
		}
	}
}
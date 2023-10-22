using UnityEngine;

namespace DevionGames
{
	public class PlayerData
	{
		private readonly string m_Tag = "Player";

		public PlayerData(string tag)
		{
			m_Tag = tag;
		}

		private GameObject m_GameObject;
		public GameObject gameObject
		{
			get
			{
				if (m_GameObject == null)
				{
					GameObject[] players = GameObject.FindGameObjectsWithTag(m_Tag);
					for (int i = 0; i < players.Length; i++)
					{
						GameObject player = players[i];

						m_GameObject = player;
					}
				}

				return m_GameObject;
			}
		}

		private readonly Transform m_Transform;
		public Transform transform
		{
			get
			{
				if (gameObject != null)
				{
					return gameObject.transform;
				}

				return null;
			}
		}

		private Collider m_Collider;
		public Collider collider
		{
			get
			{
				if (m_Collider == null && gameObject != null)
				{
					m_Collider = gameObject.GetComponent<Collider>();
				}

				return m_Collider;
			}
		}

		private Collider2D m_Collider2D;
		public Collider2D collider2D
		{
			get
			{
				if (m_Collider2D == null && gameObject != null)
				{
					m_Collider2D = gameObject.GetComponent<Collider2D>();
				}

				return m_Collider2D;
			}
		}

		private Animator m_Animator;
		public Animator animator
		{
			get
			{
				if (m_Animator == null && gameObject != null)
				{
					m_Animator = gameObject.GetComponentInChildren<Animator>();
				}

				return m_Animator;
			}
		}

		public Bounds bounds
		{
			get
			{
				return UnityTools.GetBounds(gameObject);
			}
		}
	}
}
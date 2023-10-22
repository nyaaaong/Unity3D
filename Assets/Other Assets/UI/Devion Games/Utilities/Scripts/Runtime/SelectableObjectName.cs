using UnityEngine;
using UnityEngine.UI;

namespace DevionGames
{
	public class SelectableObjectName : MonoBehaviour
	{
		[SerializeField]
		private Text m_ObjectName = null;

		private void Start()
		{
			if (m_ObjectName == null)
				m_ObjectName = GetComponent<Text>();
		}

		private void Update()
		{
			if (SelectableObject.current == null)
				return;

			string current = SelectableObject.current.name;
			if (m_ObjectName != null && !current.Equals(m_ObjectName.text))
				m_ObjectName.text = current;
		}
	}
}
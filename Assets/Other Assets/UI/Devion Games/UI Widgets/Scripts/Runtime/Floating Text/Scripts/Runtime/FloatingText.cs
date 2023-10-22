using UnityEngine;
using UnityEngine.UI;

namespace DevionGames.UIWidgets
{
	public class FloatingText : MonoBehaviour
	{
		private Transform m_Target;
		private Vector3 m_Offset;
		private Text m_Text;

		private void Awake()
		{
			m_Text = GetComponent<Text>();
		}

		private void LateUpdate()
		{

			Vector3 pos = UnityTools.GetBounds(m_Target.gameObject).center + m_Offset;
			Vector3 screenPos = Camera.main.WorldToScreenPoint(pos);
			m_Text.enabled = screenPos.x > 0 && screenPos.x < Camera.main.pixelWidth && screenPos.y > 0 && screenPos.y < Camera.main.pixelHeight && screenPos.z > 0;
			transform.position = Camera.main.WorldToScreenPoint(UnityTools.GetBounds(m_Target.gameObject).center + m_Offset);
		}

		public void SetText(Transform target, string text, Color color, Vector3 offset)
		{
			m_Target = target;
			m_Offset = offset;
			Text component = GetComponent<Text>();
			component.text = text;
			component.color = color;

		}
	}
}
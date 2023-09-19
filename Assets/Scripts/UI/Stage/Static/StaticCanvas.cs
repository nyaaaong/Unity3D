using UnityEngine;

public class StaticCanvas : BaseScript
{
	[ReadOnly(true)][SerializeField] private StaticTextUI m_StageText;
	[ReadOnly(true)][SerializeField] private StaticTextUI m_ScoreText;
	[ReadOnly(true)][SerializeField] private FloatingJoystick m_Joystick;

	public FloatingJoystick Joystick { get { return m_Joystick; } }

	public bool ActiveJoystick { set { m_Joystick.gameObject.SetActive(value); } }

	protected override void Awake()
	{
		base.Awake();

#if UNITY_EDITOR
		if (!m_Joystick)
			Debug.LogError("if (!m_Joystick)");
#endif

		if (!m_Joystick.gameObject.activeSelf)
			m_Joystick.gameObject.SetActive(true);
	}
}

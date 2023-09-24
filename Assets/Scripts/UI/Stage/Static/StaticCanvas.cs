using UnityEngine;

public class StaticCanvas : BaseScript
{
	[ReadOnly(true)][SerializeField] private FloatingJoystick m_Joystick;
	[ReadOnly(true)][SerializeField] private StaticExp m_ExpBar;

	public FloatingJoystick Joystick => m_Joystick;
	public StaticExp ExpBar => m_ExpBar;

	public bool ActiveJoystick { set => m_Joystick.gameObject.SetActive(value); }
	public float AddExp { set => m_ExpBar.AddExp(value); }

	public void UpdateExp()
	{
		m_ExpBar.UpdateExp();
	}

	public void ResetExp()
	{
		m_ExpBar.ResetExp();
	}

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

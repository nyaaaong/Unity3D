using UnityEngine;

public class StaticCanvas : BaseScript
{
	[ReadOnly(true)][SerializeField] private FloatingJoystick m_Joystick;
	[ReadOnly(true)][SerializeField] private StaticExp m_ExpBar;
	[ReadOnly(true)][SerializeField] private HPBar m_BossHPBar;

	public FloatingJoystick Joystick => m_Joystick;
	public StaticExp ExpBar => m_ExpBar;

	public bool ActiveJoystick { set => m_Joystick.gameObject.SetActive(value); }
	public float AddExp { set => m_ExpBar.AddExp(value); }
	public bool IsBarUpdate => m_ExpBar.IsUpdate;
	public bool NeedShowAbility => m_ExpBar.NeedShowAbility;

	public void ResetUI()
	{
		m_BossHPBar.gameObject.SetActive(false);
		m_ExpBar.gameObject.SetActive(true);
	}

	public void SetBossHPOwner(Character owner)
	{
		ShowBossHP();
		m_BossHPBar.SetOwner(owner, ResetUI);
	}

	private void ShowBossHP()
	{
		m_BossHPBar.gameObject.SetActive(true);
		m_ExpBar.gameObject.SetActive(false);
	}

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

		Utility.CheckEmpty(m_Joystick, "m_Joystick");
		Utility.CheckEmpty(m_ExpBar, "m_ExpBar");
		Utility.CheckEmpty(m_BossHPBar, "m_BossHPBar");

		if (!m_Joystick.gameObject.activeSelf)
			m_Joystick.gameObject.SetActive(true);
	}
}

using UnityEngine;
using UnityEngine.UI;

public class AbilityUI : BaseScript
{
	[ReadOnly(true)][SerializeField] private Ability_Type m_Type = Ability_Type.Max;
	private Text m_Text;

	protected override void OnEnable()
	{
		base.OnEnable();

		m_Text.text = string.Format(m_Text.text, DataManager.GetBuffPercent(m_Type, true));
	}

	public void OnClickEvent()
	{
		switch (m_Type)
		{
			case Ability_Type.Damage:
				DataManager.AddDamage();
				break;
			case Ability_Type.FireRate:
				DataManager.AddFireRate();
				break;
			case Ability_Type.Heal:
				DataManager.Heal();
				break;
			case Ability_Type.MoveSpeed:
				DataManager.AddMoveSpeed();
				break;
			case Ability_Type.MultiShot:
				DataManager.MultiShot();
				break;
		}
	}

	protected override void Awake()
	{
		base.Awake();

		m_Text = GetComponentInChildren<Text>();
	}
}
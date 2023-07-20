using UnityEngine;
using UnityEngine.UI;

public class Ability : BaseScript
{
	[ReadOnly(true)][SerializeField] private Ability_Type m_Type = Ability_Type.Max;

	private Button m_Button;

	private void OnButtonClick()
	{
		switch (m_Type)
		{
			case Ability_Type.Damage:
				InfoManager.AddDamage();
				break;
			case Ability_Type.FireRate:
				InfoManager.AddFireRate();
				break;
			case Ability_Type.HPMax:
				InfoManager.AddHPMax();
				break;
		}

		UIManager.HideAbility();
		StageManager.NextStage();
	}

	protected override void Awake()
	{
		base.Awake();

		if (m_Type == Ability_Type.Max)
			Debug.LogError("if (m_Type == Ability_Type.Max)");

		m_Button = GetComponent<Button>();
		m_Button.onClick.AddListener(OnButtonClick);
	}
}
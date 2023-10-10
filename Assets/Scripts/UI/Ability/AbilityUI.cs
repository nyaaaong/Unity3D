using UnityEngine;

public class AbilityUI : BaseScript
{
	[ReadOnly(true)][SerializeField] private Ability_Type m_Type = Ability_Type.Max;

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
			case Ability_Type.Speed:
				DataManager.Speed();
				break;
			case Ability_Type.MultiShot:
				DataManager.MultiShot();
				break;
		}
	}

	protected override void Awake()
	{
		base.Awake();

		Utility.CheckEmpty(transform.parent, "transform.parent");
	}
}
using UnityEngine;

public class AbilityUI : BaseScript
{
	[ReadOnly(true)][SerializeField] private Ability_Type m_Type = Ability_Type.Max;

	public void OnClickEvent()
	{
		switch (m_Type)
		{
			case Ability_Type.Damage:
				InfoManager.AddDamage();
				break;
			case Ability_Type.FireRate:
				InfoManager.AddFireRate();
				break;
			case Ability_Type.Heal:
				InfoManager.Heal();
				break;
			case Ability_Type.Speed:
				InfoManager.Speed();
				break;
			case Ability_Type.MultiShot:
				InfoManager.MultiShot();
				break;
		}
	}

	protected override void Awake()
	{
		base.Awake();

#if UNITY_EDITOR
		if (m_Type == Ability_Type.Max)
			Debug.LogError("if (m_Type == Ability_Type.Max)");

		if (transform.parent == null)
			Debug.LogError("if (transform.parent == null)");
#endif
	}
}
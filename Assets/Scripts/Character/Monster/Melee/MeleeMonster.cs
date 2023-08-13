public class MeleeMonster : Monster
{
	protected override void Awake()
	{
		m_CharInfo = InfoManager.Clone(Character_Type.Melee);

		base.Awake();
	}
}

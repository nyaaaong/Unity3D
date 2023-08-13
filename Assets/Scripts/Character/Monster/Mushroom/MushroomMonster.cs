public class MushroomMonster : RangeBase
{
	protected override void Awake()
	{
		m_CharInfo = InfoManager.Clone(Character_Type.Mushroom);

		base.Awake();

		m_Spawner.SetSpawnerInfo(this, Character_Type.Mushroom);
	}
}

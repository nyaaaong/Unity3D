public class RangeMonster : RangeBase
{
	protected override void Awake()
	{
		m_CharInfo = InfoManager.Clone(Character_Type.Range);

		base.Awake();

		m_Spawner.SetSpawnerInfo(this, Character_Type.Range);
	}
}

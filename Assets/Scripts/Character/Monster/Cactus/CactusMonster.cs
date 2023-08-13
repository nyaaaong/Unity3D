public class CactusMonster : Monster
{
	protected override void Awake()
	{
		m_CharInfo = InfoManager.Clone(Character_Type.Cactus);

		base.Awake();
	}
}

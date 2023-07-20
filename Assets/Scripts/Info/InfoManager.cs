using UnityEngine;

public class InfoManager : Singleton<InfoManager>
{
	[ReadOnly(true)][SerializeField] private CharInfo[] m_CharacterInfoBase = new CharInfo[(int)Character_Type.Max];
	[ReadOnly(true)][SerializeField] private AbilityInfo m_AbilityInfo = new AbilityInfo();

	private CharInfo[] m_IngameCharacterInfo = null;

	public static AbilityInfo AbilityInfo { get { return Inst.m_AbilityInfo; } }

	public static void AddDamage()
	{
		Inst.m_IngameCharacterInfo[(int)Character_Type.Player].AddDamage(AbilityInfo.Damage);
	}

	public static void AddFireRate()
	{
		Inst.m_IngameCharacterInfo[(int)Character_Type.Player].AddFireRate(AbilityInfo.FireRate);
	}

	public static void AddHPMax()
	{
		Inst.m_IngameCharacterInfo[(int)Character_Type.Player].AddHPMax(AbilityInfo.HPMax);
	}

	public static CharInfo Clone(Character_Type type)
	{
		if (type == Character_Type.Max)
			Debug.LogError("if (type == Character_Type.Max)");

		CharInfo info = new CharInfo();

		info.Copy(Inst.m_IngameCharacterInfo[(int)type]);

		return info;
	}

	protected override void Awake()
	{
		base.Awake();

		int count = (int)Character_Type.Max;

		m_IngameCharacterInfo = new CharInfo[count];

		for (int i = 0; i < count; ++i)
		{
			m_IngameCharacterInfo[i] = new CharInfo(Inst.m_CharacterInfoBase[i]);
		}
	}
}
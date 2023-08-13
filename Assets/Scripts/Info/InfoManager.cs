using UnityEngine;

public class InfoManager : Singleton<InfoManager>
{
	[ReadOnly(true)][SerializeField][EnumArray(typeof(Character_Type))] private CharInfo[] m_CharacterInfoBase = new CharInfo[(int)Character_Type.Max];
	[ReadOnly(true)][SerializeField] private AbilityInfo m_AbilityInfo = new AbilityInfo();
	[ReadOnly(true)][SerializeField] private StageInfo m_StageInfo = new StageInfo();

	private CharInfo[] m_IngameCharacterInfo = null;

	public static AbilityInfo AbilityInfo { get { return Inst.m_AbilityInfo; } }
	public static StageInfo StageInfo { get { return Inst.m_StageInfo; } }
	public static float PlayerHP { set { Inst.m_IngameCharacterInfo[(int)Character_Type.Player].HP = value; } }
	public static int WaveCount { get { return Inst.m_StageInfo.WaveCount; } }

	protected override void OnDisable()
	{
		base.OnDisable();

		SaveData();
	}

	public static int EnemyCount(int wave)
	{
		return Inst.m_StageInfo.EnemyCount(wave);
	}

	public static float SpawnTime()
	{
		return Inst.m_StageInfo.SpawnTime();
	}

	public static void AddDamage()
	{
		Inst.m_IngameCharacterInfo[(int)Character_Type.Player].AddDamage(AbilityInfo.Damage);
	}

	public static void AddFireRate()
	{
		Inst.m_IngameCharacterInfo[(int)Character_Type.Player].AddFireRate(AbilityInfo.FireRate);
	}

	public static void Heal()
	{
		Inst.m_IngameCharacterInfo[(int)Character_Type.Player].Heal(AbilityInfo.Heal);
	}

	public static void HealFull()
	{
		Inst.m_IngameCharacterInfo[(int)Character_Type.Player].Heal(1f);
	}

	public static void Speed()
	{
		Inst.m_IngameCharacterInfo[(int)Character_Type.Player].Speed(AbilityInfo.Speed);
	}

	public static void MultiShot()
	{
		Inst.m_IngameCharacterInfo[(int)Character_Type.Player].MultiShot();
	}

	public static CharInfo Clone(Character_Type type)
	{
#if UNITY_EDITOR
		if (type == Character_Type.Max)
			Debug.LogError("if (type == Character_Type.Max)");
#endif

		CharInfo info = new CharInfo();

		info.Copy(Inst.m_IngameCharacterInfo[(int)type]);

		return info;
	}

	public static void ResetEnemyInfo()
	{
		int max = (int)Character_Type.Max;

		for (int i = (int)Character_Type.Player + 1; i < max; ++i)
		{
			Inst.m_IngameCharacterInfo[i].Copy(Inst.m_CharacterInfoBase[i]);
		}
	}

	public static void RefreshEnemyInfo()
	{
		int max = (int)Character_Type.Max;

		for (int i = (int)Character_Type.Player + 1; i < max; ++i)
		{
			StageInfo.SetEnemyTotalStat(Inst.m_IngameCharacterInfo[i]);
		}
	}

	public void SaveData()
	{
		DataManager.SaveDataArray(m_CharacterInfoBase, Data_Type.CharInfo);
		DataManager.SaveData(m_AbilityInfo, Data_Type.AbilityInfo);
		DataManager.SaveData(m_StageInfo, Data_Type.StageInfo);
	}

	public void LoadData()
	{
		m_CharacterInfoBase = DataManager.LoadDataArray<CharInfo>(Data_Type.CharInfo);
		m_AbilityInfo = DataManager.LoadData<AbilityInfo>(Data_Type.AbilityInfo);
		m_StageInfo = DataManager.LoadData<StageInfo>(Data_Type.StageInfo);
	}

	protected override void Awake()
	{
		base.Awake();

		LoadData();

		int count = (int)Character_Type.Max;

		m_IngameCharacterInfo = new CharInfo[count];

		for (int i = 0; i < count; ++i)
		{
			m_IngameCharacterInfo[i] = new CharInfo(m_CharacterInfoBase[i]);
		}
	}
}
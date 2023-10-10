using UnityEngine;

public class DataManager : Singleton<DataManager>
{
	[ReadOnly(true)][SerializeField][EnumArray(typeof(Char_Type))] private CharData[] m_CharacterDataBase = new CharData[(int)Char_Type.Max];
	[ReadOnly(true)][SerializeField] private AbilityData m_AbilityData = new AbilityData();
	[ReadOnly(true)][SerializeField] private StageData m_StageData = new StageData();

	private CharData[] m_IngameCharacterData = null;

	public static AbilityData AbilityData => Inst.m_AbilityData;
	public static StageData StageData => Inst.m_StageData;
	public static int WaveCount => Inst.m_StageData.WaveCount;
	public static float PlayerHP { set => Inst.m_IngameCharacterData[(int)Char_Type.Player].HP = value; }
	public static ref readonly CharData[] CharData => ref Inst.m_IngameCharacterData;

	protected override void OnDisable()
	{
		base.OnDisable();

		SaveData();
	}

	public static int MonsterCount()
	{
		return Inst.m_StageData.MonsterCount();
	}

	public static float SpawnTime()
	{
		return Inst.m_StageData.SpawnTime();
	}

	public static void AddDamage()
	{
		Inst.m_IngameCharacterData[(int)Char_Type.Player].AddDamage(AbilityData.Damage);
		StageManager.Player.AddDamage(AbilityData.Damage);
	}

	public static void AddFireRate()
	{
		Inst.m_IngameCharacterData[(int)Char_Type.Player].AddFireRate(AbilityData.FireRate);
		StageManager.Player.AddFireRate(AbilityData.FireRate);
	}

	public static void Heal()
	{
		Inst.m_IngameCharacterData[(int)Char_Type.Player].Heal(AbilityData.Heal);
		StageManager.Player.Heal(AbilityData.Heal);
	}

	public static void HealFull()
	{
		Inst.m_IngameCharacterData[(int)Char_Type.Player].Heal(1f);
		StageManager.Player.Heal(1f);
	}

	public static void Speed()
	{
		Inst.m_IngameCharacterData[(int)Char_Type.Player].Speed(AbilityData.Speed);
		StageManager.Player.Speed(AbilityData.Speed);
	}

	public static void MultiShot()
	{
		Inst.m_IngameCharacterData[(int)Char_Type.Player].MultiShot();
		StageManager.Player.MultiShot();
	}

	public static CharData Clone(Char_Type type)
	{
		if (type == Char_Type.Max)
		{ Utility.LogError("if (type == Char_Type.Max)"); }

		CharData data = new CharData();

		data.Copy(Inst.m_IngameCharacterData[(int)type]);

		return data;
	}

	public static void ResetMonsterData()
	{
		int max = (int)Char_Type.Max;

		for (int i = (int)Char_Type.Player + 1; i < max; ++i)
		{
			Inst.m_IngameCharacterData[i].Copy(Inst.m_CharacterDataBase[i]);
		}
	}

	public static void RefreshMonsterData()
	{
		int max = (int)Char_Type.Max;

		for (int i = (int)Char_Type.Player + 1; i < max; ++i)
		{
			StageData.SetMonsterTotalStat(Inst.m_IngameCharacterData[i]);
		}
	}

	public void SaveData()
	{
		FileManager.SaveDataArray(m_CharacterDataBase, Data_Type.CharData);
		FileManager.SaveData(m_AbilityData, Data_Type.AbilityData);
		FileManager.SaveData(m_StageData, Data_Type.StageData);
	}

	public void LoadData()
	{
		m_CharacterDataBase = FileManager.LoadDataArray<CharData>(Data_Type.CharData);
		m_AbilityData = FileManager.LoadData<AbilityData>(Data_Type.AbilityData);
		m_StageData = FileManager.LoadData<StageData>(Data_Type.StageData);
	}

	protected override void Start()
	{
		base.Start();

		Utility.CheckEmpty(m_AbilityData, "m_AbilityData");
	}

	protected override void Awake()
	{
		base.Awake();

		LoadData();

		int count = (int)Char_Type.Max;

		m_IngameCharacterData = new CharData[count];

		for (int i = 0; i < count; ++i)
		{
			m_IngameCharacterData[i] = new CharData(m_CharacterDataBase[i]);
		}
	}
}
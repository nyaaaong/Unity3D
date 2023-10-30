using System.Collections.Generic;
using UnityEngine;

public class DataManager : Singleton<DataManager>
{
	public class BuffData
	{
		public float Multiplier;
		public int Percent;
		public int Stack;
	}

	public class PlayerSaveData
	{
		public int BulletUpgrade;
		public int AttackCount;
		public List<float> BulletAngles;

		public PlayerSaveData(int bulletUpgrade, int attackCount, in List<float> bulletAngles)
		{
			BulletUpgrade = bulletUpgrade;
			AttackCount = attackCount;
			BulletAngles = new List<float>(bulletAngles);
		}

		public void Copy(PlayerSaveData otherData)
		{
			BulletUpgrade = otherData.BulletUpgrade;
			AttackCount = otherData.AttackCount;

			BulletAngles.Clear();
			BulletAngles.AddRange(BulletAngles);
		}
	}

	[ReadOnly(true)][SerializeField][EnumArray(typeof(Char_Type))] private CharData[] m_CharacterDataBase = new CharData[(int)Char_Type.Max];
	[ReadOnly(true)][SerializeField] private AbilityData m_AbilityData;
	[ReadOnly(true)][SerializeField] private StageData m_StageData = new StageData();

	private CharData[] m_IngameCharacterData;
	private BuffData[] m_BuffData;
	private const float m_PlayerFireRateTimeMax = 2f;
	private const float m_MonsterFireRateTimeMax = 0.5f;
	private PlayerSaveData m_PlayerSaveData;

	public static AbilityData AbilityData => Inst.m_AbilityData;
	public static StageData StageData => Inst.m_StageData;
	public static int WaveCount => Inst.m_StageData.WaveCount;
	public static int SpawnCount => Inst.m_StageData.SpawnCount;
	public static float PlayerHP { set => Inst.m_IngameCharacterData[(int)Char_Type.Player].HP = value; }
	public static ref readonly CharData[] CharData => ref Inst.m_IngameCharacterData;
	public static bool IsPlayerFireRateTimeMax => Inst.m_IngameCharacterData[(int)Char_Type.Player].FireRateTime == m_PlayerFireRateTimeMax;
	public static float PlayerFireRateTimeMax => m_PlayerFireRateTimeMax;
	public static float MonsterFireRateTimeMax => m_MonsterFireRateTimeMax;

	public static void SavePlayerData(int bulletUpgrade, int attackCount, in List<float> bulletAngles)
	{
		Inst.m_PlayerSaveData = new PlayerSaveData(bulletUpgrade, attackCount, bulletAngles);
	}

	public static ref readonly PlayerSaveData LoadPlayerData()
	{
		return ref Inst.m_PlayerSaveData;
	}

	public static void AddPlayerLevel()
	{
		++Inst.m_IngameCharacterData[(int)Char_Type.Player].Level;
	}

	public static void ResetPlayerLevel()
	{
		if (!Inst || Inst.m_IngameCharacterData == null)
			return;

		Inst.m_IngameCharacterData[(int)Char_Type.Player].Level = 1;
	}

	public static int GetBuffStack(Ability_Type buff)
	{
		return Inst.m_BuffData[(int)buff].Stack;
	}

	public static int GetBuffStack(int abilityTypeIndex)
	{
		return Inst.m_BuffData[abilityTypeIndex].Stack;
	}

	// isEnhanced가 true라면 기존 수치에 강화된 수치를 더해서 보여준다.
	public static int GetBuffPercent(Ability_Type buff, bool isEnhanced)
	{
		int idx = (int)buff;

		if (!isEnhanced || buff == Ability_Type.Heal)
			return Inst.m_BuffData[idx].Percent;

		else if (buff == Ability_Type.MultiShot)
			return Inst.m_BuffData[idx].Percent + AbilityData.GetAbilityToInt(idx);

		float value = Inst.m_BuffData[idx].Multiplier * AbilityData.GetAbility(idx);

		if (buff == Ability_Type.FireRate && value > m_PlayerFireRateTimeMax)
			value = m_PlayerFireRateTimeMax;

		return Mathf.RoundToInt(value * 10f) * 10 - 100;
	}

	public static void AddBuffPercent(Ability_Type buff, float value)
	{
		int idx = (int)buff;

		Inst.m_BuffData[idx].Multiplier *= value;

		if (buff == Ability_Type.FireRate && Inst.m_BuffData[idx].Multiplier > m_PlayerFireRateTimeMax)
			Inst.m_BuffData[idx].Multiplier = m_PlayerFireRateTimeMax;

		Inst.m_BuffData[(int)buff].Percent = Mathf.RoundToInt(Inst.m_BuffData[idx].Multiplier * 10f) * 10 - 100; // 일의 자리 수는 반올림하게 해놨다.
		++Inst.m_BuffData[(int)buff].Stack;
	}

	public static void AddBuffPercent(Ability_Type buff, int value)
	{
		Inst.m_BuffData[(int)buff].Percent += value;
		++Inst.m_BuffData[(int)buff].Stack;
	}

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
		Ability_Type type = Ability_Type.Damage;
		float value = AbilityData.GetAbility(type);

		Inst.m_IngameCharacterData[(int)Char_Type.Player].AddDamage(value);
		StageManager.Player.AddDamage(value);
		AddBuffPercent(type, value);
	}

	public static void AddFireRate()
	{
		Ability_Type type = Ability_Type.FireRate;
		float value = AbilityData.GetAbility(type);

		Inst.m_IngameCharacterData[(int)Char_Type.Player].AddFireRate(value);
		StageManager.Player.AddFireRate(value);
		AddBuffPercent(type, value);
	}

	public static void Heal()
	{
		float value = AbilityData.GetAbility(Ability_Type.Heal);

		Inst.m_IngameCharacterData[(int)Char_Type.Player].Heal(value);
		StageManager.Player.Heal(value);
	}

	public static void HealFull()
	{
		Inst.m_IngameCharacterData[(int)Char_Type.Player].Heal(1f);
		StageManager.Player.Heal(1f);
	}

	public static void AddMoveSpeed()
	{
		Ability_Type type = Ability_Type.MoveSpeed;
		float value = AbilityData.GetAbility(type);

		Inst.m_IngameCharacterData[(int)Char_Type.Player].AddMoveSpeed(value);
		StageManager.Player.AddMoveSpeed(value);
		AddBuffPercent(type, value);
	}

	public static void MultiShot()
	{
		Ability_Type type = Ability_Type.MultiShot;
		int value = AbilityData.GetAbilityToInt(type);

		Inst.m_IngameCharacterData[(int)Char_Type.Player].MultiShot(value);
		StageManager.Player.MultiShot(value);
		AddBuffPercent(type, value);
	}

	public static CharData Clone(Char_Type type)
	{
		if (type == Char_Type.Max)
			Utility.LogError("if (type == Char_Type.Max)");

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

	public static void RefreshPlayerExpMax()
	{
		StageData.RefreshPlayerExpMax(Inst.m_IngameCharacterData[(int)Char_Type.Player]);

		if (!StageManager.IsPlayerDeath)
			StageManager.Player.CharData.DynamicExp = Inst.m_IngameCharacterData[(int)Char_Type.Player].DynamicExp;
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

		m_BuffData = new BuffData[(int)Ability_Type.Max];

		count = m_BuffData.Length;

		for (int i = 0; i < count; ++i)
		{
			m_BuffData[i] = new BuffData();
			m_BuffData[i].Multiplier = 1f;
		}

		m_BuffData[(int)Ability_Type.Heal].Percent = 50;
		m_BuffData[(int)Ability_Type.MultiShot].Percent = 1;
	}
}
using UnityEngine;

public class DataManager : Singleton<DataManager>
{
	public class BuffData
	{
		public float Multiplier;
		public int Percent;
		public int Stack;
	}

	[ReadOnly(true)][SerializeField][EnumArray(typeof(Char_Type))] private CharData[] m_CharacterDataBase = new CharData[(int)Char_Type.Max];
	[ReadOnly(true)][SerializeField] private AbilityData m_AbilityData;
	[ReadOnly(true)][SerializeField] private StageData m_StageData = new StageData();

	private CharData[] m_IngameCharacterData = null;
	private BuffData[] m_BuffData;

	public static AbilityData AbilityData => Inst.m_AbilityData;
	public static StageData StageData => Inst.m_StageData;
	public static int WaveCount => Inst.m_StageData.WaveCount;
	public static float PlayerHP { set => Inst.m_IngameCharacterData[(int)Char_Type.Player].HP = value; }
	public static ref readonly CharData[] CharData => ref Inst.m_IngameCharacterData;

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

		float value = AbilityData.GetAbility(idx);

		if (value < 1)
			value = 1 - value + 1;

		return Mathf.RoundToInt(Inst.m_BuffData[idx].Multiplier * value * 100f) - 100; // float값을 int로 바꿔주면서 퍼센트로 바꿔준다
	}

	public static void AddBuffPercent(Ability_Type buff, float value)
	{
		int idx = (int)buff;

		// 버프는 float인 경우 곱연산이기 때문에 버프 수치 작업을 먼저 해준다
		// 공격 속도처럼 1보다 작아지는 경우 1을 빼서 먼저 몇%인지 구해주고 연산을 위해 1을 더해준다.
		if (value < 1)
			value = 1 - value + 1;

		Inst.m_BuffData[idx].Multiplier *= value;

		// 곱연산 이후에는 Percent에 넣어줘야 하는데 퍼센트로 만들기 위해 100을 곱해준 후, int로 변환 및 반올림한다.
		// 마지막으로 100을 빼준다. 이유는 버프가 없을 때에는 기본적으로 1f인데 이건 100%가 아닌 0%이기 때문이다.
		AddBuffPercent(buff, Mathf.RoundToInt(Inst.m_BuffData[idx].Multiplier * 100f) - 100);
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

	public static void RefreshPlayerExpMax(CharData playerData)
	{
		StageData.RefreshPlayerExpMax(playerData);
		StageData.RefreshPlayerExpMax(Inst.m_IngameCharacterData[(int)Char_Type.Player]);
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
using UnityEngine;

public class StageUI : BaseScript
{
	[ReadOnly(true)][SerializeField] private AbilityRectCanvas m_AbilityRectCanvas; // 어빌리티 배경 캔버스
	[ReadOnly(true)][SerializeField] private StaticCanvas m_StaticCanvas; // 상시 보이게 될 캔버스
	[ReadOnly(true)][SerializeField][EnumArray(typeof(Ability_Type))] private AbilityUI[] m_AbilityUIPrefeb = new AbilityUI[(int)Ability_Type.Max]; // 클릭할 수 있는 어빌리티들

	private AbilityUI[] m_AbilityUI = new AbilityUI[3]; // 랜덤으로 프리팹에서 3개만 뽑아오게 한다
	private bool m_ShowAbility;
	private int[] m_AbilityIndex = new int[(int)Ability_Type.Max];
	private int[] m_AbilityIndexExcludeFireRate = new int[(int)Ability_Type.Max - 1];

	public bool IsShowAbility => m_ShowAbility;

	public FloatingJoystick Joystick => m_StaticCanvas.Joystick;
	public float AddExp { set => m_StaticCanvas.AddExp = value; }
	public bool NeedLevelUp => m_StaticCanvas.NeedLevelUp;
	public bool NeedUpdate => m_StaticCanvas.NeedUpdate;

	public void ResetUI()
	{
		m_StaticCanvas.ResetUI();
		m_StaticCanvas.ResetExp();
	}

	public void SetBossHPOwner(Character owner)
	{
		m_StaticCanvas.SetBossHPOwner(owner);
	}

	public void ResetExp()
	{
		m_StaticCanvas.ResetExp();
	}

	public void ShowAbility()
	{
		m_ShowAbility = true;
		StageManager.Pause();

		m_AbilityRectCanvas.gameObject.SetActive(true);

		if (!DataManager.IsFireRateTimeMax)
		{
			Utility.Shuffle(m_AbilityIndex, Random.Range(1, 10000));

			for (int i = 0; i < 3; ++i)
			{
				m_AbilityUI[i] = m_AbilityRectCanvas.LinkAbility(i, m_AbilityUIPrefeb[m_AbilityIndex[i]]);
			}
		}

		else // 플레이어의 공격 속도가 최대치에 도달하면 공격 속도 버프는 제외하고 꺼내준다.
		{
			Utility.Shuffle(m_AbilityIndexExcludeFireRate, Random.Range(1, 10000));

			for (int i = 0; i < 3; ++i)
			{
				m_AbilityUI[i] = m_AbilityRectCanvas.LinkAbility(i, m_AbilityUIPrefeb[m_AbilityIndexExcludeFireRate[i]]);
			}
		}

		AudioManager.PauseNeedBossSpawnAudio();
	}

	public void HideAbility()
	{
		m_ShowAbility = false;

		m_AbilityRectCanvas.gameObject.SetActive(false);

		for (int i = 0; i < 3; ++i)
		{
			if (m_AbilityUI[i])
			{
				Destroy(m_AbilityUI[i]);

				m_AbilityUI[i] = null;
			}
		}

		AudioManager.ResumeNeedBossSpawnAudio();
	}

	protected override void Awake()
	{
		base.Awake();

		Utility.CheckEmpty(m_AbilityRectCanvas, "m_AbilityRectCanvas");
		Utility.CheckEmpty(m_AbilityUIPrefeb, "m_AbilityUIPrefeb");
		Utility.CheckEmpty(m_StaticCanvas, "m_StaticCanvas");

		int Size = m_AbilityUIPrefeb.Length;

		for (int i = 0; i < Size; ++i)
		{
			m_AbilityIndex[i] = i;
		}

		Size = m_AbilityIndexExcludeFireRate.Length;
		int prefebIndex = 0;

		for (int i = 0; i < Size; ++i, ++prefebIndex)
		{
			if (prefebIndex == (int)Ability_Type.FireRate)
				m_AbilityIndexExcludeFireRate[i] = ++prefebIndex;

			else
				m_AbilityIndexExcludeFireRate[i] = prefebIndex;
		}

		HideAbility();
	}
}
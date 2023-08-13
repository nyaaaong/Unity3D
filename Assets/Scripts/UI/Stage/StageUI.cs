using UnityEngine;

public class StageUI : BaseScript
{
	[ReadOnly(true)][SerializeField] private AbilityRectCanvas m_AbilityRectCanvas; // 어빌리티 배경 캔버스
	[ReadOnly(true)][SerializeField] private PopupCanvas m_WaveCanvas; // 웨이브를 나타낼 캔버스
	[ReadOnly(true)][SerializeField] private StaticCanvas m_StaticCanvas; // 상시 보이게 될 캔버스
	[ReadOnly(true)][SerializeField][EnumArray(typeof(Ability_Type))] private AbilityUI[] m_AbilityUIPrefeb = new AbilityUI[(int)Ability_Type.Max]; // 클릭할 수 있는 어빌리티들

	private AbilityUI[] m_AbilityUI = new AbilityUI[3]; // 랜덤으로 프리팹에서 3개만 뽑아오게 한다
	private bool m_ShowAbility;
	private bool m_ShowWave; // 웨이브가 보이는지 판단하고, 안보인다면 StaticCavnas를 보이게 해야 한다
	private int[] m_AbilityIndex = new int[(int)Ability_Type.Max];

	public bool IsShowAbility { get { return m_ShowAbility; } }
	public bool IsShowWave
	{
		get { return m_ShowWave; }

		set
		{
			m_ShowWave = value;

			m_StaticCanvas.gameObject.SetActive(!m_ShowWave);
		}
	}

	public int Stage { get { return m_StaticCanvas.Stage; } set { m_StaticCanvas.Stage = value; } }
	public int Score { get { return m_StaticCanvas.Score; } set { m_StaticCanvas.Score = value; } }
	public FloatingJoystick Joystick { get { return m_StaticCanvas.Joystick; } }

	public void ShowPopup(Popup_Type type, int waveNum = -1)
	{
		m_WaveCanvas.ShowPopup(type, waveNum);
	}

	public void NextWave()
	{
		StageManager.NextWave();
	}

	public void ShowAbility()
	{
		m_ShowAbility = true;

		m_AbilityRectCanvas.gameObject.SetActive(true);

		Utility.Shuffle(m_AbilityIndex, Random.Range(1, 10000));

		for (int i = 0; i < 3; ++i)
		{
			m_AbilityUI[i] = m_AbilityRectCanvas.LinkAbility(i, m_AbilityUIPrefeb[m_AbilityIndex[i]]);
		}
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
	}

	protected override void Awake()
	{
		base.Awake();

#if UNITY_EDITOR
		if (!m_AbilityRectCanvas)
			Debug.LogError("if (!m_AbilityRectCanvas)");

		Utility.CheckEmpty(m_AbilityUIPrefeb, "m_AbilityUIPrefeb");

		if (!m_WaveCanvas)
			Debug.LogError("if (!m_WaveCanvas)");

		if (!m_StaticCanvas)
			Debug.LogError("if (!m_StaticCanvas)");
#endif

		int Size = m_AbilityUIPrefeb.Length;

		for (int i = 0; i < Size; ++i)
		{
			m_AbilityIndex[i] = i;
		}

		HideAbility();
	}
}
using UnityEngine;

public class UIManager : Singleton<UIManager>
{
	[ReadOnly(true)][SerializeField] private AbilityRect m_AbilityRectCanvas; // 어빌리티 배경 캔버스
	[ReadOnly(true)][SerializeField] private GameObject m_AbilityCanvas; // 어빌리티를 나타낼 캔버스
	[ReadOnly(true)][SerializeField] private GameObject[] m_AbilityUIPrefeb = new GameObject[(int)Ability_Type.Max]; // 클릭할 수 있는 어빌리티들

	private Vector3[] m_AbilityRectPos; // 어빌리티 배경의 왼쪽, 가운데, 오른쪽 위치
	private GameObject[] m_AbilityUI = new GameObject[3]; // 랜덤으로 프리팹에서 3개만 뽑아오게 한다
	private bool m_ShowAbility;
	private int[] m_AbilityIndex = new int[(int)Ability_Type.Max];

	public static bool IsShowAbility { get { return Inst.m_ShowAbility; } }

	public static void ShowAbility()
	{
		Inst.m_ShowAbility = true;

		Inst.m_AbilityRectCanvas.gameObject.SetActive(true);
		Inst.m_AbilityCanvas.SetActive(true);

		Utility.Shuffle(Inst.m_AbilityIndex, Random.Range(1, 10000));

		for (int i = 0; i < 3; ++i)
		{
			Inst.m_AbilityUI[i] = Instantiate(Inst.m_AbilityUIPrefeb[Inst.m_AbilityIndex[i]], Inst.m_AbilityCanvas.transform);
			Inst.m_AbilityUI[i].GetComponent<RectTransform>().anchoredPosition3D = Inst.m_AbilityRectPos[i];
		}
	}

	public static void HideAbility()
	{
		Inst.m_ShowAbility = false;

		Inst.m_AbilityRectCanvas.gameObject.SetActive(false);
		Inst.m_AbilityCanvas.SetActive(false);

		for (int i = 0; i < 3; ++i)
		{
			if (Inst.m_AbilityUI[i])
			{
				Destroy(Inst.m_AbilityUI[i]);

				Inst.m_AbilityUI[i] = null;
			}
		}
	}

	protected override void Awake()
	{
		base.Awake();

		if (m_AbilityRectCanvas == null)
			Debug.LogError("if (m_AbilityRectCanvas == null)");

		if (m_AbilityUIPrefeb.Length == 0)
			Debug.LogError("if (m_AbilityUIPrefeb.Length == 0)");

		else
		{
			int Size = m_AbilityUIPrefeb.Length;

			for (int i = 0; i < Size; ++i)
			{
				if (m_AbilityUIPrefeb[i] == null)
					Debug.LogError("if (m_AbilityUIPrefeb[i] == null)");

				m_AbilityIndex[i] = i;
			}
		}

		if (!m_AbilityCanvas)
			Debug.LogError("if (!m_AbilityCanvas)");


		m_AbilityRectCanvas.GetRectPos(out Inst.m_AbilityRectPos);

		HideAbility();
	}
}
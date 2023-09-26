using UnityEngine;

public class AbilityRectCanvas : BaseScript
{
	[ReadOnly(true)][SerializeField][EnumArray(typeof(AbilityRect_Position))] private AbilityRectUI[] m_Rect = new AbilityRectUI[(int)AbilityRect_Position.Max]; // 각 어빌리티 (좌, 중간, 우)
	[ReadOnly(true)][SerializeField] private float m_ClickedSize = 0.8f; // 어빌리티 클릭 시 원래 크기에서 얼마나 작아질지

	private bool m_Selected;

	public bool Selected { get { return m_Selected; } set { m_Selected = value; } }
	public float ClickedSize { get { return m_ClickedSize; } }

	public AbilityUI LinkAbility(int index, AbilityUI abilityUIPrefeb)
	{
#if UNITY_EDITOR
		if (m_Rect.Length < index)
			Debug.LogError("if (m_Rect.Length < index)");
#endif

		AbilityUI newAbilityUI = Instantiate(abilityUIPrefeb.gameObject, m_Rect[index].transform).GetComponent<AbilityUI>();
		m_Rect[index].AbilityUI = newAbilityUI;

		return newAbilityUI;
	}

	protected override void OnEnable()
	{
		base.OnEnable();

		AudioManager.PlayAbilityBGM();
	}

	protected override void Awake()
	{
		base.Awake();

#if UNITY_EDITOR
		Utility.CheckEmpty(m_Rect, "m_Rect");
#endif
	}
}
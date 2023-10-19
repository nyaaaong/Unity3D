using UnityEngine;

public class AbilityRectCanvas : BaseScript
{
	[ReadOnly(true)][SerializeField][EnumArray(typeof(AbilityRect_Position))] private AbilityRectUI[] m_Rect = new AbilityRectUI[(int)AbilityRect_Position.Max]; // 각 어빌리티 (좌, 중간, 우)
	[ReadOnly(true)][SerializeField] private float m_ClickedSize = 0.8f; // 어빌리티 클릭 시 원래 크기에서 얼마나 작아질지

	public float ClickedSize => m_ClickedSize;

	public AbilityUI LinkAbility(int index, AbilityUI abilityUIPrefeb)
	{
		if (m_Rect.Length < index)
			Utility.LogError("if (m_Rect.Length < index)");

		AbilityUI newAbilityUI = Utility.Instantiate(abilityUIPrefeb.gameObject, m_Rect[index].transform).GetComponent<AbilityUI>();
		m_Rect[index].AbilityUI = newAbilityUI;

		return newAbilityUI;
	}

	protected override void Awake()
	{
		base.Awake();

		Utility.CheckEmpty(m_Rect, "m_Rect");
	}
}
using UnityEngine;

public class AbilityRectCanvas : BaseScript
{
	[ReadOnly(true)][SerializeField][EnumArray(typeof(AbilityRect_Position))] private AbilityRectUI[] m_Rect = new AbilityRectUI[(int)AbilityRect_Position.Max];
	[ReadOnly(true)][SerializeField] private float m_AnimTime = .05f;
	[ReadOnly(true)][SerializeField] private Vector2 m_AnimMinSize;

	private bool m_Selected;

	public bool Selected { get { return m_Selected; } set { m_Selected = value; } }
	public float AnimTime { get { return m_AnimTime; } }
	public Vector2 AnimMinSize { get { return m_AnimMinSize; } }

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

using UnityEngine;

public class MenuUI : BaseScript
{
	[ReadOnly(true)][SerializeField] private OptionUI m_OptionUI;
	[ReadOnly(true)][SerializeField] private ContinueUI m_ContinueUI;

	private bool m_ShowMenu;

	public bool IsShowMenu { get { return m_ShowMenu; } }

	public bool ShowMenu(Menu_Type menu)
	{
		switch (menu)
		{
			case Menu_Type.Option:
				if (m_ShowMenu)
					return false;
				m_OptionUI.gameObject.SetActive(true);
				break;
			case Menu_Type.Continue:
				if (m_ShowMenu)
					return false;
				m_ContinueUI.gameObject.SetActive(true);
				break;
		}

		m_ShowMenu = true;

		return true;
	}

	public bool HideMenu(Menu_Type menu)
	{
		switch (menu)
		{
			case Menu_Type.Option:
				if (!m_ShowMenu)
					return false;
				m_OptionUI.gameObject.SetActive(false);
				break;
			case Menu_Type.Continue:
				if (!m_ShowMenu)
					return false;
				m_ContinueUI.gameObject.SetActive(false);
				break;
			case Menu_Type.All:
				if (!m_ShowMenu)
					return false;
				m_OptionUI.gameObject.SetActive(false);
				m_ContinueUI.gameObject.SetActive(false);
				break;
		}

		m_ShowMenu = false;

		return true;
	}

	protected override void Awake()
	{
		base.Awake();

#if UNITY_EDITOR
		if (!m_OptionUI)
			Debug.LogError("if (!m_OptionUI)");

		if (!m_ContinueUI)
			Debug.LogError("if (!m_ContinueUI)");
#endif

		HideMenu(Menu_Type.All);
	}
}

using UnityEngine;

public class MenuUI : BaseScript
{
	[ReadOnly(true)][SerializeField] private OptionUI m_OptionUI;
	[ReadOnly(true)][SerializeField] private ContinueUI m_ContinueUI;

	private bool m_ShowMenu;

	public bool IsShowMenu => m_ShowMenu;

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
				StageManager.Pause();
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
				StageManager.Resume();
				m_ContinueUI.gameObject.SetActive(false);
				break;
		}

		m_ShowMenu = false;

		return true;
	}

	protected override void Awake()
	{
		base.Awake();

		Utility.CheckEmpty(m_OptionUI, "m_OptionUI");
		Utility.CheckEmpty(m_ContinueUI, "m_ContinueUI");

		HideMenu(Menu_Type.Option);
		HideMenu(Menu_Type.Continue);
	}
}
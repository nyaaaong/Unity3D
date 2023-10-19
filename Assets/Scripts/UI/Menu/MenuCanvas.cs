
using UnityEngine;

public class MenuCanvas : BaseScript
{
	[ReadOnly(true)][SerializeField] private MenuUI m_MenuUI;
	[ReadOnly(true)][SerializeField] private ContinueUI m_ContinueUI;

	private bool m_ShowMenu;

	public bool IsShowMenu => m_ShowMenu;

	public bool ShowMenu(Menu_Type menu)
	{
		switch (menu)
		{
			case Menu_Type.Menu:
				if (m_ShowMenu)
					return false;
				m_MenuUI.gameObject.SetActive(true);
				break;
			case Menu_Type.Continue:
				if (m_ShowMenu)
					return false;
				m_ContinueUI.gameObject.SetActive(true);
				break;
		}

		m_ShowMenu = true;
		StageManager.Pause();

		return true;
	}

	public bool HideMenu(Menu_Type menu)
	{
		switch (menu)
		{
			case Menu_Type.Menu:
				if (!m_ShowMenu)
					return false;
				m_MenuUI.gameObject.SetActive(false);
				break;
			case Menu_Type.Continue:
				if (!m_ShowMenu)
					return false;
				m_ContinueUI.gameObject.SetActive(false);
				break;
		}

		m_ShowMenu = false;
		StageManager.Resume();

		return true;
	}

	protected override void Awake()
	{
		base.Awake();

		Utility.CheckEmpty(m_MenuUI, "m_MenuUI");
		Utility.CheckEmpty(m_ContinueUI, "m_ContinueUI");

		HideMenu(Menu_Type.Menu);
		HideMenu(Menu_Type.Continue);
	}
}
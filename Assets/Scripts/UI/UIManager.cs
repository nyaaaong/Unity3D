using System;
using UnityEngine;

public class UIManager : Singleton<UIManager>
{
	[ReadOnly(true)][SerializeField] private StageUI m_StageUI;
	[ReadOnly(true)][SerializeField] private MenuUI m_MenuUI;

	private event Action m_ShowMenuEvent;
	private event Action m_HideMenuEvent;

	public static bool IsShowAbility { get { return Inst.m_StageUI.IsShowAbility; } }
	public static bool IsShowWave { get { return Inst.m_StageUI.IsShowWave; } }
	public static bool IsShowMenu { get { return Inst.m_MenuUI.IsShowMenu; } }
	public static int Stage { get { return Inst.m_StageUI.Stage; } set { Inst.m_StageUI.Stage = value; } }
	public static int Score { get { return Inst.m_StageUI.Score; } set { Inst.m_StageUI.Score = value; } }
	public static FloatingJoystick Joystick { get { return Inst.m_StageUI.Joystick; } }

	public static void AddHideMenuEvent(Action action)
	{
		if (Inst.m_HideMenuEvent != action)
			Inst.m_HideMenuEvent += action;
	}

	public static void RemoveHideMenuEvent(Action action)
	{
		if (Inst.m_HideMenuEvent != null)
		{
			if (Inst.m_HideMenuEvent == action)
				Inst.m_HideMenuEvent -= action;
		}
	}

	public static void AddShowMenuEvent(Action action)
	{
		if (Inst.m_ShowMenuEvent != action)
			Inst.m_ShowMenuEvent += action;
	}

	public static void RemoveShowMenuEvent(Action action)
	{
		if (Inst.m_ShowMenuEvent != null)
		{
			if (Inst.m_ShowMenuEvent == action)
				Inst.m_ShowMenuEvent -= action;
		}
	}

	public static bool ShowMenu(Menu_Type type)
	{
		if (Inst.m_ShowMenuEvent != null)
			Inst.m_ShowMenuEvent();

		return Inst.m_MenuUI.ShowMenu(type);
	}

	public static bool HideMenu(Menu_Type type)
	{
		if (Inst.m_HideMenuEvent != null)
			Inst.m_HideMenuEvent();

		return Inst.m_MenuUI.HideMenu(type);
	}

	public static void ShowPopup(Popup_Type type, int waveNum = -1)
	{
		Inst.m_StageUI.ShowPopup(type, waveNum);
	}

	public static void HideAbility()
	{
		Inst.m_StageUI.HideAbility();
	}
}
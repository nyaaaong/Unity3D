using System;
using UnityEngine;

public class UIManager : Singleton<UIManager>
{
	[ReadOnly(true)][SerializeField] private StageUI m_StageUI;
	[ReadOnly(true)][SerializeField] private MenuCanvas m_MenuCanvas;

	private event Action m_ShowMenuEvent;
	private event Action m_HideMenuEvent;

	public static bool IsShowAbility => Inst.m_StageUI.IsShowAbility;
	public static bool IsHideAbility => !Inst.m_StageUI.IsShowAbility;
	public static bool IsShowMenu => Inst.m_MenuCanvas.IsShowMenu;
	public static FloatingJoystick Joystick => Inst.m_StageUI.Joystick;
	public static float AddExp { set => Inst.m_StageUI.AddExp = value; }
	public static bool NeedLevelUp => Inst.m_StageUI.NeedLevelUp;
	public static bool NeedUpdate => Inst.m_StageUI.NeedUpdate;

	public static void ResetUI()
	{
		Inst.m_StageUI.ResetUI();
	}

	public static void SetBossHPOwner(Character owner)
	{
		Inst.m_StageUI.SetBossHPOwner(owner);
	}

	public static void ResetExp()
	{
		if (!Inst)
			return;

		Inst.m_StageUI.ResetExp();
	}

	public static void ShowAbility()
	{
		Inst.m_StageUI.ShowAbility();
	}

	public static void AddHideMenuEvent(Action action)
	{
		if (Inst.m_HideMenuEvent != action)
			Inst.m_HideMenuEvent += action;
	}

	public static void RemoveHideMenuEvent(Action action)
	{
		if (!Inst)
			return;

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
		if (!Inst)
			return;

		if (Inst.m_ShowMenuEvent != null)
		{
			if (Inst.m_ShowMenuEvent == action)
				Inst.m_ShowMenuEvent -= action;
		}
	}

	public static void ShowMenu(Menu_Type type)
	{
		if (!Inst)
			return;

		if (Inst.m_ShowMenuEvent != null)
			Inst.m_ShowMenuEvent();

		Inst.m_MenuCanvas.ShowMenu(type);
	}

	public static void HideMenu(Menu_Type type)
	{
		if (Inst.m_HideMenuEvent != null)
			Inst.m_HideMenuEvent();

		Inst.m_MenuCanvas.HideMenu(type);
	}

	public static void HideAbility()
	{
		Inst.m_StageUI.HideAbility();
	}
}
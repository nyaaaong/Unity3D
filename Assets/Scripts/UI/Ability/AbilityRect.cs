using UnityEngine;

public class AbilityRect : BaseScript
{
	[ReadOnly(true)][SerializeField] private RectTransform[] m_RectPos = new RectTransform[3]; // Left, Center, Right

	public void GetRectPos(out Vector3[] rectPos)
	{
		rectPos = new Vector3[3];

		for (int i = 0; i < 3; ++i)
		{
			rectPos[i] = m_RectPos[i].anchoredPosition3D;
		}
	}

	protected override void Awake()
	{
		base.Awake();

		foreach (Transform tr in m_RectPos)
		{
			if (tr == null)
				Debug.LogError("if (tr == null)");
		}
	}
}
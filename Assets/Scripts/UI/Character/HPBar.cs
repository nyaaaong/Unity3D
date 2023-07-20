using UnityEngine;
using UnityEngine.UI;

public class HPBar : BaseScript
{
	private Character m_Owner;
	private Slider m_HPSlider;
	private float m_HP = 1;
	private float m_HPMax = 1;
	private float m_HPSpeed = 10f;
	private float m_LerpTime = 0f;

	private void WorldToScreen()
	{
		transform.position = m_Owner.Pos;
	}

	protected override void Awake()
	{
		base.Awake();

		m_Owner = transform.root.GetComponentInChildren<Character>();
		m_HPSlider = transform.Find("HPBar").GetComponent<Slider>();

		m_HPMax = m_Owner.HPMax;
		m_HP = m_HPMax;
	}

	protected override void OnEnable()
	{
		base.OnEnable();

		m_HPSlider.value = 1f;
	}

	protected override void AfterUpdate()
	{
		base.AfterUpdate();

		WorldToScreen();

		m_HP = m_Owner.HP;

		m_LerpTime = Mathf.Clamp(m_deltaTime * m_HPSpeed, 0f, 1f);

		if (m_HP > 0f)
			m_HPSlider.value = Mathf.Lerp(m_HPSlider.value, m_HP / m_HPMax, m_LerpTime);

		else
		{
			if (m_HPSlider.value > 1f)
				m_HPSlider.value = Mathf.Lerp(m_HPSlider.value, 0f, m_LerpTime);

			else
				gameObject.SetActive(false);
		}
	}
}
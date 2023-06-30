
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class HPBar : BaseScript
{
	private Character m_Owner;
	private Slider m_HPSlider;
	private float m_HP = 1;
	private float m_HPMax = 1;
	private float m_HPSpeed = 10f;

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

		if (m_Owner)
		{
			transform.position = m_Owner.transform.position;

			m_HP = m_Owner.HP;
		}

		if (m_HP > 0f)
			m_HPSlider.value = Mathf.Lerp(m_HPSlider.value, m_HP / m_HPMax, m_deltaTime * m_HPSpeed);

		else
		{
			if (m_HPSlider.value > 1f)
				m_HPSlider.value = Mathf.Lerp(m_HPSlider.value, 0f, m_deltaTime * m_HPSpeed);

			else
				gameObject.SetActive(false);
		}
	}
}
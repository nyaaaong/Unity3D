using UnityEngine;
using UnityEngine.UI;

public class HPBar : BaseScript
{
	private Character m_Owner;
	private Slider m_HPSlider;
	private Image m_HPBar;
	private float m_HP = 1;
	private float m_Heal = 1;
	private float m_LerpSpeed = 10f;
	private float m_LerpTime = 0f;

	private float HP
	{
		get
		{
			if (m_HPSlider != null)
				return m_HPSlider.value;

			return m_HPBar.fillAmount;
		}

		set
		{
			if (m_HPSlider != null)
				m_HPSlider.value = value;

			else
				m_HPBar.fillAmount = value;
		}
	}

	private void WorldToScreen()
	{
		transform.position = m_Owner.transform.position;
	}

	protected override void Awake()
	{
		base.Awake();

		m_Owner = transform.root.GetComponentInChildren<Character>();

		Transform hpbar = transform.Find("HPBar");

		m_HPSlider = hpbar.GetComponent<Slider>();
		m_HPBar = hpbar.GetComponent<Image>();

		m_Heal = m_Owner.HPMax;
		m_HP = m_Heal;
	}

	protected override void OnEnable()
	{
		base.OnEnable();

		HP = 1f;
	}

	protected override void AfterUpdate()
	{
		base.AfterUpdate();

		WorldToScreen();

		m_HP = m_Owner.HP;

		m_LerpTime = Mathf.Clamp(m_deltaTime * m_LerpSpeed, 0f, 1f);

		if (m_HP > 0f)
			HP = Mathf.Lerp(HP, m_HP / m_Heal, m_LerpTime);

		else
		{
			if (HP > 1f)
				HP = Mathf.Lerp(HP, 0f, m_LerpTime);

			else
				gameObject.SetActive(false);
		}
	}
}
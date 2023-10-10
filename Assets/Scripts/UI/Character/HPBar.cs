using System;
using UnityEngine;
using UnityEngine.UI;

public class HPBar : BaseScript
{
	[ReadOnly(true)][SerializeField] private bool m_BossHPBar;

	private Character m_Owner;
	private Slider m_HPSlider;
	private Image m_HPBar;
	private float m_HP;
	private float m_HPMax;
	private float m_LerpSpeed = 10f;
	private float m_LerpTime = 0f;

	public event Action OnHide;

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

	public void SetOwner(Character owner = null, Action onHide = null)
	{
		if (owner == null)
			m_Owner = transform.root.GetComponentInChildren<Character>();

		else
		{
			m_Owner = owner;
			OnHide += onHide;
		}

		m_HPMax = m_Owner.HPMax;
		m_HP = m_HPMax;
	}

	protected override void Awake()
	{
		base.Awake();

		Transform hpbar = transform.Find("HPBar");

		m_HPSlider = hpbar.GetComponent<Slider>();
		m_HPBar = hpbar.GetComponent<Image>();

		if (!m_BossHPBar)
			SetOwner();
	}

	protected override void OnEnable()
	{
		base.OnEnable();

		HP = 1f;
	}

	protected override void OnDisable()
	{
		base.OnDisable();

		if (m_BossHPBar)
		{
			m_Owner = null;
			OnHide = null;
		}
	}

	protected override void LateUpdate()
	{
		base.LateUpdate();

		if (!m_BossHPBar)
			WorldToScreen();

		m_HP = m_Owner.HP;

		m_LerpTime = Mathf.Clamp(Time.deltaTime * m_LerpSpeed, 0f, 1f);

		if (m_HP > 0f)
			HP = Mathf.Lerp(HP, m_HP / m_HPMax, m_LerpTime);

		else
		{
			if (HP > 1f)
				HP = Mathf.Lerp(HP, 0f, m_LerpTime);

			else if (m_BossHPBar)
				OnHide();

			else
				gameObject.SetActive(false);
		}
	}
}
using UnityEngine.UI;

public class StaticExpText : BaseScript
{
	private Text m_Text;
	private float m_InputExp;
	private int m_Exp;

	public void SetExp(float exp)
	{
		if (m_InputExp != exp)
		{
			m_InputExp = exp;

			m_Exp = (int)(exp * 100f);

			m_Text.text = m_Exp.ToString() + "%";
		}
	}

	protected override void Awake()
	{
		base.Awake();

		m_Text = GetComponent<Text>();
	}
}
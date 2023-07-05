using UnityEngine;

public class BaseScript : MonoBehaviour
{
	private bool m_InitProc;
	protected float m_deltaTime = 0f;
	protected float m_unscaleDeltaTime = 0f;
	protected float m_fixedDeltaTime = 0f;

	protected virtual void Awake() { }

	protected virtual void OnEnable() 
	{
		m_InitProc = false;
	}

	protected virtual void Start() { }

	protected virtual void FixedUpdate()
	{
		m_fixedDeltaTime = Time.fixedDeltaTime;
	}

	protected virtual void Init()
	{
		m_InitProc = true;
	}

	protected virtual void BeforeUpdate()
	{
		m_deltaTime = Time.deltaTime;
		m_unscaleDeltaTime = Time.unscaledDeltaTime;
	}

	protected virtual void AfterUpdate() { }

	private void Update()
	{
		if (!m_InitProc)
			Init();

		BeforeUpdate();
		AfterUpdate();
	}

	protected virtual void LateUpdate()
	{
		m_deltaTime = Time.deltaTime;
		m_unscaleDeltaTime = Time.unscaledDeltaTime;
	}

	protected virtual void OnDisable() { }
}
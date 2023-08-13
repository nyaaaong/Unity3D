using UnityEngine;

public class BaseScript : MonoBehaviour
{
	protected float m_deltaTime = 0f;
	protected float m_unscaleDeltaTime = 0f;
	protected float m_fixedDeltaTime = 0f;
	protected static bool m_Quit;

	protected virtual void Awake() { }

	protected virtual void OnEnable() { }

	protected virtual void Start() { }

	protected virtual void FixedUpdate()
	{
		m_fixedDeltaTime = Time.fixedDeltaTime;
	}

	protected virtual void BeforeUpdate()
	{
		m_deltaTime = Time.deltaTime;
		m_unscaleDeltaTime = Time.unscaledDeltaTime;
	}

	protected virtual void AfterUpdate() { }

	private void Update()
	{
		BeforeUpdate();
		AfterUpdate();
	}

	protected virtual void LateUpdate()
	{
		m_deltaTime = Time.deltaTime;
		m_unscaleDeltaTime = Time.unscaledDeltaTime;
	}

	protected virtual void OnDisable() { }

	private void OnApplicationQuit()
	{
		m_Quit = true;
	}

	protected virtual void OnDestroy() { }
}
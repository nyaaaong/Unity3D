using UnityEngine;

public class GunController : BaseScript
{
	[SerializeField] private Transform m_Hand;
	[SerializeField] private Gun m_NormalGun;

	private Gun m_EquipGun;

	public void SetInfo(float maxDist, Bullet_Owner owner)
	{
		m_EquipGun.SetInfo(maxDist, owner);
	}

	protected override void Awake()
	{
		base.Awake();

		if (m_NormalGun)
			EquipGun(m_NormalGun);
	}

	public void EquipGun(Gun gun)
	{
		if (m_EquipGun)
			Destroy(m_EquipGun.gameObject);

		m_EquipGun = Instantiate(gun, m_Hand.position, m_Hand.rotation);
		m_EquipGun.transform.parent = m_Hand;
	}

	public void Shoot(bool shoot)
	{
		if (m_EquipGun)
			m_EquipGun.Shoot(shoot);
	}
}

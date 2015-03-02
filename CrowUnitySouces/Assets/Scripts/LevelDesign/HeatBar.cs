using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using UnityEngine.UI;
using System;

public class HeatBar : MonoBehaviour {

	private static HeatBar _instance;

	public static HeatBar Instance
	{
		get{ 
			if(_instance == null)
				_instance = GameObject.FindObjectOfType<HeatBar>();
			return _instance; }
	}

	Image[] m_heatUnits;
	public Color m_activeColor = Color.red;
	public Color m_inactiveColor = Color.grey;
	public float m_unitCooldownSpeed = 0.1f;
	public float m_unitCooldownDelay = 1.0f;
	public GameObject m_overheatGameObject;
	float m_unitCooldownSpeedTimer;
	float m_unitCooldownDelayTimer;
	int m_currentHeatIndex = -1;

	public UnityEvent _onOverHeat;
	
	void Start () 
	{
		m_heatUnits = GetComponentsInChildren<Image> ();
		Array.Reverse (m_heatUnits);
	}

	public void AddHeatUnits(int amount)
	{
		//m_currentHeatIndex = Mathf.Max (0, m_currentHeatIndex); //Reset to 0 if at -1
		int nextHeatLevel = m_currentHeatIndex + amount;
		if (nextHeatLevel >= m_heatUnits.Length - 1) 
		{
			m_overheatGameObject.SetActive(true);
			nextHeatLevel = m_heatUnits.Length - 1;
		}

		for (int i = Mathf.Max (0, m_currentHeatIndex); i <= nextHeatLevel; ++i)
			m_heatUnits [i].color = m_activeColor;

		m_unitCooldownDelayTimer = m_unitCooldownDelay;
		m_unitCooldownSpeedTimer = m_unitCooldownSpeed;
		m_currentHeatIndex = nextHeatLevel;
	}

	void Update () 
	{
		if (Input.GetKeyDown (KeyCode.Space))
			AddHeatUnits (1);

		if (m_currentHeatIndex < 0)
			return;

		m_unitCooldownDelayTimer -= Time.deltaTime;
		m_unitCooldownDelayTimer = Mathf.Max (m_unitCooldownDelayTimer, 0);

		if (m_unitCooldownDelayTimer > 0)
			return;

		m_heatUnits [m_currentHeatIndex].color = Color.Lerp (m_inactiveColor, m_activeColor, m_unitCooldownSpeedTimer / m_unitCooldownSpeed);

		m_unitCooldownSpeedTimer -= Time.deltaTime;
		while (m_unitCooldownSpeedTimer <= 0) {
			m_unitCooldownSpeedTimer += m_unitCooldownSpeed;
			m_currentHeatIndex--;
			if(m_currentHeatIndex < 0)
				m_overheatGameObject.SetActive(false);
		}
	}
}

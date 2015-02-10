using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GameOverController : MonoBehaviour {

	private static GameOverController m_instance;
	private bool m_showFlag;

	public static GameOverController Instance
	{
		get
		{
			if (m_instance == null)
			{
				m_instance = GameObject.FindObjectOfType<GameOverController>();
			}
			
			return m_instance;
		}
	}

	void Awake()
	{
		m_instance = this;
		gameObject.SetActive(false);
	}

	public void Show()
	{
		if(m_showFlag == true)
			return;

		gameObject.SetActive(true);
		gameObject.GetComponentInChildren<Text>().text = "Distance: " + Score.Instance.DistanceTravaled;
		m_showFlag = true;
	}
}

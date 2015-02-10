using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.IO;

public class GameOverController : MonoBehaviour {

	private static GameOverController m_instance;
	private bool m_showFlag;

	public Text _distance;
	public Text _time;
	public Text _carsDestroyed;
	public Text _gadgetsUsed;

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
		_distance.text = "Distance: " + Score.Instance.DistanceTravaled;
		_time.text = "Time: " + Score.Instance._gameTime;
		_carsDestroyed.text = "Obstacles Cleared: " + Score.Instance._carsDestroyed;
		_gadgetsUsed.text = "Buttons Pressed: " + Score.Instance._gadgetsUsed;
		m_showFlag = true;
		
		string fileName = "PlaytestLog";
		StreamWriter sr;
		if (File.Exists(fileName))
			sr = File.AppendText (fileName);
		else
			sr = System.IO.File.CreateText (fileName);
	
		sr.WriteLine (_distance.text);
		sr.WriteLine (_time.text);
		sr.WriteLine (_carsDestroyed.text);
		sr.WriteLine (_gadgetsUsed.text);
		sr.WriteLine ();

		sr.Close ();
	}
}

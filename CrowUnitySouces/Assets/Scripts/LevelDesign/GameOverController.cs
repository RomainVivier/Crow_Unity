using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.IO;

public class GameOverController : MonoBehaviour {

	private static GameOverController m_instance;
	private bool m_showFlag;

	public Text _message;
	public Text _highScore;
	public Text _score;
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
		int finalScore = Score.Instance.Fuck;
		_score.text = "Score: " + finalScore;
		_distance.text = "Distance: " + Score.Instance.DistanceTravaled;
		_time.text = "Time: " + Score.Instance._gameTime;
		_carsDestroyed.text = "Obstacles Cleared: " + Score.Instance._carsDestroyed;
		_gadgetsUsed.text = "Buttons Pressed: " + Score.Instance._gadgetsUsed;
		m_showFlag = true;
		
		string fileName = "PlaytestLog.txt";
		StreamWriter sw;
		if (File.Exists(fileName))
			sw = File.AppendText (fileName);
		else
			sw = System.IO.File.CreateText (fileName);
	

		sw.WriteLine (_score.text);
		sw.WriteLine (_distance.text);
		sw.WriteLine (_time.text);
		sw.WriteLine (_carsDestroyed.text);
		sw.WriteLine (_gadgetsUsed.text);
		sw.WriteLine ();
		sw.Close ();


		fileName = "HighScore.txt";
		StreamReader sr;
		int highscore;

		if (!File.Exists (fileName)) 
		{
			File.CreateText (fileName);
			highscore = 0;
		}
		else
		{
			string text = File.ReadAllText (fileName);
			highscore = int.Parse (text);
		}

		_highScore.text = "Highscore: " + highscore;
		if ( finalScore > highscore) {
			_message.text = "HIGHSCORE EXTERMINE!";
			File.WriteAllText(fileName, finalScore.ToString());
		}
		else
		{
			_message.text = "Game Over, Man!";
		}
	}
}

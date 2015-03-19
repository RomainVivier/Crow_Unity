using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GameOverScreen : MonoBehaviour {
	public GameObject _highScoreCanvas;

	void Start ()
	{
		KeyBinder.Instance.enabled=false;
		
		RectTransform rectTransform=GetComponent<RectTransform>();
		GameInfos gi=GameInfos.Instance;
		
		Text scoreText=rectTransform.Find("ScoreText").GetComponent<Text>();
		scoreText.text="SCORE: "+gi.score;
		Text distText=rectTransform.Find("DistText").GetComponent<Text>();
		distText.text="DISTANCE: "+gi.dist+" M";
		Text comboText=rectTransform.Find("ComboText").GetComponent<Text>();
		comboText.text="COMBO MAX: "+gi.maxCombo+"X";
		
		Text nbEvents=rectTransform.Find("NbEvents").GetComponent<Text>();
		nbEvents.text=gi.nbEvents+"X";
		Text eventsCombo=rectTransform.Find("EventsCombo").GetComponent<Text>();
		eventsCombo.text="COMBO X"+gi.eventsCombo;
		Text nbMinorObstacles=rectTransform.Find("NbMinorObstacles").GetComponent<Text>();
		nbMinorObstacles.text=gi.nbMinorObstacles+"X";
		Text minorObstaclesCombo=rectTransform.Find("MinorObstaclesCombo").GetComponent<Text>();
		minorObstaclesCombo.text="COMBO X"+gi.minorObstaclesCombo;
		Text nbStuffText=rectTransform.Find("NbStuff").GetComponent<Text>();
		nbStuffText.text=gi.nbStuff+"X";
		Text stuffPoints=rectTransform.Find("StuffPoints").GetComponent<Text>();
		stuffPoints.text=gi.stuffPoints+" POINTS";
	}
	
	void Update()
	{
		if(Input.GetMouseButtonDown(0))
		{
			_highScoreCanvas.SetActive(true);
			this.gameObject.SetActive(false);
		}
	}
	
}

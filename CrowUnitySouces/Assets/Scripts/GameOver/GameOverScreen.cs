using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GameOverScreen : MonoBehaviour {
	public GameOverScript _gameOverScript;
	private Image m_background;
	
	public void init ()
	{
		
		RectTransform rectTransform=GetComponent<RectTransform>();
		GameInfos gi=GameInfos.Instance;
		
		Text scoreText=rectTransform.Find("ScoreText").GetComponent<Text>();
		scoreText.text=gi.score.ToString();
		Text distText=rectTransform.Find("DistText").GetComponent<Text>();
		distText.text=gi.dist.ToString();
		Text comboText=rectTransform.Find("ComboText").GetComponent<Text>();
		comboText.text=gi.maxCombo.ToString();
		
		Text nbEvents=rectTransform.Find("NbEvents").GetComponent<Text>();
		nbEvents.text=gi.nbEvents.ToString();
		Text nbMinorObstacles=rectTransform.Find("NbMinorObstacles").GetComponent<Text>();
		nbMinorObstacles.text=gi.nbMinorObstacles.ToString();
		Text nbStuffText=rectTransform.Find("NbStuff").GetComponent<Text>();
		nbStuffText.text=gi.nbStuff.ToString();
		
		m_background=rectTransform.Find("Background").GetComponent<Image>();
	}
	
	void Update()
	{
		if(Input.GetMouseButtonDown(0))
		{
			_gameOverScript.startHighScores();
		}
	}
	
	public void setScrollPos(float scrollPos)
	{
		m_background.material.SetTextureOffset("_MainTex",new Vector2(scrollPos,0));
	}
}

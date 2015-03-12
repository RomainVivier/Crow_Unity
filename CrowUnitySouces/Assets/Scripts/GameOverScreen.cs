using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GameOverScreen : MonoBehaviour {

	void Start ()
	{
		RectTransform rectTransform=GetComponent<RectTransform>();
		Text scoreText=rectTransform.Find("ScoreText").GetComponent<Text>();
		scoreText.text="SCORE: "+GameInfos.Instance.score;
		Text distText=rectTransform.Find("DistText").GetComponent<Text>();
		distText.text="DISTANCE: "+GameInfos.Instance.dist+" M";
		Text comboText=rectTransform.Find("ComboText").GetComponent<Text>();
		comboText.text="COMBO MAX: "+GameInfos.Instance.maxCombo+"X";
		
	}
	
	
}

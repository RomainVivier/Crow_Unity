using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class HighScoresScreen : MonoBehaviour {

	public GameOverScript _gameOverScript;
	
	private int m_currentHighScore=-1;
	private Image m_background;
	
	public void init ()
	{
		GameInfos gi=GameInfos.Instance;
		if(gi.score>gi.scores[9])
		{
			TouchScreenKeyboard.Open("");
			int place=9;
			while(place>0 && gi.scores[place-1]<gi.score)
			{
				gi.scores[place]=gi.scores[place-1];
				gi.names[place]=gi.names[place-1];
				place--;
			}
			gi.scores[place]=gi.score;
			gi.names[place]="";
			m_currentHighScore=place;
		}
		printRanking ();
		
		m_background=GetComponent<RectTransform>().Find("Background").GetComponent<Image>();
	}
	
	void Update ()
	{
		GameInfos gi=GameInfos.Instance;
		if(m_currentHighScore!=-1)
		{
			if(Input.inputString!="")
			{
				string str=gi.names[m_currentHighScore];
				if(Input.inputString=="\n" || Input.GetKeyDown(KeyCode.Return))
				{
					m_currentHighScore=-1;
					gi.save();
				}
				else if(Input.inputString=="\b") str=str.Remove(str.Length-1,1);
				else
				{
					str+=Input.inputString;
					if(str.Length>3) str=str.Remove(3,str.Length-3);
				}
				gi.names[m_currentHighScore]=str;
				printRanking ();
			}
		}
		else if(Input.GetMouseButtonDown(0))
		{
			_gameOverScript.restartGame();
		}
	}
	
	void printRanking()
	{
		RectTransform rectTransform=GetComponent<RectTransform>();
		GameInfos gi=GameInfos.Instance;
		
		Text namesText=rectTransform.Find("NamesText").GetComponent<Text>();
		namesText.text="";
		Text scoresText=rectTransform.Find("ScoresText").GetComponent<Text>();
		scoresText.text="";
		
		for(int i=0;i<10;i++)
		{
			if(i==m_currentHighScore) namesText.text+=gi.names[i]+"_\n";
			else namesText.text+=gi.names[i]+"\n";
			scoresText.text+=gi.scores[i]+"\n";
		}
	}
	
	public void setScrollPos(float scrollPos)
	{
		m_background.material.SetTextureOffset("_MainTex",new Vector2(scrollPos,0));
	}
}

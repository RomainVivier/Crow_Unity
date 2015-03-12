using UnityEngine;
using System.Collections;

public class GameInfos
{

	private static GameInfos instance=null;
	public static GameInfos Instance
	{
		get
		{
			if(instance==null) instance=new GameInfos();
			return instance;
		}
	}
	
	public int score;
	public int dist;
	public int maxCombo;
	public int nbEvents;
	public int eventsCombo;
	public int nbMinorObstacles;
	public int minorObstaclesCombo;
	public int nbStuff;
	public int stuffPoints;
	
	public int[] scores={1000,900,800,700,600,500,400,300,200,100};
	public string[] names={"DH ","RE ","IR ","V  ","ED "," A ","AY ","N  ","O  ","T  "};
	
	public void reset()
	{
		score=0;
		dist=0;
		maxCombo=0;
		nbEvents=0;
		eventsCombo=0;
		nbMinorObstacles=0;
		minorObstaclesCombo=0;
		nbStuff=0;
		stuffPoints=0;
	}
	
	public GameInfos()
	{
		reset();
	}
	
}

using UnityEngine;
using System.Collections;

public class GameInfos {

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
	
	public void reset()
	{
		score=0;
		dist=0;
		maxCombo=0;
	}
	
	public GameInfos()
	{
		reset();
	}
	
}

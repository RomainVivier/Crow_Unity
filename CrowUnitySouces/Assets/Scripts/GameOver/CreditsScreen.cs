using UnityEngine;
using System.Collections;

public class CreditsScreen : MonoBehaviour
{
	public GameOverScript _gameOverScript;
	
	public void restartClicked()
	{
		_gameOverScript.restartGame();
	}
}

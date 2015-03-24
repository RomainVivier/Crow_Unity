using UnityEngine;
using System.Collections;

public class GameOverTrigger : MonoBehaviour {

	void OnTriggerEnter(Collider other)
	{
		GameObject oth = other.transform.root.gameObject;
		if (oth.name == "CarV2")
		{
			GameObject.Find ("GameOver").GetComponent<GameOverScript>().startGameOver();
		}
		else Debug.Log(oth.name);
	}
}

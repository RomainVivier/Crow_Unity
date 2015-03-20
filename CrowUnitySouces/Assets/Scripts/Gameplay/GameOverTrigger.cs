using UnityEngine;
using System.Collections;

public class GameOverTrigger : MonoBehaviour {

	void Update()
	{
		if(Input.GetKeyDown(KeyCode.G)) GameObject.Find ("GameOver").GetComponent<GameOverScript>().startGameOver();
	}
	
	void OnTriggerEnter(Collider other)
	{
		GameObject oth = other.transform.root.gameObject;
		if (oth.name == "Car(Clone)")
		{
			GameObject.Find ("GameOver").GetComponent<GameOverScript>().startGameOver();
		}
		else Debug.Log(oth.name);
	}
}

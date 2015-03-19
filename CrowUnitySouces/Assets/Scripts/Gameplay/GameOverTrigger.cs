using UnityEngine;
using System.Collections;

public class GameOverTrigger : MonoBehaviour {

	void Update()
	{
		if(Input.GetKeyDown(KeyCode.G)) Application.LoadLevel("GameOver");
	}
	
	void OnTriggerEnter(Collider other)
	{
		Debug.Log ("aaa");
		GameObject oth = other.transform.root.gameObject;
		if (oth.name == "CarV2")
		{
			Debug.Log ("bbb");
			Application.LoadLevel("GameOver");
		}
		else Debug.Log(oth.name);
	}
}

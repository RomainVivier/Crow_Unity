﻿using UnityEngine;
using System.Collections;

public class GameOverTrigger : MonoBehaviour {

	void OnTriggerEnter(Collider other)
	{
		GameObject oth = other.transform.root.gameObject;
		if (oth.name == "Car(Clone)")
		{
			Application.LoadLevel("GameOver");
		}
		else Debug.Log(oth.name);
	}
}

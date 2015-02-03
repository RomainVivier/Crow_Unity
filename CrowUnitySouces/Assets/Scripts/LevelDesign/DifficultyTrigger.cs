using UnityEngine;
using System.Collections;

public class DifficultyTrigger : MonoBehaviour 
{
	void OnTriggerEnter(Collider other)
	{
		if (other.tag == "CarTrigger") {
		}
	}
}
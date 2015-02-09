using UnityEngine;
using System.Collections;

public class BuildingSplatController : MonoBehaviour {

	public void ShakeDatAss()
	{
		CameraShake camShake = Camera.main.GetComponent<CameraShake>();
		camShake.DoShake();
	}
}

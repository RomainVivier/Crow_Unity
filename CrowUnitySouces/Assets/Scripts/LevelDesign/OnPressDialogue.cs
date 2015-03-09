using UnityEngine;
using System.Collections;

public class OnPressDialogue : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.P))
			FMOD_StudioSystem.instance.PlayOneShot("event:/Dialog/IA/BCP/AI Scripted/AI_End",transform.position);
	}
}

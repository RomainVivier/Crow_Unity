using UnityEngine;
using System.Collections;

public class LoopSplosion : MonoBehaviour {

	public float m_rateInSeconds = 1.0f;
	private ParticleSystem m_particleSystem;


	// Use this for initialization
	void Start () {
		m_particleSystem = GetComponent<ParticleSystem>();
		InvokeRepeating ("Play", m_rateInSeconds, m_rateInSeconds);
	}
	
	// Update is called once per frame
	void Update () {

	}

	void Play()
	{
		m_particleSystem.Play ();
		FMOD_StudioSystem.instance.PlayOneShot("event:/SFX/Gadgets/Rocket/gadgetRocketSuccess", transform.position);
	}
}

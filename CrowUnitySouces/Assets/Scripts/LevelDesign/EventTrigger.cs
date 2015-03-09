using UnityEngine;
using System.Collections;
using UnityEngine.Events;

public class EventTrigger : MonoBehaviour {

	public string _triggererName = "Body";
	public UnityEvent _event;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.name == _triggererName) {
			_event.Invoke ();
			collider.enabled = false;
		}
	}
}

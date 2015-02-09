using UnityEngine;
using System.Collections;
using UnityEngine.Events;

public class EventTrigger : MonoBehaviour {

	public string _triggererTag;
	public UnityEvent _event;
	public bool _fireOnce = true;
	private bool _firedOnce = false;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter(Collider other)
	{
		if(!other.gameObject.CompareTag(_triggererTag))
			return;

		if(_fireOnce && _firedOnce == true)
			return;

		Debug.Log(other.gameObject.name);

		_event.Invoke();

		_firedOnce = false;
	}
}

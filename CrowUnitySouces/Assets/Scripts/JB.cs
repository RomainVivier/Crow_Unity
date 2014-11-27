using UnityEngine;
using System.Collections;

public class JB : MonoBehaviour {

    private bool active;

	// Use this for initialization
	void Start ()
    {
        active = false;
	}
	
	// Update is called once per frame
	void Update ()
    {
	
	}

    public void activate()
    {
        active = true;
    }
}

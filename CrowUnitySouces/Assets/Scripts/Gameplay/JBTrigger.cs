using UnityEngine;
using System.Collections;

public class JBTrigger : MonoBehaviour
{

	// Use this for initialization
	void Start ()
    {
	
	}
	
	// Update is called once per frame
	void Update ()
    {
	
	}

    void OnCollisionEnter(Collision collision)
    {
        GameObject oth = collision.collider.transform.root.gameObject;
        if (oth.name == "Car")
        {
            if(GameObject.Find("JB").GetComponent<JB>().activate()==false)
                oth.GetComponentInChildren<RailsControl>().setSpeedKmh *= 0.7f;
        }
        else Debug.Log(oth.name);
    }
}

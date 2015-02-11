using UnityEngine;
using System.Collections;

public class CarCollisionsInhibiter : MonoBehaviour {

    public int _nbCol=0;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	}

    void OnTriggerEnter(Collider other)
    {
        GameObject oth = other.gameObject;
        if (oth.name == "Obstacle_Car(Clone)")
        {
            _nbCol++;
            CarCollisionsHandler._dontCollide = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        GameObject oth = other.gameObject;
        if (oth.name == "Obstacle_Car(Clone)")
        {
            _nbCol--;
            CarCollisionsHandler._dontCollide = (_nbCol>0);
        }
    }


}

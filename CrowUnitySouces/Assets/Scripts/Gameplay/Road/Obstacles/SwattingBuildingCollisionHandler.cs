using UnityEngine;
using System.Collections;

public class SwattingBuildingCollisionHandler : MonoBehaviour {

    public SwattingBuilding _swattingBuilding;

	void Start ()
    {
	
	}
	
	void Update ()
    {
	
	}

    void OnCollisionEnter(Collision collision)
    {
        _swattingBuilding.handleCollision(collision);
    }
}

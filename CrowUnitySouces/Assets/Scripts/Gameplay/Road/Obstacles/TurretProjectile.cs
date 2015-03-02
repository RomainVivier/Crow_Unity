using UnityEngine;
using System.Collections;

public class TurretProjectile : MonoBehaviour {

    public float _speed;
	
	void Update ()
    {
        transform.position -= Vector3.right * Time.deltaTime * _speed;
	}
}

using UnityEngine;
using System.Collections;

public abstract class Obstacle : MonoBehaviour {

    public GameObject _prefab;

    void Start()
    {
	}
	
	void Update()
    {
	}

    public virtual void Behaviour()
    {
    }
}

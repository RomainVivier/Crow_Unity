using UnityEngine;
using System.Collections;

public class Burger : MonoBehaviour
{
    public Vector3 _direction;
    public float _speed;
	public ParticleSystem _ps; 

    void Update()
    {
        transform.position += _direction * Time.deltaTime * _speed;
    }

	public void Bounce()
	{
		_ps.Stop();
		_ps.Play ();
	}

	void OnTriggerEnter(Collider other)
	{
		if (other.collider.name == "SpoonBill")
		{
			Debug.Log("prout");
			Destroy(GetComponent<Animator>());
			Destroy(this);
		}

	}
}
	
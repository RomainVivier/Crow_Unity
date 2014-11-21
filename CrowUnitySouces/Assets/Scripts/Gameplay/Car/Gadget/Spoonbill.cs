using UnityEngine;
using System.Collections;

public class Spoonbill : Gadget {

    
	public Animator _animator;
	public GameObject _spoonbill;
    public float _spoonbillForce;

    private Timer m_timer;

    void Start()
    {
        GadgetManager.Instance.Register("Spoonbill", this);
        m_timer = new Timer();
        gameObject.SetActive(false);
    }

	public override void Play ()
	{
		base.Play ();
        Debug.Log("play !");
        gameObject.SetActive(true);
	}

    public override void Stop()
    {
        base.Stop();
        gameObject.SetActive(false);
    }

    void Update()
    {
        if (m_timer.IsElapsedOnce)
        {
            Stop();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.collider.CompareTag("Obstacle"))
        {
            other.rigidbody.AddForce(Vector3.up * _spoonbillForce);
            _animator.SetTrigger("Flip");
            m_timer.Reset(2f);
        }
    }

}

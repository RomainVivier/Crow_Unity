using UnityEngine;
using System.Collections;
using FMOD.Studio;

public class Burger : MonoBehaviour
{
    public Vector3 _direction;
    public float _speed;
	public ParticleSystem _ps; 
	
	private FMOD.Studio.EventInstance m_sound;
	
	void Start()
	{
		m_sound=FMOD_StudioSystem.instance.GetEvent("event:/SFX/Obstacles/Burger/obsBurgerRumble");
		m_sound.start();
	}
	
	void OnDestroy()
	{
		m_sound.release();
	}
	
    void Update()
    {
        transform.position += _direction * Time.deltaTime * _speed;
        
		_3D_ATTRIBUTES threeDeeAttr = new _3D_ATTRIBUTES();
		threeDeeAttr.position = UnityUtil.toFMODVector(transform.position);
		threeDeeAttr.up = UnityUtil.toFMODVector(transform.up);
		threeDeeAttr.forward = UnityUtil.toFMODVector(transform.forward);
		threeDeeAttr.velocity = UnityUtil.toFMODVector(Vector3.zero);
		m_sound.set3DAttributes(threeDeeAttr);
    }

	public void Bounce()
	{
		_ps.Stop();
		_ps.Play ();
	}

	void OnTriggerEnter(Collider other)
	{
		Car car = other.collider.gameObject.transform.parent.GetComponent<Car> ();

		if (other.collider.name == "SpoonBill")
		{
			Destroy(GetComponent<Animator>());
			Destroy(this);
		}
		else if( car != null)
		{
			Destroy (this);
		}
	}
}
	
using UnityEngine;
using System.Collections;

public class Dash : Gadget
{
    #region Members
    public RailsControl _rc;
    public Car _car;
    public float _speedCoeff;
	public WindshieldController _windshield;

	const float COOLDOWN = 4.0f;

    private Timer m_timer;

    #endregion

    #region MonoBehaviour

    public override void Awake()
    {
        GadgetManager.Instance.Register("Dash", this);
        m_timer = new Timer();
        base.Awake();
    }

    public override void Update()
    {
        if (m_timer.IsElapsedOnce)
        {
            Stop();
        }
        base.Update();
    }

    #endregion

    public override void Play()
    {
        base.Play();
		FMOD_StudioSystem.instance.PlayOneShot("event:/SFX/Gadgets/Boost/gadgetBoostExecute",transform.position);
        _rc.setSpeedKmh *= _speedCoeff;
        //_car.InstantSetSpeedKmh(_rc.setSpeedKmh);
        _car.gameObject.GetComponent<PolynomialEngine>().maxPowerKw *= 50;
        _car.gameObject.GetComponent<PolynomialEngine>().powerMinRpmKw*= 50;
        _car.gameObject.GetComponent<Transmission>().lockGear(4);
        //_car.gameObject.transform.FindChild("Body/CenterOfMass").localPosition += new Vector3(0, -0.25f, 0);
        _car.maxSpeedKmh *= _speedCoeff;
        _car.updateValues();
        m_timer.Reset(1f);
        IsReady = false;
		//GetComponentInChildren<SpriteRenderer> ().enabled = true;
		GetComponentInChildren<Animator> ().SetTrigger ("HornsOut");
		_windshield._isInvincible = true;
    }

    public override void Stop()
    {
        _rc.setSpeedKmh /= _speedCoeff;
        _car.gameObject.GetComponent<PolynomialEngine>().maxPowerKw /= 50;
        _car.gameObject.GetComponent<PolynomialEngine>().powerMinRpmKw/= 50;
        _car.gameObject.GetComponent<Transmission>().lockGear(1);
        _car.gameObject.GetComponent<Transmission>().lockGear(-1);
        //_car.gameObject.transform.FindChild("Body/CenterOfMass").localPosition -= new Vector3(0, -0.25f, 0);
        _car.maxSpeedKmh /= _speedCoeff;
        _car.updateValues();
        

		Invoke("StopInvincibility", 0.5f);
		Invoke("StopBase", COOLDOWN);
    }

	void StopBase()
	{
		IsReady = true;
		base.Stop();
	}


	void StopInvincibility()
	{
		//GetComponentInChildren<SpriteRenderer> ().enabled = false;
		GetComponentInChildren<Animator> ().SetTrigger ("HornsIn");
		_windshield._isInvincible = false;
	}
}

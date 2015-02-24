using UnityEngine;
using System.Collections;

public class Dash : Gadget
{
    #region Members
    public RailsControl _rc;
    public Car _car;
    public float _speedCoeff;

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
        _car.gameObject.GetComponent<PolynomialEngine>().maxPowerKw *= 100;
        _car.gameObject.GetComponent<PolynomialEngine>().powerMinRpmKw*= 100;
        if(!_car.isSteering()) _car.transform.FindChild("Body").GetComponent<Rigidbody>().AddRelativeTorque(-10000, 0, 0, ForceMode.Impulse);
        _car.gameObject.GetComponent<Transmission>().lockGear(4);
        //_car.gameObject.transform.FindChild("Body/CenterOfMass").localPosition += new Vector3(0, -0.25f, 0);
        _car.maxSpeedKmh *= _speedCoeff;
        _car.updateValues();
        m_timer.Reset(1f);
        IsReady = false;
    }

    public override void Stop()
    {
        base.Stop();

        _rc.setSpeedKmh /= _speedCoeff;
        _car.gameObject.GetComponent<PolynomialEngine>().maxPowerKw /= 100;
        _car.gameObject.GetComponent<PolynomialEngine>().powerMinRpmKw/= 100;
        _car.gameObject.GetComponent<Transmission>().lockGear(1);
        _car.gameObject.GetComponent<Transmission>().lockGear(-1);
        //_car.gameObject.transform.FindChild("Body/CenterOfMass").localPosition -= new Vector3(0, -0.25f, 0);
        _car.maxSpeedKmh /= _speedCoeff;
        _car.updateValues();
        if (_car.getForwardVelocityKmh() > _car.maxSpeedKmh) _car.InstantSetSpeedKmh(_car.maxSpeedKmh);
        IsReady = true;
	
    }
}

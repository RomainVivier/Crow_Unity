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

    public override void Start()
    {
        GadgetManager.Instance.Register("Dash", this);
        m_timer = new Timer();
        base.Start();
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
        Debug.Log("dashing bitch !");
        _rc.setSpeedKmh *= _speedCoeff;
        //_car.InstantSetSpeedKmh(_rc.setSpeedKmh);
        _car.gameObject.GetComponent<PolynomialEngine>().maxPowerKw *= 1000;
        _car.gameObject.GetComponent<PolynomialEngine>().powerMinRpmKw*= 1000;
        _car.updateValues();
        m_timer.Reset(1f);
        IsReady = false;
    }

    public override void Stop()
    {
        base.Stop();

        _rc.setSpeedKmh /= _speedCoeff;
        _car.gameObject.GetComponent<PolynomialEngine>().maxPowerKw /= 1000;
        _car.gameObject.GetComponent<PolynomialEngine>().powerMinRpmKw/= 1000;
        _car.updateValues();
        IsReady = true;
    }
}

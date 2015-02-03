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
        _rc.setSpeedKmh *= _speedCoeff;
        _car.InstantSetSpeedKmh(_rc.setSpeedKmh);
        m_timer.Reset(1f);
        IsReady = false;
    }

    public override void Stop()
    {
        base.Stop();

        _rc.setSpeedKmh /= _speedCoeff;
        IsReady = true;
    }
}

using UnityEngine;
using System.Collections;

public class Dash : Gadget
{
    #region Members
    public RailsControl _rc;
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
        m_timer.Reset(1f);
    }

    public override void Stop()
    {
        base.Stop();

        _rc.setSpeedKmh /= _speedCoeff;
    }
}

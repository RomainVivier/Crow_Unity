using UnityEngine;
using System.Collections;

public class Laser : ButtonGadget
{

    #region constants
    const float COOLDOWN_TIME=10f;
    const float VALVE_OPENING_TIME = 0.2f;
    const float FIRING_TIME = 1.5f;
    const float VALVE_CLOSING_TIME = 0.2f;

    const float startAngleDeg = -10f;
    const float endAngleDeg = 10f;
    #endregion

    #region members
    private Timer m_cooldownTimer;
    private Timer m_stateTimer;

    private enum State
    {
        READY,
        VALVE_OPENING,
        FIRING,
        VALVE_CLOSING,
        COOLDOWN
    };

    State m_state=State.READY;
    #endregion

    #region methods
	void Start () {
        m_cooldownTimer = new Timer(0.01f);
        m_stateTimer = new Timer();
        GadgetManager.Instance.Register("Laser", this);
        base.Start();
	}
	
	void Update () {
        switch(m_state)
        {
            case State.READY:
                break;
            case State.VALVE_OPENING:
                if (m_stateTimer.IsElapsedOnce)
                {
                    transform.FindChild("ValvesPivot").localEulerAngles=new Vector3(-180, 0, 0);
                    m_state = State.FIRING;
                    m_stateTimer.Reset(FIRING_TIME);
                }
                else transform.FindChild("ValvesPivot").localEulerAngles=new Vector3(-180+m_stateTimer.CurrentNormalized * 180,0,0);
                break;
            case State.COOLDOWN:
                if (m_cooldownTimer.IsElapsedOnce)
                {
                    IsReady = true;
                    m_state = State.READY;
                    Stop();
                }
                break;
            case State.FIRING:
                // TODO : I'M FIRING MY LASER
                if(m_stateTimer.IsElapsedOnce)
                {
                    m_state = State.VALVE_CLOSING;
                    m_stateTimer.Reset(VALVE_CLOSING_TIME);
                }
                break;
            case State.VALVE_CLOSING:
                if (m_stateTimer.IsElapsedOnce)
                {
                    transform.FindChild("ValvesPivot").localEulerAngles=new Vector3(0, 0, 0);
                    m_state = State.COOLDOWN;
                }
                else transform.FindChild("ValvesPivot").localEulerAngles=new Vector3(m_stateTimer.CurrentNormalized*-180,0,0);
                break;
        }
        base.Update();
    }

    public override void Play()
    {
        if(m_state!=State.READY)
        {
            Debug.LogError("Error : gadget laser is not ready");
        }
        else
        {
            IsReady = false;
            m_cooldownTimer.Reset(COOLDOWN_TIME);
            m_state = State.VALVE_OPENING;
            m_stateTimer.Reset(VALVE_OPENING_TIME);
        }
        base.Play();
    }

    #endregion
}

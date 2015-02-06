using UnityEngine;
using System.Collections;

public class DiscoBall : Gadget
{

    #region constants
    const float ACTIVATION_TIME = 0.5f;
    const float ACTIVATION_LEVER_COOLDOWN = 3f;
    const float DISACTIVATION_TIME = 0.5f;
    const float DISACTIVAION_LEVER_COOLDOWN = 3f;
    #endregion

    #region members
    private enum State
    {
        READY,
        ACTIVATING,
        ACTIVE,
        DISACTIVATING,
        COOLDOWN
    };
    
    private Timer m_cooldownTimer;
    private Timer m_stateTimer;
    private State m_state;
    private Animator m_animator;
    #endregion

    #region methods
    void Start ()
    {
        m_cooldownTimer = new Timer(0.01f);
        m_stateTimer = new Timer(0.01f);
        m_state = State.READY;
        m_animator = transform.FindChild("Ball").GetComponent<Animator>();
        m_animator.SetBool("activated", false);
    }
	
	void Update ()
    {
	    switch(m_state)
        {
            case State.READY:
                break;
            case State.ACTIVATING:
                if(m_stateTimer.IsElapsedOnce)
                {
                    m_state = State.ACTIVE;
                }
                break;
            case State.ACTIVE:

                break;
        }
	}

    public override void Play()
    {
        if(m_state!=State.READY)
        {
            Debug.LogError("Error : gadget discoball is not ready");
        }
        else
        {
            IsReady = false;
            m_cooldownTimer.Reset(ACTIVATION_LEVER_COOLDOWN);
            m_state = State.ACTIVATING;
            m_stateTimer.Reset(ACTIVATION_TIME);
            m_animator.SetBool("activated", true);
        }
        base.Play();
    }
    #endregion
}

using UnityEngine;
using System.Collections;

public class Spoonbill : Gadget
{

    #region Members

	public GameObject _spoonbill;
    public float _spoonbillForce;
    public Animator _flipflopAnimator;
    public Animator _spoonbillAnimator;


    private Timer m_attackTimer;
    private Timer m_engageTimer;

    enum State
    {
        Engaged,
        Engaging,
        Disengaged,
        Disengaging,
        Attacking
    }

    private State m_state;
    private GameObject m_target;

    #endregion 

    #region MonoBehaviour

    void Start()
    {
        GadgetManager.Instance.Register("Spoonbill", this);
        m_attackTimer = new Timer();
        m_engageTimer = new Timer();
        gameObject.SetActive(false);
        IsReady = true;
        m_state = State.Disengaged;
    }

    void Update()
    {
        if(m_target != null && m_state == State.Attacking && m_attackTimer.CurrentNormalized < 0.5)
        {
            m_target.transform.parent = null;
            m_target = null;
        }

        if (m_attackTimer.IsElapsedOnce)
        {
            Disengage();
        }

        if(m_engageTimer.IsElapsedOnce)
        {
            m_state = m_state == State.Engaging ? State.Engaged : State.Disengaged;
        }

        if (m_state == State.Disengaged)
        {
            Stop();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.collider.CompareTag("Obstacle") && m_state != State.Attacking)
        {
            other.transform.parent = _spoonbill.transform;
            m_target = other.gameObject;
            _spoonbillAnimator.SetTrigger("Attack");
            m_attackTimer.Reset(1.5f);
            m_state = State.Attacking;
        }
    }

    #endregion

    #region Overrided Functions

    public override void Play ()
	{
		base.Play ();

        switch (m_state)
        {
            case State.Disengaged :
                gameObject.SetActive(true);
                m_state = State.Engaging;
                m_engageTimer.Reset(1f);
                _spoonbillAnimator.SetTrigger("Engage");
                _flipflopAnimator.SetBool("Engage", true);
                break;

            case State.Engaged :
                Disengage();
                break;
        }

	}

    void Disengage()
    {
        m_state = State.Disengaging;
        m_engageTimer.Reset(1f);
        _spoonbillAnimator.SetTrigger("Disengage");
        _flipflopAnimator.SetBool("Engage", false);
    }

    public override void Stop()
    {
        base.Stop();
        gameObject.SetActive(false);
        _flipflopAnimator.SetBool("Engage", false);
    }

    #endregion

}

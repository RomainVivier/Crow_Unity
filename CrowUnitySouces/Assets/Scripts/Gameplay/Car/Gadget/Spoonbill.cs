using UnityEngine;
using System.Collections;

public class Spoonbill : Gadget
{

    #region Members

	public GameObject _spoonbill;
    public float _spoonbillForce;
    public Animator _spoonbillAnimator;

    private Timer m_attackTimer;
    private Timer m_engageTimer;
	private GameObject m_spatulaMesh;
	private float m_disengageTime=1;
	
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
    private bool inhibitedCollisions = false;

    #endregion 

    #region MonoBehaviour

    public override void Awake()
    {
        GadgetManager.Instance.Register("Spoonbill", this);
        m_attackTimer = new Timer();
        m_engageTimer = new Timer();
        gameObject.SetActive(false);
        IsReady = true;
        m_state = State.Disengaged;
        m_spatulaMesh=transform.Find("spatula_interior").gameObject;
        base.Awake();
    }

    public override void Update()
    {
        if(m_target != null && m_state == State.Attacking && m_attackTimer.CurrentNormalized < 0.01)
        {
            m_target.rigidbody.isKinematic = false;
            m_target.transform.parent = null;
            m_target = null;
        }

        if (m_attackTimer.IsElapsedOnce)
        {
            Disengage();
        }

        if(m_engageTimer.IsElapsedOnce && m_state != State.Attacking)
        {
            m_state = m_state == State.Engaging ? State.Engaged : State.Disengaged;
			if (m_state == State.Disengaged)
			{
				Stop();
			}
		}
		base.Update();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.collider.CompareTag("Obstacle") && (m_state == State.Engaged || m_state == State.Engaging) && other.rigidbody!=null)
        {
            //(GameObject.FindObjectOfType<CarCollisionsInhibiter>() as CarCollisionsInhibiter)._nbCol++;
			Transform parent=other.transform.parent;
			Obstacle obstacle= parent==null ? null : parent.GetComponent<Obstacle>();
			if(obstacle!=null) obstacle.PlayDestructionSound();
		
		    inhibitedCollisions = true;
            other.transform.parent = _spoonbill.transform;
            m_target = other.gameObject;
            other.rigidbody.isKinematic = true;
            FMOD_StudioSystem.instance.PlayOneShot("event:/SFX/Gadgets/Spatula/gadgetSpatulaExecute", transform.position);
            _spoonbillAnimator.SetTrigger("Attack");
            m_attackTimer.Reset(0.76f);//0.56f);
            m_disengageTime=0;
            m_state = State.Attacking;
            addScore(Score.ScoreType.MINOR_OBSTACLE);
//            DialogsManager._instance.triggerEvent(DialogsManager.DialogInfos.EventType.OBSTACLE_DESTRUCTION, other.gameObject.name);
//            DialogsManager._instance.triggerEvent(DialogsManager.DialogInfos.EventType.DESTRUCTION_WITH_GADGET, "SpoonBill");
        }
        if(!other.isTrigger) FMOD_StudioSystem.instance.PlayOneShot("event:/SFX/Gadgets/Spatula/gadgetSpatulaImpact", transform.position);
    }

    #endregion

    #region Overrided Functions

    public override void Play ()
	{
		base.Play ();
        _invertGesture = true;

        switch (m_state)
        {
            case State.Disengaged :
                FMOD_StudioSystem.instance.PlayOneShot("event:/SFX/Gadgets/Spatula/gadgetSpatulaEngage", transform.position);
				gameObject.SetActive(true);
				m_spatulaMesh.SetActive(true);
				collider.enabled=true;
			
                m_state = State.Engaging;
                m_engageTimer.Reset(1f);
                //transform.FindChild("Vignette").GetComponent<Vignette>().pop(1f);

                _spoonbillAnimator.SetTrigger("Engage");
                _spoonbillAnimator.ResetTrigger("Disengage");
                _buttonAnim.SetBool("Engage", true);
                m_disengageTime=1;
                break;

            case State.Engaged :
                Disengage();
                break;
        }

	}

    void Disengage()
    {
        /*if (inhibitedCollisions)
        {
            (GameObject.FindObjectOfType<CarCollisionsInhibiter>() as CarCollisionsInhibiter)._nbCol--;
            inhibitedCollisions = false;
        }*/
        FMOD_StudioSystem.instance.PlayOneShot("event:/SFX/Gadgets/Spatula/gadgetSpatulaDisengage", transform.position);
        m_state = State.Disengaging;
        m_engageTimer.Reset(m_disengageTime);
        _spoonbillAnimator.SetTrigger("Disengage");
        //_buttonAnim.SetBool("Engage", false);
        _invertGesture = false;
    }

    public override void Stop()
    {
		IsReady=false;
		m_spatulaMesh.SetActive(false);
		collider.enabled=false;
		//gameObject.SetActive(false);
		//_buttonAnim.SetBool("Engage", false);
		base.Stop();
    }

    #endregion

}

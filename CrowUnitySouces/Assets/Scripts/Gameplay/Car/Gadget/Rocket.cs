using UnityEngine;
using System.Collections;

public class Rocket : Gadget {


    #region Members

    public float _blastRadius;
    public float _rocketSpeed;
    public float _rocketUIMax;

    enum State
    {
        Idle,
        Launching,
        Engaged
    }

    private Timer m_rocketLaunchtimer;
    private Vector3 m_offsetWithParent;
    private Transform m_carTransform;
    private GameObject m_explosionParticles;
    public RailsControl m_railsControl;

    private State m_state;
    private Rails m_rails;
    private float m_railsIndex;
    private float m_railsProgress;
    private float m_railsSpeed;
    private float m_failProgress;
    private Timer m_cooldownTimer;
    private Obstacle m_target;


    private GameObject m_rocketObject;
    private FMOD.Studio.EventInstance m_rocketUI;
    private FMOD.Studio.ParameterInstance m_rocketDist;
    private FMOD.Studio.EventInstance m_rocketExecute3D;
    private FMOD.Studio.ParameterInstance m_rocketExecute3DDist;
    private FMOD.Studio.EventInstance m_blowInstance;

    public FMOD_StudioEventEmitter _rocketExecute;

    #endregion

    #region MonoBehaviour

    public override void Awake()
    {
        m_offsetWithParent = transform.localPosition;

        m_rocketUI= FMOD_StudioSystem.instance.GetEvent("event:/SFX/Gadgets/Rocket/gadgetRocketUI");
        m_rocketUI.getParameter("distToTarget", out m_rocketDist);

        m_rocketExecute3D = FMOD_StudioSystem.instance.GetEvent("event:/SFX/Gadgets/Rocket/gadgetRocketExecute3D");
        m_rocketExecute3D.getParameter("Distance", out m_rocketExecute3DDist);
        
        GadgetManager.Instance.Register("Rocket", this);
        
        m_rocketLaunchtimer = new Timer();
        m_rocketObject = transform.Find("Object").gameObject;
        m_rocketObject.gameObject.SetActive(false);
        m_explosionParticles = GameObject.Find("RocketExplosion");

        m_carTransform = transform.parent.parent;
        m_railsControl = FindObjectOfType<RailsControl>();

        base.Awake();
    }

    public override void Update()
    {
        switch(m_state)
        {
            case State.Launching :

                if (m_rocketLaunchtimer.IsElapsedOnce)
                {
                    Launch();
                }
                else if (!m_rocketLaunchtimer.IsElapsedLoop && m_rocketLaunchtimer.Current > 0.5f)
                {
                    _rocketExecute.Play();
                    m_rocketExecute3D.start();
                }

                break;
            case State.Engaged :

                UpdateProgress();

                UpdateSound();

                break;
        }

        base.Update();
    }

    void UpdateProgress()
    {
        //Update rails progression
        m_railsProgress += m_railsSpeed * Time.fixedDeltaTime;
        m_failProgress += _rocketSpeed * Time.deltaTime; 

        if(m_railsProgress > 1f && ( (m_target != null && m_rails != m_target.Rails) || m_target == null ) )
        {
            Rails newRails = m_rails.GetComponent<RoadChunk>().NextChunk._rails;
            m_railsProgress = (m_railsProgress - 1) * (m_rails.Dist / newRails.Dist);
            m_rails = newRails;
            m_railsSpeed = _rocketSpeed / m_rails.Dist;
        }

        if (m_target == null)
        {
            if (m_failProgress < 400f)
            {
                m_rocketObject.transform.position = m_rails.getPoint(m_railsIndex, m_rails.correct2Incorrect(m_railsProgress)) + Vector3.Scale(Vector3.up, m_offsetWithParent);
            }
            else
            {
                Blow();
            }
        } 
        else
        {
            //Debug.Log(m_railsProgress + " " + m_target.RailsProgress);
            if (m_rails == m_target.Rails && m_railsProgress >= m_target.RailsProgress)
            {
                Blow();
            }
            else
            {
                m_rocketObject.transform.position = m_rails.getPoint(m_railsIndex, m_rails.correct2Incorrect(m_railsProgress)) + Vector3.Scale(Vector3.up, m_offsetWithParent);
            }
        }

    }

    void UpdateSound()
    {

        if(m_target != null)
        {
            m_rocketDist.setValue(Mathf.Clamp((Vector3.Distance(m_rocketObject.transform.position, m_target.transform.position) / _rocketUIMax), 0f, 1f));
        }

        // Set gadgetRocketExecte3D 3D attributesFMOD.Studio._3D_ATTRIBUTES threeDeeAttr = new FMOD.Studio._3D_ATTRIBUTES();
        FMOD.Studio._3D_ATTRIBUTES threeDeeAttr = new FMOD.Studio._3D_ATTRIBUTES();
        threeDeeAttr.position = FMOD.Studio.UnityUtil.toFMODVector(m_rocketObject.transform.position);
        threeDeeAttr.up = FMOD.Studio.UnityUtil.toFMODVector(m_rocketObject.transform.up);
        threeDeeAttr.forward = FMOD.Studio.UnityUtil.toFMODVector(m_rocketObject.transform.forward);
        threeDeeAttr.velocity = FMOD.Studio.UnityUtil.toFMODVector(Vector3.zero);
        m_rocketExecute3D.set3DAttributes(threeDeeAttr);

        float dist = (m_rocketObject.transform.position - m_carTransform.position).magnitude;
        dist = Mathf.Clamp(dist, 10, 200);
        m_rocketExecute3DDist.setValue(dist);
    }

    #endregion

    #region Overrided Functions

    public override void Play()
    {
        base.Play();
        m_rocketObject.gameObject.SetActive(true);
        IsReady = false;

        var obstacles = GameObject.FindGameObjectsWithTag("Obstacle");
        if (obstacles.Length > 0)
        {
            foreach (GameObject go in obstacles)
            {
                if (go.transform.position.x - m_rocketObject.transform.position.x > 20  && (m_target == null || Vector3.Distance(m_rocketObject.transform.position, m_target.transform.position) > Vector3.Distance(m_rocketObject.transform.position, go.transform.position)))
                {
                    if(go.GetComponent<Obstacle>()!=null) m_target = go.GetComponent<Obstacle>();
                }
            }
        }


        if (obstacles.Length == 0 || m_target == null || Vector3.Distance(m_rocketObject.transform.position, m_target.transform.position) > 400) 
        {
            //TODO SET A FAKE Obstacle
            //m_target = transform.position + transform.forward * 100;
            m_target = null;
        }

        m_rails = m_railsControl.Rails;
        m_railsProgress = m_railsControl.Progress;
        m_railsIndex = m_target == null ? m_railsControl.currentRail : m_target.RailsIndex;
        m_state = State.Launching;
        m_failProgress = 0f;

        m_rocketLaunchtimer.Reset(0.6f);
        FMOD_StudioSystem.instance.PlayOneShot("event:/SFX/Gadgets/Rocket/gadgetRocketEngage", m_rocketObject.transform.position);

    }

    public override void Stop()
    {
        base.Stop();
        m_rocketObject.transform.localPosition = m_offsetWithParent;
        m_state = State.Idle;
        m_rocketObject.gameObject.SetActive(false);
        //IsReady = true;
    }

    #endregion

    #region Rocket Functions

    void Launch()
    {
        m_rocketUI.start();
        m_railsSpeed = _rocketSpeed /  m_rails.Dist;
        m_state = State.Engaged;
    }

    void Blow()
    {
        //play visual effect
        m_rocketUI.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        m_rocketExecute3D.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        FMOD.Studio.EventInstance m_blowInstance
            = FMOD_StudioSystem.instance.GetEvent("event:/SFX/Gadgets/Rocket/gadgetRocketSuccess");
        m_blowInstance.start();
        FMOD.Studio.ParameterInstance param;
        m_blowInstance.getParameter("Position", out param);
        Vector3 dpos = m_rocketObject.transform.position - transform.parent.parent.position;
        Vector3 forward = transform.parent.parent.forward;
        float position = Vector3.Dot(dpos, forward);
        param.setValue(position > 0 ? 0 : 1);
        FMOD.Studio._3D_ATTRIBUTES threeDeeAttr = new FMOD.Studio._3D_ATTRIBUTES();
        threeDeeAttr.position = FMOD.Studio.UnityUtil.toFMODVector(m_rocketObject.transform.position);
        threeDeeAttr.up = FMOD.Studio.UnityUtil.toFMODVector(m_rocketObject.transform.up);
        threeDeeAttr.forward = FMOD.Studio.UnityUtil.toFMODVector(m_rocketObject.transform.forward);
        threeDeeAttr.velocity = FMOD.Studio.UnityUtil.toFMODVector(Vector3.zero);
        m_blowInstance.set3DAttributes(threeDeeAttr);

        m_explosionParticles.transform.position = m_target == null ? m_rocketObject.transform.position : m_target.transform.position;
        m_explosionParticles.GetComponent<ParticleSystem>().Play();
        var colliders = Physics.OverlapSphere(m_rocketObject.transform.position, _blastRadius);
        m_target = null;
        foreach(Collider collider in colliders)
        {
            if (collider.CompareTag("Obstacle"))
            {
                collider.gameObject.SetActive(false);
                addScore();
                DialogsManager._instance.triggerEvent(DialogsManager.DialogInfos.EventType.OBSTACLE_DESTRUCTION, collider.gameObject.name);
                DialogsManager._instance.triggerEvent(DialogsManager.DialogInfos.EventType.DESTRUCTION_WITH_GADGET, "Rocket");
            }
        }

        Stop();
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        //Gizmos.DrawWireSphere(m_rocketObject.transform.position, _blastRadius);
    }

    #endregion 
}

using UnityEngine;
using System.Collections;

public class Rocket : ButtonGadget {


    #region Members

    public float _rocketForce;
    public float _blastRadius;
    public float _rocketSpeed;
    public float _targetMaxDistance;
    public float _rocketUIMax;
    public GameObject _explosionParticles;

    private Timer m_rocketLaunchtimer;
    private Timer m_timer;
    private Vector3 m_offsetWithParent;
    private Vector3 m_startPosition;
    private Vector3 m_target;
    private Transform m_carTransform;

    private FMOD.Studio.EventInstance m_rocketUI;
    private FMOD.Studio.ParameterInstance m_rocketDist;
    private FMOD.Studio.EventInstance m_rocketExecute3D;
    private FMOD.Studio.ParameterInstance m_rocketExecute3DDist;
    private FMOD.Studio.EventInstance m_blowInstance;

    public FMOD_StudioEventEmitter _rocketExecute;

    #endregion

    #region MonoBehaviour

    public override void Start()
    {
        m_offsetWithParent = transform.localPosition;
        m_rocketUI= FMOD_StudioSystem.instance.GetEvent("event:/SFX/Gadgets/Rocket/gadgetRocketUI");
        m_rocketUI.getParameter("distToTarget", out m_rocketDist);

        m_rocketExecute3D = FMOD_StudioSystem.instance.GetEvent("event:/SFX/Gadgets/Rocket/gadgetRocketExecute3D");
        m_rocketExecute3D.getParameter("Distance", out m_rocketExecute3DDist);
        
        GadgetManager.Instance.Register("Rocket", this);
        m_rocketLaunchtimer = new Timer();
        m_timer = new Timer();
        gameObject.SetActive(false);
        m_target = Vector3.zero;
        _explosionParticles = GameObject.Find("RocketExplosion");

        m_carTransform = transform.parent.parent;
        base.Start();
    }

    public override void Update()
    {
        if (m_rocketLaunchtimer.IsElapsedOnce)
        {
            Launch();
        }

        if(!m_rocketLaunchtimer.IsElapsedLoop && m_rocketLaunchtimer.Current > 0.5f)
        {
            _rocketExecute.Play();
            m_rocketExecute3D.start();
        }


        if (m_timer.IsElapsedOnce)
        {
            Blow();
            transform.localPosition = m_offsetWithParent;
            Stop();
        }

        if(!m_timer.IsElapsedLoop)
        {
            transform.position = Vector3.Lerp(m_startPosition, m_target, 1 - m_timer.CurrentNormalized);
            m_rocketDist.setValue(Mathf.Clamp((Vector3.Distance(transform.position, m_target) / _rocketUIMax), 0f, 1f));
        }

        // Set gadgetRocketExecte3D 3D attributesFMOD.Studio._3D_ATTRIBUTES threeDeeAttr = new FMOD.Studio._3D_ATTRIBUTES();
        FMOD.Studio._3D_ATTRIBUTES threeDeeAttr = new FMOD.Studio._3D_ATTRIBUTES();
        threeDeeAttr.position = FMOD.Studio.UnityUtil.toFMODVector(transform.position);
        threeDeeAttr.up = FMOD.Studio.UnityUtil.toFMODVector(transform.up);
        threeDeeAttr.forward = FMOD.Studio.UnityUtil.toFMODVector(transform.forward);
        threeDeeAttr.velocity = FMOD.Studio.UnityUtil.toFMODVector(Vector3.zero);
        m_rocketExecute3D.set3DAttributes(threeDeeAttr);

        float dist = (transform.position - m_carTransform.position).magnitude;
        dist = Mathf.Clamp(dist, 10, 200);
        m_rocketExecute3DDist.setValue(dist);

        base.Update();
    }

    #endregion

    #region Overrided Functions

    public override void Play()
    {
        base.Play();
        gameObject.SetActive(true);
        IsReady = false;

        var obstacles = GameObject.FindGameObjectsWithTag("Obstacle");
        if (obstacles.Length > 0)
        {
            foreach (GameObject go in obstacles)
            {
                if (go.transform.position.x > transform.position.x && (m_target == Vector3.zero || Vector3.Distance(transform.position, m_target) > Vector3.Distance(transform.position, go.transform.position)))
                {
                    m_target = go.transform.position;
                    FMOD_StudioSystem.instance.PlayOneShot("event:/SFX/Gadgets/Rocket/gadgetRocketEngage", transform.position);
                    m_rocketLaunchtimer.Reset(0.6f);
                }
            }
        }

        if (obstacles.Length == 0 || m_target == Vector3.zero)
        {
            Stop();
        }

        

    }

    public override void Stop()
    {
        base.Stop();
        gameObject.SetActive(false);
        IsReady = true;
    }

    #endregion

    #region Rocket Functions

    void Launch()
    {
        //_rocketExecute.Play();
        m_rocketUI.start();
        m_startPosition = transform.position;
        m_target.y = m_startPosition.y;

        if (Vector3.Distance(m_startPosition, m_target) > _targetMaxDistance)
        {
            m_target = Vector3.zero;
            Stop();
        }
        else
        {
            m_timer.Reset(Vector3.Distance(m_startPosition, m_target) / _rocketSpeed);
        }
    }

    void Blow()
    {
        //play visual effect
        m_rocketUI.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        m_rocketExecute3D.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);    
        //FMOD_StudioSystem.instance.PlayOneShot("event:/SFX/Gadgets/Rocket/gadgetRocketSuccess", transform.position);
        FMOD.Studio.EventInstance m_blowInstance
            = FMOD_StudioSystem.instance.GetEvent("event:/SFX/Gadgets/Rocket/gadgetRocketSuccess");
        m_blowInstance.start();
        FMOD.Studio.ParameterInstance param;
        m_blowInstance.getParameter("Position", out param);
        Vector3 dpos = transform.position - transform.parent.parent.position;
        Vector3 forward = transform.parent.parent.forward;
        float position = Vector3.Dot(dpos, forward);
        param.setValue(position > 0 ? 0 : 1);
        FMOD.Studio._3D_ATTRIBUTES threeDeeAttr = new FMOD.Studio._3D_ATTRIBUTES();
        threeDeeAttr.position = FMOD.Studio.UnityUtil.toFMODVector(transform.position);
        threeDeeAttr.up = FMOD.Studio.UnityUtil.toFMODVector(transform.up);
        threeDeeAttr.forward = FMOD.Studio.UnityUtil.toFMODVector(transform.forward);
        threeDeeAttr.velocity = FMOD.Studio.UnityUtil.toFMODVector(Vector3.zero);
        m_blowInstance.set3DAttributes(threeDeeAttr);
        m_target.y = -0.2f; //Offset for explosion height. DELETE ME!
		_explosionParticles.transform.position = m_target;
        _explosionParticles.GetComponent<ParticleSystem>().Play();
        var colliders = Physics.OverlapSphere(transform.position, _blastRadius);
        m_target = Vector3.zero;
        foreach(Collider collider in colliders)
        {
            if (collider.CompareTag("Obstacle"))
            {
                collider.gameObject.SetActive(false);
            }
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, _blastRadius);
    }

    #endregion 
}

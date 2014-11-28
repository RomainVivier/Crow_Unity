using UnityEngine;
using System.Collections;

public class Rocket : Gadget {


    #region Members

    public float _rocketForce;
    public float _blastRadius;
    public float _rocketSpeed;
    public float _targetMaxDistance;

    private Timer m_rocketLaunchtimer;
    private Timer m_timer;
    private Vector3 m_offsetWithParent;
    private Vector3 m_startPosition;
    private Vector3 m_target;

    private FMOD.Studio.EventInstance m_rocketUI;
    private FMOD.Studio.ParameterInstance m_rocketDist;
    public FMOD_StudioEventEmitter m_rocketExecute;

    #endregion

    #region MonoBehaviour

    void Start()
    {
        m_offsetWithParent = transform.localPosition;
        m_rocketUI= FMOD_StudioSystem.instance.GetEvent("event:/SFX/Gadgets/Rocket/gadgetRocketUI");
        m_rocketUI.getParameter("distToTarget", out m_rocketDist);

        GadgetManager.Instance.Register("Rocket", this);
        m_rocketLaunchtimer = new Timer();
        m_timer = new Timer();
        gameObject.SetActive(false);
        m_target = Vector3.zero;
    }

    void Update()
    {
        if (m_rocketLaunchtimer.IsElapsedOnce)
        {
            Launch();
        }

        if (m_timer.IsElapsedOnce)
        {
            transform.localPosition = m_offsetWithParent;
            Blow();
            Stop();
        }

        if(!m_timer.IsElapsedLoop)
        {
            transform.position = Vector3.Lerp(m_startPosition, m_target, 1 - m_timer.CurrentNormalized);
            m_rocketDist.setValue(m_timer.CurrentNormalized);
        }
    }

    #endregion

    #region Overrided Functions

    public override void Play()
    {
        base.Play();
        gameObject.SetActive(true);
        IsReady = false;

        var obstacles = GameObject.FindGameObjectsWithTag("Obstacle");
        if(obstacles.Length > 0)
        {
            foreach(GameObject go in obstacles)
            {
                if (m_target == Vector3.zero || Vector3.Distance(transform.position, m_target) > Vector3.Distance(transform.position, go.transform.position))
                {
                    m_target = go.transform.position;
                    FMOD_StudioSystem.instance.PlayOneShot("event:/SFX/Gadgets/Rocket/gadgetRocketEngage", transform.position);
                    m_rocketLaunchtimer.Reset(0.6f);
                }
            }
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
        m_rocketExecute.Play();
        //FMOD_StudioSystem.instance.PlayOneShot("event:/SFX/Gadgets/Rocket/gadgetRocketExecute", transform.position);
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
        FMOD_StudioSystem.instance.PlayOneShot("event:/SFX/Gadgets/Rocket/gadgetRocketSuccess", transform.position);
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

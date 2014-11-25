using UnityEngine;
using System.Collections;

public class Rocket : Gadget {


    #region Members

    public float _rocketForce;
    public float _blastRadius;
    public float _rocketSpeed;
    public float _targetMaxDistance;

    private Timer m_timer;
    private Vector3 m_startPosition;
    private Vector3 m_target;

    #endregion

    #region MonoBehaviour

    void Start()
    {
        GadgetManager.Instance.Register("Rocket", this);
        m_timer = new Timer();
        gameObject.SetActive(false);
        m_target = Vector3.zero;
    }

    void Update()
    {
        if (m_timer.IsElapsedOnce)
        {
            transform.position = m_startPosition;
            Blow();
            Stop();
        }

        if(!m_timer.IsElapsedLoop)
        {
            transform.position = Vector3.Lerp(m_startPosition, m_target, 1 - m_timer.CurrentNormalized);
        }
    }

    #endregion

    #region Overrided Functions

    public override void Play()
    {
        base.Play();
        gameObject.SetActive(true);
        IsReady = false;

        m_startPosition = transform.position;
        var obstacles = GameObject.FindGameObjectsWithTag("Obstacle");
        if(obstacles.Length > 0)
        {
            foreach(GameObject go in obstacles)
            {
                if (m_target == Vector3.zero || Vector3.Distance(m_startPosition, m_target) > Vector3.Distance(m_startPosition, go.transform.position))
                {
                    m_target = go.transform.position;
                }
            }
        }

        m_target.y = m_startPosition.y;

        if (Vector3.Distance(m_startPosition, m_target) > _targetMaxDistance)
        {
            m_target = Vector3.zero;
        }else{
            Debug.Log(Vector3.Distance(m_startPosition, m_target) / _rocketSpeed);
            m_timer.Reset(Vector3.Distance(m_startPosition, m_target) / _rocketSpeed);
        }

        Debug.Log("target = " + m_target);
    }

    public override void Stop()
    {
        base.Stop();
        gameObject.SetActive(false);
        IsReady = true;
    }

    #endregion

    #region Rocket Functions

    void Blow()
    {
        //play visual effect
        var colliders = Physics.OverlapSphere(transform.position, _blastRadius);
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

using UnityEngine;
using System.Collections;

public class Spring : Gadget
{

    #region attribures
    private enum State { NOT_PLAYING, ASCENDING, GLIDING, FALLING}

    [System.Serializable]
    public class FallParameters
    {
        public float height;
        public float time;
        public float dist;
    }
    

    public float _jumpHeight=4;
    public float _ascendingTime = 0.4f;
    public float _glidingDist = 10;
    public float _glidingTime = 0.3f;
    public FallParameters[] _fallParameters;

    private Timer m_timer;
    private Vector3 m_basePos;
    //private Vector3 m_baseForward;
    private int m_nbBounces;
    private float m_vSpeed;
    private float m_vAccel;
    private float m_oldSpeed;

    private Car m_car;
    private Transform m_carBodyTransform;
    private Vector3 m_addPos;
    private State m_state;
    #endregion

    #region methods
    public override void Awake()
    {
        m_timer = new Timer();
        GadgetManager.Instance.Register("Spring", this);
        base.Awake();
    }

    void Start()
    {
        m_car = GameObject.FindObjectOfType<Car>();
        m_carBodyTransform = m_car.gameObject.transform.Find("Body");
        m_state = State.NOT_PLAYING;
    }

    public void FixedUpdate()
    {                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                    
        switch(m_state)
        {
            case State.ASCENDING:
                if(m_timer.IsElapsedOnce)
                {
                    m_state = State.GLIDING;
                    m_timer.Reset(_glidingTime);
                }
                else
                {
                    m_addPos += new Vector3(0,Time.fixedDeltaTime * _jumpHeight / _ascendingTime,0);
                    m_carBodyTransform.position = m_basePos + m_addPos;
                    setRot();
                }
                break;
            case State.GLIDING:
                if(m_timer.IsElapsedOnce)
                {
                    m_state = State.FALLING;
                    m_nbBounces = 0;
                    m_vAccel = _jumpHeight / (_fallParameters[0].time * _fallParameters[0].time);
                }
                else
                {
                    Vector3 forwardTarget = m_car.getDeltaTarget();
                    forwardTarget.y = 0;
                    forwardTarget.Normalize();
                    m_addPos += forwardTarget * Time.fixedDeltaTime * _glidingDist / _glidingTime;
                    m_carBodyTransform.position = m_basePos + m_addPos;
                    setRot();
                }
                break;
            case State.FALLING:
                {
                    float oldVSpeed = m_vSpeed;
                    m_vSpeed += m_vAccel * Time.fixedDeltaTime;
                    float avgVSpeed = (oldVSpeed + m_vSpeed) / 2;
                    m_addPos.y -= m_vSpeed * Time.fixedDeltaTime;
                    Vector3 forwardTarget = m_car.getDeltaTarget();
                    forwardTarget.y = 0;
                    forwardTarget.Normalize();
                    m_addPos += forwardTarget * Time.fixedDeltaTime * _fallParameters[m_nbBounces].dist / _fallParameters[m_nbBounces].time;
                    if (m_addPos.y <= 0 || m_car.isOnGround())
                    {
                        if (!m_car.isOnGround()) m_addPos.y = 0;
                        else Debug.Log("isOnGround");
                        m_nbBounces++;
                        if (m_nbBounces >= _fallParameters.Length) Stop();
                        else
                        {
                            m_vSpeed = -_fallParameters[m_nbBounces].height * 4 / _fallParameters[m_nbBounces].time;
                            m_vAccel = -m_vSpeed * 2 / _fallParameters[m_nbBounces].time;
                        }
                    }

                    m_carBodyTransform.position = m_basePos + m_addPos;
                    setRot();

                }
                break;

        }
    }

    private void setRot()
    {
        Vector3 forwardTarget = m_car.getForwardTarget();
        forwardTarget.y = 0;
        forwardTarget.Normalize();
        //Debug.Log(forwardTarget);
        Vector3 rot = m_carBodyTransform.rotation.eulerAngles;
        float mult = Mathf.Pow(0.2f, Time.fixedDeltaTime);
        rot.y=Mathf.LerpAngle(-Mathf.Atan2(forwardTarget.z, forwardTarget.x)*Mathf.Rad2Deg+90,rot.y,mult);

        m_carBodyTransform.rotation = Quaternion.Euler(rot);

    }
    public override void Play()
    {
        base.Play();
        m_basePos = m_carBodyTransform.position;
        m_state = State.ASCENDING;
        m_timer.Reset(_ascendingTime);
        m_oldSpeed = m_car.getForwardVelocity();
        //m_car.InstantSetSpeed(0);
        m_car.setDontMove(true);
        m_basePos = m_carBodyTransform.position;
        m_addPos = Vector3.zero;
        m_carBodyTransform.gameObject.rigidbody.isKinematic = true;
    }

    public override void Stop()
    {
        m_carBodyTransform.gameObject.rigidbody.isKinematic = false;
        m_car.setDontMove(false);
        m_car.InstantSetSpeed(m_oldSpeed,true);
        m_state = State.NOT_PLAYING;
        base.Stop();
    }
    #endregion
}

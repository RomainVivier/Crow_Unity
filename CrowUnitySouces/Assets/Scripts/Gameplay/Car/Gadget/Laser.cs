using UnityEngine;
using System.Collections;

public class Laser : ButtonGadget
{

    #region constants
    const float COOLDOWN_TIME=10f;
    const float VALVE_OPENING_TIME = 0.2f;
    const float FIRING_TIME = 1.5f;
    const float VALVE_CLOSING_TIME = 0.2f;

    const float START_ANGLE_DEG = -10f;
    const float END_ANGLE_DEG = 10f;
    const float RANGE = 100f;
    const float DISPLAY_RANGE = RANGE;
    #endregion

    #region members
    private Timer m_cooldownTimer;
    private Timer m_stateTimer;
    private Transform m_leftLightTransform;
    private Transform m_rightLightTransform;
    private Car m_car;
    private LineRenderer m_leftLineRenderer;
    private LineRenderer m_rightLineRenderer;
    private Transform m_particlesLTransform;
    private Transform m_particlesRTransform;
    private ParticleSystem m_particlesL;
    private ParticleSystem m_particlesR;
    private float m_lightsYOffset;

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
        // Init timers
        m_cooldownTimer = new Timer(0.01f);
        m_stateTimer = new Timer();

        // Register gadget
        GadgetManager.Instance.Register("Laser", this);

        // Set references
        Transform valvesPivotTransform=transform.Find("ValvesPivot");
        m_leftLightTransform = valvesPivotTransform.Find("ValveL");
        m_rightLightTransform = transform.Find("ValvesPivot").Find("ValveR");
        m_car = transform.parent.parent.parent.GetComponent<Car>();
        m_leftLineRenderer = m_leftLightTransform.GetComponent<LineRenderer>();
        m_rightLineRenderer = m_rightLightTransform.GetComponent<LineRenderer>();
        m_particlesL = transform.FindChild("ParticlesL").GetComponent<ParticleSystem>();
        m_particlesR = transform.FindChild("ParticlesR").GetComponent<ParticleSystem>();
        m_particlesLTransform = transform.FindChild("ParticlesL");
        m_particlesRTransform = transform.FindChild("ParticlesR");

        // Compute lightsYOffset
        Vector3 leftLightPos = m_leftLightTransform.position;
        Vector3 rightLightPos = m_rightLightTransform.position;
        Vector3 lightPosCenter = (leftLightPos + rightLightPos) / 2;
        m_lightsYOffset = (valvesPivotTransform.position - lightPosCenter).magnitude * 2;
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
                    m_leftLineRenderer.enabled = true;
                    m_rightLineRenderer.enabled = true;
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
                if(m_stateTimer.IsElapsedOnce)
                {
                    m_state = State.VALVE_CLOSING;
                    m_stateTimer.Reset(VALVE_CLOSING_TIME);
                    m_leftLineRenderer.enabled = false;
                    m_rightLineRenderer.enabled = false;
                    m_particlesL.Stop();
                    m_particlesR.Stop();
                }
                else
                {
                    float angle = Mathf.Lerp(END_ANGLE_DEG, START_ANGLE_DEG, m_stateTimer.CurrentNormalized)*Mathf.Deg2Rad;
                    Vector3 pos0L = m_leftLightTransform.position;
                    Vector3 up = m_car.getUpVector();
                    pos0L -= up * m_lightsYOffset;
                    Vector3 forward=m_car.getForwardVector();
                    Vector3 direc = forward * Mathf.Cos(angle) + up * Mathf.Sin(angle);
                    direc.Normalize();
                    Vector3 pos1L = pos0L + direc * DISPLAY_RANGE;
                    Vector3 pos0R = m_rightLightTransform.position - up * m_lightsYOffset;
                    Vector3 pos1R = pos0R + direc * DISPLAY_RANGE;
                    m_leftLineRenderer.SetPosition(0, pos0L);
                    m_leftLineRenderer.SetPosition(1, pos1L);
                    m_rightLineRenderer.SetPosition(0, pos0R);
                    m_rightLineRenderer.SetPosition(1, pos1R);
                    RaycastHit rh;
                    if (Physics.Raycast(pos0L, direc, out rh, RANGE))
                    {
                        if (!m_particlesL.isPlaying) m_particlesL.Play();
                        m_particlesLTransform.position = rh.point;
                    }
                    else m_particlesL.Stop();
                    if (Physics.Raycast(pos0R, direc, out rh, RANGE))
                    {
                        if (!m_particlesR.isPlaying) m_particlesR.Play();
                        m_particlesRTransform.position = rh.point;
                    }
                    else m_particlesR.Stop();
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

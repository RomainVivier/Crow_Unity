using UnityEngine;
using System.Collections;

public class Laser : Gadget
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
    const float TARGET_CONTACT_TIME = 0.2f;
    #endregion

    #region members
    public GameObject _laserEffect;
    private Timer m_cooldownTimer;
    private Timer m_stateTimer;
    private Car m_car;
    private float m_lightsYOffset;
    private LaserMembers[] m_lasers;
    
    private struct LaserMembers
    {
        public Transform lightTransform;
        public LineRenderer lineRenderer;
        public Transform particlesTransform;
        public ParticleSystem particles;
        public GameObject contactObject;
        public float contactTime;
    }


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
    public override void Awake () {

        // Init timers
        m_cooldownTimer = new Timer(0.01f);
        m_stateTimer = new Timer();

        // Register gadget
        GadgetManager.Instance.Register("Laser", this);

        // Set references
        m_lasers = new LaserMembers[2];
        Transform valvesPivotTransform=transform.Find("ValvesPivot");
        m_lasers[0].lightTransform = valvesPivotTransform.Find("ValveL");
        m_lasers[1].lightTransform = valvesPivotTransform.Find("ValveR");
        m_car = transform.parent.parent.parent.GetComponent<Car>();
        m_lasers[0].lineRenderer = m_lasers[0].lightTransform.GetComponent<LineRenderer>();
        m_lasers[1].lineRenderer = m_lasers[1].lightTransform.GetComponent<LineRenderer>();
        m_lasers[0].particles = transform.FindChild("ParticlesL").GetComponent<ParticleSystem>();
        m_lasers[1].particles = transform.FindChild("ParticlesR").GetComponent<ParticleSystem>();
        m_lasers[0].particlesTransform = transform.FindChild("ParticlesL");
        m_lasers[1].particlesTransform = transform.FindChild("ParticlesR");
        _laserEffect=GameObject.Find("LaserEffect");

        // Compute lightsYOffset
        Vector3 leftLightPos = m_lasers[0].lightTransform.position;
        Vector3 rightLightPos = m_lasers[1].lightTransform.position;
        Vector3 lightPosCenter = (leftLightPos + rightLightPos) / 2;
        m_lightsYOffset = (valvesPivotTransform.position - lightPosCenter).magnitude * 2;
        base.Awake();


	}
	
	public override void Update () {
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
                    for (int i = 0; i < 2;i++)
                    {
                        m_lasers[i].lineRenderer.enabled = true;
                        m_lasers[i].contactObject = null;
                    }
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
                    for (int i = 0; i < 2;i++)
                    {
                        m_lasers[i].lineRenderer.enabled = false;
                        m_lasers[i].particles.Stop();
                    }
                }
                else
                {
                    float angle = Mathf.Lerp(END_ANGLE_DEG, START_ANGLE_DEG, m_stateTimer.CurrentNormalized)*Mathf.Deg2Rad;
                    Vector3 forward=m_car.getForwardVector();
                    Vector3 up = m_car.getUpVector();
                    for(int i=0;i<2;i++)
                    {
                        Vector3 pos0 = m_lasers[i].lightTransform.position;
                        pos0 -= up * m_lightsYOffset;
                        Vector3 direc = forward * Mathf.Cos(angle) + up * Mathf.Sin(angle);
                        direc.Normalize();
                        Vector3 pos1 = pos0 + direc * DISPLAY_RANGE;
                        m_lasers[i].lineRenderer.SetPosition(0, pos0);
                        m_lasers[i].lineRenderer.SetPosition(1, pos1);
                        RaycastHit rh;
                        if (Physics.Raycast(pos0, direc, out rh, RANGE))
                        {
                            if (!m_lasers[i].particles.isPlaying) m_lasers[i].particles.Play();
                            m_lasers[i].particlesTransform.position = rh.point;
                            if(rh.collider!=null)
                            {
                                GameObject go = rh.collider.gameObject;
                                if(go!=m_lasers[i].contactObject)
                                {
                                    m_lasers[i].contactObject = go;
                                    m_lasers[i].contactTime = 0;
                                }
                                else
                                {
                                    m_lasers[i].contactTime += Time.deltaTime;
                                    if(m_lasers[i].contactTime>=TARGET_CONTACT_TIME && rh.collider.CompareTag("Obstacle"))
                                    {
                                        rh.collider.gameObject.SetActive(false);
                                        _laserEffect.transform.position = rh.point;
                                        _laserEffect.GetComponent<ParticleSystem>().Play();
                                    }
                                }
                            }
                            else m_lasers[i].contactObject=null;
                        }
                        else m_lasers[i].particles.Stop();
                   }
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
			FMOD_StudioSystem.instance.PlayOneShot("event:/SFX/Gadgets/Laser/gadgetLaserExecute",transform.position);
		
            IsReady = false;
            m_cooldownTimer.Reset(COOLDOWN_TIME);
            m_state = State.VALVE_OPENING;
            m_stateTimer.Reset(VALVE_OPENING_TIME);
        }
        base.Play();
    }

    #endregion
}

using UnityEngine;
using System.Collections;

public class Laser : Gadget
{


    #region constants
    const float COOLDOWN_TIME=10f;
    const float VALVE_OPENING_TIME = 0.2f;
    const float FIRING_TIME = 1.5f;
    const float VALVE_CLOSING_TIME = 0.2f;
    const float CONVERGING_DIST = 2f;
    const float RANGE = 100f;
    //const float DISPLAY_RANGE = RANGE;
    const float TARGET_CONTACT_TIME = 0.2f;
    #endregion

    #region members
    public GameObject _laserEffect;
    private Timer m_cooldownTimer;
    private Timer m_stateTimer;
    private Car m_car;
    private float m_lightsYOffset;
    private float m_laserLength = 0;
    private Transform m_leftLightTransform, m_rightLightTransform;
    private LineRenderer m_leftLineRenderer, m_rightLineRenderer, m_centerLineRenderer;
    //public Transform particlesTransform;
    //public ParticleSystem particles;
    private GameObject m_contactObject;
    private float m_contactTime;

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
        Transform valvesPivotTransform=transform.Find("ValvesPivot");
        m_leftLightTransform= valvesPivotTransform.Find("ValveL");
        m_rightLightTransform = valvesPivotTransform.Find("ValveR");
        m_car = transform.parent.parent.parent.GetComponent<Car>();
        m_leftLineRenderer = m_leftLightTransform.GetComponent<LineRenderer>();
        m_rightLineRenderer = m_rightLightTransform.GetComponent<LineRenderer>();
        m_centerLineRenderer = valvesPivotTransform.GetComponent<LineRenderer>();
        // = transform.FindChild("ParticlesL").GetComponent<ParticleSystem>();
        //m_lasers[1].particles = transform.FindChild("ParticlesR").GetComponent<ParticleSystem>();
        //m_lasers[0].particlesTransform = transform.FindChild("ParticlesL");
        //m_lasers[1].particlesTransform = transform.FindChild("ParticlesR");
        //_laserEffect=GameObject.Find("LaserEffect");

        // Compute lightsYOffset
        /*Vector3 leftLightPos = m_lasers[0].lightTransform.position;
        Vector3 rightLightPos = m_lasers[1].lightTransform.position;
        Vector3 lightPosCenter = (leftLightPos + rightLightPos) / 2;
        m_lightsYOffset = (valvesPivotTransform.position - lightPosCenter).magnitude * 2;*/

        // Stop explosion
        _laserEffect.GetComponent<ParticleSystem>().Stop();

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
                    m_leftLineRenderer.enabled=true;
                    m_rightLineRenderer.enabled=true;
                    m_centerLineRenderer.enabled=true;
                    m_contactObject=null;
                    m_laserLength = 0;
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
                        m_leftLineRenderer.enabled=false;
                        m_rightLineRenderer.enabled=false;
                        m_centerLineRenderer.enabled=false;
                        //m_lasers[i].particles.Stop();
                    }
                }
                else
                {
                    Vector3 forward=m_car.getForwardVector();
                    Vector3 right = m_car.getRightVector();

                    // Update laser length
                    m_laserLength += Time.deltaTime * RANGE / FIRING_TIME;

                    // Set converging lasers linerenderers position
                    Vector3 pos0l = m_leftLightTransform.position;
                    Vector3 pos0r = m_rightLightTransform.position;
                    Vector3 pos1 = (pos0l + pos0r) / 2+forward*CONVERGING_DIST;
                    float length = (pos1 - pos0l).magnitude;
                    m_leftLineRenderer.SetPosition(0, pos0l);
                    m_rightLineRenderer.SetPosition(0, pos0r);
                    if(length>m_laserLength)
                    {
                        Vector3 pos1l = pos0l + (pos1 - pos0l) * m_laserLength / length;
                        Vector3 pos1r = pos0r + (pos1 - pos0r) * m_laserLength / length;
                        m_leftLineRenderer.SetPosition(1, pos1l);
                        m_rightLineRenderer.SetPosition(1, pos1r);
                        length = m_laserLength;
                    }
                    else
                    {
                        m_leftLineRenderer.SetPosition(1, pos1);
                        m_rightLineRenderer.SetPosition(1, pos1);
                    }

                    // Update and draw center laser
                    length = m_laserLength - length;
                    Vector3 pos2 = pos1 + forward * length;
                    m_centerLineRenderer.SetPosition(0, pos1);
                    m_centerLineRenderer.SetPosition(1, pos2);

                    // Raycast to check damages
                    RaycastHit rh;
                    if(Physics.Raycast(pos1,forward,out rh,length))
                    {
                        //if (!m_lasers[i].particles.isPlaying) m_lasers[i].particles.Play();
                        //m_lasers[i].particlesTransform.position = rh.point;
                        if(rh.collider!=null)
                        {
                            GameObject go = rh.collider.gameObject;
                            if(go!=m_contactObject)
                            {
                                m_contactObject = go;
                                m_contactTime = 0;
                            }
                            else
                            {
                                m_contactTime += Time.deltaTime;
                                if(m_contactTime>=TARGET_CONTACT_TIME && rh.collider.CompareTag("Obstacle"))
                                {
                                    _laserEffect.GetComponent<ParticleSystem>().Play();
                                    _laserEffect.transform.position = go.transform.position;//rh.point;
                                    rh.collider.gameObject.SetActive(false);
                                    Score.Instance.AddScore(500);

                                    // Play sound
                                    FMOD.Studio.EventInstance blowInstance
                                        = FMOD_StudioSystem.instance.GetEvent("event:/SFX/Gadgets/Rocket/gadgetRocketSuccess");
                                    blowInstance.start();
                                    FMOD.Studio.ParameterInstance param;
                                    blowInstance.getParameter("Position", out param);
                                    Vector3 dpos = go.transform.position - transform.position;
                                    Vector3 fwd = transform.forward;
                                    float position = Vector3.Dot(dpos, fwd);
                                    param.setValue(position > 0 ? 0 : 1);
                                    FMOD.Studio._3D_ATTRIBUTES threeDeeAttr = new FMOD.Studio._3D_ATTRIBUTES();
                                    threeDeeAttr.position = FMOD.Studio.UnityUtil.toFMODVector(go.transform.position);
                                    threeDeeAttr.up = FMOD.Studio.UnityUtil.toFMODVector(go.transform.up);
                                    threeDeeAttr.forward = FMOD.Studio.UnityUtil.toFMODVector(go.transform.forward);
                                    threeDeeAttr.velocity = FMOD.Studio.UnityUtil.toFMODVector(Vector3.zero);
                                    blowInstance.set3DAttributes(threeDeeAttr);
                                }
                            }
                        }
                        else m_contactObject=null;
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
		    _laserEffect.GetComponent<ParticleSystem>().Stop();
            IsReady = false;
            m_cooldownTimer.Reset(COOLDOWN_TIME);
            m_state = State.VALVE_OPENING;
            m_stateTimer.Reset(VALVE_OPENING_TIME);
        }
        base.Play();
    }

    #endregion
}

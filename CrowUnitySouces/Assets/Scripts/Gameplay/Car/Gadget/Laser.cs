using UnityEngine;
using System.Collections;

public class Laser : Gadget
{


    #region constants
    const float VALVE_OPENING_TIME = 0.2f;
    const float LASER_JUNCTION_TIME = 0.3f;
    const float VALVE_CLOSING_TIME = 0.2f;
    const float RANGE = 500f;
    //const float DISPLAY_RANGE = RANGE;
    #endregion

    #region members
    public GameObject _laserEffect;
    public float _convergingDist = 2;
    public float _firingTime = 4;
    public float _particlesSpeed = 100; // units/s
    public float _particlesRotationSpeed = 1; // rad/s
    public float _targetContactTime = 0.2f;
    private Timer m_cooldownTimer;
    private Timer m_stateTimer;
    private Car m_car;
    private float m_lightsYOffset;
    private float m_laserLength = 0;
    private float m_particlesPos = 0;
    private float m_particlesRot = 0;

    private Transform m_leftLightTransform, m_rightLightTransform;
    private LineRenderer m_leftLineRenderer, m_rightLineRenderer, m_centerLineRenderer;

    private Transform m_particlesJunctionBoomTransform;
    private ParticleSystem m_particlesJunctionBoom;
    private Transform m_particlesJunctionTransform;
    private ParticleSystem m_particlesJunction;
    private Transform m_particlesLaserTransform;
    private ParticleSystem m_particlesLaser;

    private GameObject m_contactObject;
    private float m_contactTime;

    private enum State
    {
        READY,
        VALVE_OPENING,
        LASER_JUNCTION,
        FIRING,
        VALVE_CLOSING,
        COOLDOWN
    };

    State m_state = State.READY;

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
        m_particlesJunctionBoomTransform = transform.Find("ParticlesJunctionBoom");
        m_particlesJunctionBoom = m_particlesJunctionBoomTransform.GetComponent<ParticleSystem>();
        m_particlesJunctionTransform = transform.Find("ParticlesJunction");
        m_particlesJunction = m_particlesJunctionBoomTransform.GetComponent<ParticleSystem>();
        m_particlesLaserTransform = transform.Find("ParticlesLaser");
        m_particlesLaser = m_particlesLaserTransform.GetComponent<ParticleSystem>();
        m_particlesLaser.Stop();

        // = transform.FindChild("ParticlesL").GetComponent<ParticleSystem>();

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
                    m_state = State.LASER_JUNCTION;
                    m_stateTimer.Reset(LASER_JUNCTION_TIME);
                    m_leftLineRenderer.enabled=true;
                    m_rightLineRenderer.enabled=true;
                    m_contactObject=null;
                    m_laserLength = 0;
                }
                else transform.FindChild("ValvesPivot").localEulerAngles=new Vector3(-180+m_stateTimer.CurrentNormalized * 180,0,0);
                break;
            case State.LASER_JUNCTION:
                if(m_stateTimer.IsElapsedOnce)
                {
                    // Update state
                    m_centerLineRenderer.enabled = true;
                    m_state = State.FIRING;
                    m_stateTimer.Reset(_firingTime);

                    // Start particles
                    m_particlesJunctionBoom.Stop();
                    m_particlesJunctionBoom.Play();
                    m_particlesJunction.Play();
                    m_particlesJunctionTransform.gameObject.SetActive(true);
                    Vector3 forward=m_car.getForwardVector();
                    Vector3 pos0l = m_leftLightTransform.position;
                    Vector3 pos0r = m_rightLightTransform.position;
                    Vector3 pos1 = (pos0l + pos0r) / 2+forward*_convergingDist*0.9f;
                    m_particlesJunctionBoomTransform.position = pos1;
                    m_particlesJunctionTransform.position = pos1;
                    m_particlesLaser.Play();
                    m_particlesPos = 0;
                    m_particlesRot = 0;
                }
                else
                {
                    Vector3 forward=m_car.getForwardVector();
                    float progress = 1-(m_stateTimer.Current / LASER_JUNCTION_TIME);
                    Vector3 pos0l = m_leftLightTransform.position;
                    Vector3 pos0r = m_rightLightTransform.position;
                    Vector3 pos1 = (pos0l + pos0r) / 2+forward*_convergingDist;
                    m_leftLineRenderer.SetPosition(0, pos0l);
                    m_rightLineRenderer.SetPosition(0, pos0r);
                    Vector3 pos1l = pos0l + (pos1 - pos0l) * progress;
                    Vector3 pos1r = pos0r + (pos1 - pos0r) * progress;
                    m_leftLineRenderer.SetPosition(1, pos1l);
                    m_rightLineRenderer.SetPosition(1, pos1r);
                    float length = (pos1l - pos0l).magnitude;
                    float finalLength = (pos1 - pos0l).magnitude;
                    m_leftLineRenderer.material.mainTextureScale = new Vector2(finalLength/2*length/finalLength,1);
                    m_rightLineRenderer.material.mainTextureScale = new Vector2(finalLength/2*length/finalLength,1);
                }
                break;
            case State.FIRING:
                if(m_stateTimer.IsElapsedOnce)
                {
                    m_state = State.VALVE_CLOSING;
                    m_stateTimer.Reset(VALVE_CLOSING_TIME);
                    m_leftLineRenderer.enabled=false;
                    m_rightLineRenderer.enabled=false;
                    m_centerLineRenderer.enabled=false;
                    m_particlesJunctionTransform.gameObject.SetActive(false);
                    m_particlesLaser.Stop();
                }
                else
                {
                    Vector3 forward=m_car.getForwardVector();
                    Vector3 forwardTarget = m_car.getForwardTarget();
                    Vector3 right = m_car.getRightVector();
                    Vector3 up = m_car.getUpVector();

                    // Update laser length
                    m_laserLength += Time.deltaTime * RANGE / _firingTime;

                    // Set converging lasers linerenderers position
                    Vector3 pos0l = m_leftLightTransform.position;
                    Vector3 pos0r = m_rightLightTransform.position;
                    Vector3 pos1 = (pos0l + pos0r) / 2+forward*_convergingDist;
                    m_leftLineRenderer.SetPosition(0, pos0l);
                    m_rightLineRenderer.SetPosition(0, pos0r);
                    m_leftLineRenderer.SetPosition(1, pos1);
                    m_rightLineRenderer.SetPosition(1, pos1);

                    // Update and draw center laser
                    Vector3 pos2 = pos1 + forwardTarget * m_laserLength;
                    m_centerLineRenderer.SetPosition(0, pos1);
                    m_centerLineRenderer.SetPosition(1, pos2);
                    m_centerLineRenderer.material.mainTextureScale = new Vector2(m_laserLength/25,1);
                    m_centerLineRenderer.SetWidth(0.2f, 5f);
                    
                    // Place particles
                    m_particlesPos += Time.deltaTime * _particlesSpeed;
                    m_particlesRot += Time.deltaTime * _particlesRotationSpeed;
                    float progress = (1 - (m_stateTimer.Current / _firingTime));
                    float dist=0.2f+progress*1f;
                    m_particlesLaserTransform.position = pos1 + forwardTarget * m_particlesPos + up * dist * Mathf.Cos(m_particlesRot) + right * dist * Mathf.Sin(m_particlesRot);
                    m_particlesLaser.startColor = new Color(1,1,1,1 - progress);
                    
                    // Raycast to check damages
                    RaycastHit rh;
                    if(Physics.Raycast(pos1,forwardTarget,out rh,m_laserLength))
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
                                if(m_contactTime>=_targetContactTime && rh.collider.CompareTag("Obstacle"))
                                {
                                    _laserEffect.GetComponent<ParticleSystem>().Play();
                                    _laserEffect.transform.position = go.transform.position;//rh.point;
                                    rh.collider.gameObject.SetActive(false);
                                    addScore();

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
            case State.COOLDOWN:
                if (m_cooldownTimer.IsElapsedOnce)
                {
                    IsReady = true;
                    m_state = State.READY;
                    Stop();
                    m_cooldownTimer.Reset(0);
                }
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
            m_cooldownTimer.Reset(_cooldown);
            m_state = State.VALVE_OPENING;
            m_stateTimer.Reset(VALVE_OPENING_TIME);
        }
        base.Play();
    }

    #endregion
}

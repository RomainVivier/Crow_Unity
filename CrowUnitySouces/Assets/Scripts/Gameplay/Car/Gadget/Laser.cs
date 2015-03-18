using UnityEngine;
using System.Collections;

public class Laser : Gadget
{


    #region constants
    const float VALVE_OPENING_TIME = 0.2f;
    const float VALVE_CLOSING_TIME = 0.2f;
    const float CONVERGING_INERTIA = 0.1f;
    const float TEXTURE_SCROLL_SPEED = 5;
    //const float DISPLAY_RANGE = RANGE;
    #endregion

    #region members
    public GameObject _laserEffect;
    public float _firingTime = 4;
    public float _particlesSpeed = 100; // units/s
    public float _particlesRotationSpeed = 1; // rad/s
    public float _targetContactTime = 0.2f;
 	public float _range=500f;
 	
    private Timer m_stateTimer;
    private Car m_car;
    private float m_lightsYOffset;
    private float m_laserLength = 0;
    private float m_particlesPos = 0;
    private float m_particlesRot = 0;
    private float m_textureScroll;

    private enum State
    {
        READY,
        VALVE_OPENING,
        FIRING,
        VALVE_CLOSING,
        COOLDOWN
    };

    private struct LaserInfos
    {
        public Transform lightTransform;
        public LineRenderer lineRenderer;
        public Transform particlesTransform;
        public ParticleSystem particles;
        public float angle;
        public GameObject contactObject;
        public float contactTime;
    }
    private LaserInfos[] m_lasers;

    State m_state = State.READY;

    #endregion

    #region methods
    public override void Awake () {
        // Init timers
        m_stateTimer = new Timer();

        // Register gadget
        GadgetManager.Instance.Register("Laser", this);

        // Set references
        Transform valvesPivotTransform=transform.Find("ValvesPivot");
        m_lasers=new LaserInfos[2];
        m_lasers[0].lightTransform=valvesPivotTransform.Find("ValveL");
        m_lasers[1].lightTransform=valvesPivotTransform.Find("ValveR");
        m_lasers[0].particlesTransform = transform.Find("ParticlesLaserL");
        m_lasers[1].particlesTransform = transform.Find("ParticlesLaserR");
        for(int i=0;i<2;i++)
        {
            m_lasers[i].lineRenderer = m_lasers[i].lightTransform.GetComponent<LineRenderer>();
            m_lasers[i].particles = m_lasers[i].particlesTransform.GetComponent<ParticleSystem>();
            m_lasers[i].particles.Stop();
        }
        m_car = transform.parent.parent.parent.GetComponent<Car>();


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
                    m_stateTimer.Reset(_firingTime);
                    for (int i = 0; i < 2;i++)
                    {
                        m_lasers[i].lineRenderer.enabled = true;
                        m_lasers[i].contactObject = null;
                        m_lasers[i].particlesTransform.position = m_lasers[i].lightTransform.position;
                        m_lasers[i].particles.Play();
                        m_lasers[i].particles.active = true;
                        m_lasers[i].angle = 0;
                    }
                    m_laserLength = 0;
                    m_particlesPos = 0;
                    m_particlesRot = 0;
                    m_textureScroll = 0;
                }
                else transform.FindChild("ValvesPivot").localEulerAngles=new Vector3(-180+m_stateTimer.CurrentNormalized * 180,0,0);
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
                        m_lasers[i].particles.active = false;
                    }
                }
                else
                {
                    Vector3 forward=m_car.getForwardVector();
                    Vector3 forwardTarget = m_car.getForwardTarget();
                    Vector3 right = m_car.getRightVector();
                    Vector3 up = m_car.getUpVector();
                    float hAngle = Mathf.Acos(Vector3.Dot(forward, forwardTarget));
                    if (Vector3.Dot(forwardTarget, right) < 0) hAngle = -hAngle;
                    if (float.IsNaN(hAngle)) hAngle = 0;

                    // Update laser length
                    m_laserLength += Time.deltaTime * _range / _firingTime;
                    m_textureScroll += Time.deltaTime * TEXTURE_SCROLL_SPEED;

                    // Raycast to detect target
                    Vector3 centerPoint = (m_lasers[0].lightTransform.position + m_lasers[1].lightTransform.position) / 2;
                    RaycastHit rh;
                    float tgtAngle;
                    float hypothenuse=m_laserLength;
                    if (Physics.Raycast(centerPoint, forwardTarget, out rh, m_laserLength) && rh.collider.CompareTag("Obstacle"))
                    {
                        Vector3 contactPoint = rh.point;
                        float adjacent = (contactPoint - centerPoint).magnitude;
                        float opposed = (centerPoint - m_lasers[0].lightTransform.position).magnitude;
                        hypothenuse = Mathf.Sqrt(adjacent * adjacent + opposed * opposed);
                        tgtAngle = Mathf.Atan(opposed / adjacent);
                    }
                    else tgtAngle = 0;

                    // Update particles
                    m_particlesPos += Time.deltaTime * _particlesSpeed;
                    m_particlesRot += Time.deltaTime * _particlesRotationSpeed;
                    float progress = (1 - (m_stateTimer.Current / _firingTime));
                    float dist=0.2f+progress*1f;
                    
                    float mult=Mathf.Pow(CONVERGING_INERTIA,Time.deltaTime);
                    for(int i=0;i<2;i++)
                    {
                        // Update and draw lasers
                        m_lasers[i].angle=Mathf.Lerp(tgtAngle,m_lasers[i].angle,mult);
                        Vector3 direc = Mathf.Cos(m_lasers[i].angle) * forwardTarget + Mathf.Sin(m_lasers[i].angle) * right * (i == 0 ? 1 : -1);
                        direc.Normalize();
                        Vector3 startPos = m_lasers[i].lightTransform.position;
                        Vector3 endPos = startPos + direc * hypothenuse;
                        m_lasers[i].lineRenderer.SetPosition(0, startPos);
                        m_lasers[i].lineRenderer.SetPosition(1, endPos);
                        m_lasers[i].lineRenderer.material.mainTextureScale = new Vector2(hypothenuse / 10, 1);
                        m_lasers[i].lineRenderer.material.mainTextureOffset = new Vector2(-m_textureScroll, 1);

                        // Place particles
                        m_lasers[i].particlesTransform.position = startPos;// + /*endPos +  direc * m_particlesPos +*/;// up * dist * Mathf.Cos(m_particlesRot) + right * dist * Mathf.Sin(m_particlesRot);
                        /*Vector3 rot=m_lasers[i].particlesTransform.localRotation.eulerAngles;
                        rot.z=m_particlesRot*Mathf.Rad2Deg%360;
                        rot.y = hAngle*Mathf.Rad2Deg;
                        rot.x = 0;
                        m_lasers[i].particlesTransform.localRotation.=Quaternion.Euler(rot);*/
                        m_lasers[i].particlesTransform.localEulerAngles = new Vector3(0, hAngle * Mathf.Rad2Deg, m_particlesRot * Mathf.Rad2Deg%360);
                        m_lasers[i].particles.startColor = new Color(1,1,1,1 - progress);
                        //m_lasers[i].particles.startSpeed = m_car.getForwardVelocity();

                        // Raycast to check damages
                        if(Physics.Raycast(startPos,direc,out rh,hypothenuse))
                        {
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
                                    if(m_lasers[i].contactTime>=_targetContactTime && rh.collider.CompareTag("Obstacle"))
                                    {
                                        _laserEffect.GetComponent<ParticleSystem>().Play();
                                        _laserEffect.transform.position = go.transform.position;//rh.point;
                                        rh.collider.gameObject.transform.parent.gameObject.SetActive(false);
                                        Score.ScoreType type=rh.collider.gameObject.name.Contains("Obstacle") ? Score.ScoreType.MINOR_OBSTACLE : Score.ScoreType.EVENT;
                                        addScore(type,rh.collider.transform.position);

                                        // Play sound
                                        FMOD.Studio.EventInstance blowInstance
												= FMOD_StudioSystem.instance.GetEvent("event:/SFX/Gadgets/Laser/gadgetLaserSuccess");
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
                                        
                                        // Trigger events
                                        DialogsManager._instance.triggerEvent(DialogsManager.DialogInfos.EventType.OBSTACLE_DESTRUCTION, go.gameObject.name);
                                        DialogsManager._instance.triggerEvent(DialogsManager.DialogInfos.EventType.DESTRUCTION_WITH_GADGET, "Laser");
                                    }
                                }
                            }
                            else m_lasers[i].contactObject=null;
                        }
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
                /*if (m_gadgetCooldownTimer.IsElapsedOnce)
                {
                    m_state = State.READY;
                    Stop();
                    m_cooldownTimer.Reset(0);
                }*/
                break;
        }
        base.Update();
    }

    public override void Play()
    {
        if (IsReady) m_state = State.READY;
        if(m_state!=State.READY)
        {
            Debug.LogError("Error : gadget laser is not ready");
        }
        else
        {
			FMOD_StudioSystem.instance.PlayOneShot("event:/SFX/Gadgets/Laser/gadgetLaserExecute",transform.position);
		    _laserEffect.GetComponent<ParticleSystem>().Stop();
            IsReady = false;
            //m_gadgetCooldownTimer.Reset(_cooldown);
            m_state = State.VALVE_OPENING;
            m_stateTimer.Reset(VALVE_OPENING_TIME);
        }
        base.Play();
    }

    #endregion
}

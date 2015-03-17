using UnityEngine;
using System.Collections;

public class CarCollisionsHandler : MonoBehaviour
{
    #region public parameters
    public float _maxAngleHDeg = 45;
	public float _minAngleVDeg = 40;
    public float _maxAngleVDeg = 45;

    public float _minMomentum = 10000;// In Kg.m/s (or N.s)
    public float _maxMomentum = 30000;
    public float _ownMomentum = 10000;
    public static bool _dontCollide = false;
    public Spring _spring;
    public Radio _radio;
    #endregion

    #region private members
    private FMOD.Studio.EventInstance m_impactVehicleSound;
    private FMOD.Studio.ParameterInstance m_impactVehicleSpeed;
    private FMOD.Studio.EventInstance m_impactConcreteSound;
    private FMOD.Studio.ParameterInstance m_impactConcreteSpeed;
    private Car m_car;
    private RailsControl m_railsControl;
    private Timer cooldownTimer;
	private WindshieldController m_windshield;
	private CameraShake m_cameraShake;
    private GameObject m_lastObject;
    private Collider m_collider;
	private bool m_projectObstacles=false;
	
    #endregion

    #region MonoBehaviour
    void Start()
    {
        m_impactVehicleSound=FMOD_StudioSystem.instance.GetEvent("event:/SFX/Impacts/impactVehicle");
        m_impactVehicleSound.getParameter("Speed", out m_impactVehicleSpeed);
        m_impactConcreteSound=FMOD_StudioSystem.instance.GetEvent("event:/SFX/Impacts/impactConcrete");
        m_impactConcreteSound.getParameter("Speed", out m_impactConcreteSpeed);
        m_car = transform.parent.gameObject.GetComponent<Car>();
        m_railsControl = m_car.gameObject.GetComponent<RailsControl>();
		m_windshield = GetComponentInChildren<WindshieldController>();
		m_cameraShake = GetComponentInChildren<CameraShake>();
        cooldownTimer = new Timer();
        cooldownTimer.Reset(0.01f);
        m_lastObject = null;
        m_collider = collider;
    }
    #endregion

    #region Collider
    
    void OnTriggerEnter(Collider other)
    {
        GameObject oth = other.gameObject;
        if (oth.tag == "Obstacle")
        {
            //if (_dontCollide) return;
            Vector3 forward = m_car.getForwardVector();
            Vector3 diff = oth.transform.position - m_car.transform.Find("Body").position;
            float fPos = Vector3.Dot(diff, forward); 
            if (fPos<2 || fPos>5) return;
            if (cooldownTimer.IsElapsedLoop || oth!=m_lastObject)
            {
                if(!m_projectObstacles)
                {
                	playSound(null, oth, m_impactVehicleSound, m_impactVehicleSpeed);
                	if(m_car.getForwardVelocityKmh()>100) _radio.ResetPickup();
					m_windshield.Hit();
					DialogsManager._instance.triggerEvent(DialogsManager.DialogInfos.EventType.CAR_HP, (float) m_windshield._hp);
					DialogsManager._instance.triggerEvent(DialogsManager.DialogInfos.EventType.CAR_DAMAGE, oth.name);
                }
				else FMOD_StudioSystem.instance.PlayOneShot("event:/SFX/Impacts/impactDash",transform.position);
                cooldownTimer.Reset(2f);
                m_lastObject = oth;
            }
            float hAngle = Random.Range(-_maxAngleHDeg, _maxAngleHDeg) * Mathf.Deg2Rad;
            Vector3 right = m_car.getRightVector();
            Vector3 hVector = forward * Mathf.Cos(hAngle) + right * Mathf.Sin(hAngle);
			float vAngle = Random.Range(_minAngleVDeg, _maxAngleVDeg) * Mathf.Deg2Rad;
            Vector3 up = m_car.getUpVector();
            Vector3 direc = hVector * Mathf.Cos(vAngle) + up * Mathf.Sin(vAngle);
            float momentum=Mathf.Lerp(_minMomentum,_maxMomentum,m_car.getForwardVelocityKmh()/m_car.maxSpeedKmh);
            if(oth.rigidbody != null)
            {
                oth.rigidbody.AddForce(direc * momentum,ForceMode.Impulse);
                oth.transform.parent.gameObject.AddComponent<ObstacleDestroyer>();
				if(!m_projectObstacles) rigidbody.AddForce(-forward * _ownMomentum, ForceMode.Impulse);
            }
            else GameObject.Destroy(oth.transform.parent.gameObject);
			m_cameraShake.DoShake();
			
            if(!m_projectObstacles)
            {
    	        Score.Instance.ResetCombo();
            	_spring.collide();
            }
            else
            {
            	Score.Instance.AddScore(Score.ScoreType.MINOR_OBSTACLE,0,transform.position,1);
            }
        }
        else if (oth.tag == "Barrier")
        {
            m_railsControl.ShiftToPreviousRail();
        } 
    }

    void OnCollisionEnter(Collision collision)
    {
        if (_dontCollide) return;
        GameObject oth = collision.gameObject;
        if (!collision.collider.isTrigger)
        {
            Vector3 contactPoint = collision.contacts[0].point;
            Vector3 center = transform.position;
            Vector3 diff = contactPoint - center;
            float relDy = Vector3.Dot(transform.up, diff);
            if (relDy > -0.7) playSound(collision, oth, m_impactConcreteSound, m_impactConcreteSpeed);
        }

    }
    #endregion
	
	#region public methods
	public void setProjectObstacles(bool projectObstacles)
	{
		m_projectObstacles=projectObstacles;
	}
	#endregion
	
    #region private methods
    void playSound(Collision collision, GameObject oth, FMOD.Studio.EventInstance sound, FMOD.Studio.ParameterInstance param)
    {
		if (param == null) return;
        FMOD.Studio._3D_ATTRIBUTES threeDeeAttr = new FMOD.Studio._3D_ATTRIBUTES();
        threeDeeAttr.up = FMOD.Studio.UnityUtil.toFMODVector(new Vector3(0,1,0));
        if(collision!=null)
        {
            threeDeeAttr.forward = FMOD.Studio.UnityUtil.toFMODVector(collision.contacts[0].normal);
            threeDeeAttr.position = FMOD.Studio.UnityUtil.toFMODVector(collision.contacts[0].point);
        }
        else
        {
            threeDeeAttr.forward = FMOD.Studio.UnityUtil.toFMODVector(m_car.getForwardVector());
            threeDeeAttr.position = FMOD.Studio.UnityUtil.toFMODVector(oth.transform.position);
        }
        threeDeeAttr.velocity = FMOD.Studio.UnityUtil.toFMODVector(Vector3.zero);
        sound.set3DAttributes(threeDeeAttr);
        float maxSpeed = m_car.maxSpeedKmh;
        RailsControl rc = m_car.gameObject.GetComponent<RailsControl>();
        if (rc != null) maxSpeed = rc.setSpeedKmh;
        Vector3 relativeVelocity = m_car.getVelocity();
        Rigidbody body=collision==null ? oth.rigidbody : collision.gameObject.rigidbody;
        //if(body!=null) relativeVelocity-=body.GetRelativePointVelocity(new Vector3(0, 0, 0));
        param.setValue(Mathf.Min(relativeVelocity.magnitude*3.6f / maxSpeed,1));
        sound.start();
    }
    #endregion
}

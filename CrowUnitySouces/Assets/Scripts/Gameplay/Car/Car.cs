using UnityEngine;
using System.Collections;

public class Car : MonoBehaviour 
{

	// Inspector visible variables
    [Header("Physics parameters")]
	public float maxSpeedKmh=300; // km/h
	public float fwd; // Front wheels drive [0;1] 0=RWD 1=FWD
	public float brakeDecceleration=10; // m/s²
	public float brakesRepartition=0.6f; // 0=rear, 1=front
	public float steerAngle0kmhDeg=40;
	public float steerAngleTopSpeedDeg=20;
	public float antiRoll=8000;
	public float downforce=10;
    public float wheelRotation = 180;

    [Header("Fake engine sound parameters")]
    public float fakeSoundAcceleration = 0.9f;
    public float fakeSoundBrakes = 0.7f;
    public float fakeSoundBrakesSpeedFriction = 0.2f;
    public float fakeSoundInstantThresholdKmh = 2f;
    public float overRevPeriod = 0.01f;

	// Components
	private Engine engine;
	private CarControl control;
	private Rigidbody body;
	private WheelCollider[] wheels;
	private Transmission transmission;
    private RailsControl railsControl;
   
	// Private attributes
	private float dragCoef;
	private float mass;
	private float maxSpeed; // unit/s (aka m/s)
	private float wheelRadius;
	private int nbUpdates=0;
	private float acceleration2Torque;
	private float brakeTorque;
	private float wheelBase;
	private float wheelTrack;
	private CarControl.CarInputs oldInputs;
	private Vector3 centerOfMass;
    private Quaternion wheelQuaternion;
    private GameObject wheelObject;

	// Sounds
    private FMOD.Studio.EventInstance engineSound;
    private FMOD.Studio.ParameterInstance engineRPM;
    private FMOD.Studio.ParameterInstance engineSpeed;
    private FMOD.Studio.ParameterInstance engineLoad;
    private const int ENGINE_SOUND_MAX_RPM = 7000;
    private FMOD.Studio.EventInstance rumbleSound;
    private FMOD.Studio.ParameterInstance rumbleSpeed;
    private FMOD.Studio.EventInstance tiresSound;
    private FMOD.Studio.ParameterInstance tiresFriction;
    private FMOD.Studio.ParameterInstance tiresSpeed;
    private FMOD.Studio.ParameterInstance tiresGround;

    private float fakeSoundSpeed=0; // 0=0m/s, 1=max speed
    private int fakeGear = 0;
    private float overRevTime = 0;

     // MonoBehaviour methods
	void Start ()
	{
		updateValues ();
        engineSound = FMOD_StudioSystem.instance.GetEvent("event:/SFX/Car Mechanics/carEngine");
        engineSound.start();
        engineSound.getParameter("RPM", out engineRPM);
        engineSound.getParameter("Speed", out engineSpeed);
        engineSound.getParameter("Load", out engineLoad);
        rumbleSound = FMOD_StudioSystem.instance.GetEvent("event:/SFX/Car Mechanics/carRumble");
        rumbleSound.start();
        rumbleSound.getParameter("Speed", out rumbleSpeed);
        tiresSound = FMOD_StudioSystem.instance.GetEvent("event:/SFX/Car Mechanics/carTyres");
        tiresSound.getParameter("Friction", out tiresFriction);
        tiresSound.getParameter("Speed", out tiresSpeed);
        tiresSound.getParameter("Ground", out tiresGround);
        tiresSound.start();
        wheelObject = transform.FindChild("Body/CarModel/RootWheel/Wheel").gameObject;
        wheelQuaternion = wheelObject.transform.localRotation;
	}

    void FixedUpdate ()
	{
        if (Input.GetAxis("Upshift") > 0) InstantSetSpeedKmh(300);
		CarControl.CarInputs inputs=control.getInputs();
		float dt=Time.fixedDeltaTime;
		int freq=(int) (1.0f/dt);
		nbUpdates=(nbUpdates+1)%freq;

		// Update transmission
        if (inputs.upshift && !oldInputs.upshift)
        {
            transmission.upshift();
            FMOD_StudioSystem.instance.PlayOneShot("event:/SFX/Car Mechanics/carGearUp", transform.position);
        }
        if (inputs.downshift && !oldInputs.downshift)
        {
            transmission.downshift();
            FMOD_StudioSystem.instance.PlayOneShot("event:/SFX/Car Mechanics/carGearDown", transform.position);
        }
				
		// Compute forward torque
		Vector3 velocity=body.GetRelativePointVelocity(new Vector3(0,0,0));
		float forwardVelocity=Vector3.Dot(velocity,body.transform.forward);
		float rpm=forwardVelocity*transmission.getSpeed2Rpm();
		float power=engine.getPower(rpm,inputs.throttle);
		float acceleration=
			(Mathf.Sqrt(forwardVelocity*forwardVelocity+dt*power*2/mass)-forwardVelocity)/dt;
		if(float.IsNaN(acceleration)) acceleration=0;
		float torque=acceleration*acceleration2Torque;
		
		// Apply torque to wheels
		for(int i=0;i<4;i++)
		{
			float accelMult= i<2 ? fwd : 1-fwd;
			float brakeMult= i<2 ? brakesRepartition : 1-brakesRepartition;
			wheels[i].brakeTorque=inputs.brake*brakeTorque*brakeMult;
			if(transmission.isEngaged()==1) wheels[i].motorTorque=torque*accelMult/2;
				else wheels[i].motorTorque=0;
		}		
		
		// Steering
		float steerAngleOut=Mathf.Lerp(steerAngle0kmhDeg,steerAngleTopSpeedDeg,forwardVelocity/maxSpeed)*inputs.steering;
        float steerAngleIn;
        if(steerAngleOut>0) steerAngleIn=90-Mathf.Atan (Mathf.Tan ((90-steerAngleOut)*Mathf.Deg2Rad)-wheelTrack/wheelBase)*Mathf.Rad2Deg;
        else if(steerAngleOut<0) steerAngleIn=Mathf.Atan (Mathf.Tan ((90+steerAngleOut)*Mathf.Deg2Rad)-wheelTrack/wheelBase)*Mathf.Rad2Deg-90;
        else steerAngleIn=0;
        if(steerAngleOut>0)
		{
			wheels[0].steerAngle=steerAngleOut;
			wheels[1].steerAngle=steerAngleIn;	
		}
		else
		{
			wheels[0].steerAngle=steerAngleIn;
			wheels[1].steerAngle=steerAngleOut;			
		}
        Quaternion newRotation = wheelQuaternion;
        newRotation *= Quaternion.Euler(new Vector3(0,0,-wheelRotation*inputs.steering));
        wheelObject.transform.localRotation = newRotation;
		
		// Aerodynamic drag & downforce
		float force=forwardVelocity*forwardVelocity*dragCoef;
		body.AddForce(body.transform.forward*-force);
		float downForce=downforce*forwardVelocity*forwardVelocity;
		body.AddForce(body.transform.up*-downForce);		

		// Antiroll bars
		for(int i=0;i<2;i++)
		{
			WheelHit hit;
			wheels[i*2].GetGroundHit(out hit);
			bool groundedL=wheels[i*2].isGrounded;
			float travelL,travelR;
			if(groundedL) travelL=wheels[i*2].transform.InverseTransformPoint(hit.point).y-wheels[i*2].radius;
			else travelL=1;
			wheels[i*2+1].GetGroundHit(out hit);
			bool groundedR=wheels[i*2+1].isGrounded;
			if(groundedR) travelR=wheels[i*2+1].transform.InverseTransformPoint(hit.point).y-wheels[i*2+1].radius;
			else travelR=1;
			float antiRollForce=(travelL-travelR)*antiRoll;
			if(groundedL) body.AddForceAtPosition(transform.up*-antiRollForce,wheels[i*2].transform.position);
			if(groundedR) body.AddForceAtPosition(transform.up*antiRollForce,wheels[i*2+1].transform.position);
		}
		
		// Store old inputs
		oldInputs=inputs;
		
        // Update fake speed
        /*
        float tgtFakeSpeed = forwardVelocity / (railsControl ? railsControl.setSpeedKmh/3.6f : maxSpeed);
        if (tgtFakeSpeed < fakeSoundSpeed - fakeSoundInstantThresholdKmh / 3.6)
            fakeSoundSpeed = tgtFakeSpeed;
        float lerpVal= tgtFakeSpeed>fakeSoundSpeed ? Mathf.Pow(fakeSoundAcceleration,Time.fixedDeltaTime)
                                                : Mathf.Pow(fakeSoundBrakes,Time.fixedDeltaTime);
        fakeSoundSpeed -= frictionSound * fakeSoundBrakesSpeedFriction*Time.fixedDeltaTime;
        if (fakeSoundSpeed < 0) fakeSoundSpeed = 0;
        fakeSoundSpeed = Mathf.Lerp(tgtFakeSpeed, fakeSoundSpeed, lerpVal);
        float fakeSpeed = fakeSoundSpeed * maxSpeed;
        int newFakeGear;
        float fakeRPM = transmission.getMaxPossibleRPM(fakeSpeed, engine.getMaxRpm(), out newFakeGear);
        if(newFakeGear>fakeGear)
        {
            fakeGear = newFakeGear;
            //FMOD_StudioSystem.instance.PlayOneShot("event:/SFX/Car Mechanics/carGearUp", transform.position);
        }
        if(newFakeGear<fakeGear)
        {
            fakeGear = newFakeGear;
            //FMOD_StudioSystem.instance.PlayOneShot("event:/SFX/Car Mechanics/carGearDown", transform.position);
        }*/
        float fakeRPM = Mathf.Lerp(engine.getMaxRpm(),rpm,transmission.isEngaged());


        // Update overRev
        //fakeRPM = rpm;
        if(rpm>engine.getMaxRpm()*0.97 && transmission.getCurrentGear()==0)
        {
            fakeRPM=Mathf.Lerp(engine.getMaxRpm(),engine.getMaxRpm()*0.8f,(Mathf.Sin(overRevTime*2*Mathf.PI/overRevPeriod)+1)/2);
            overRevTime += Time.fixedDeltaTime;
        }
        else overRevTime=0;
        //Debug.Log(overRevTime);

        // Update sounds
        float frictionSound = Mathf.Abs(inputs.steering);
        if (fakeRPM < engine.getMinRpm()) fakeRPM = engine.getMinRpm();
        if (!isOnGround()) fakeRPM = engine.getMaxRpm();
        float soundRpm=fakeRPM*ENGINE_SOUND_MAX_RPM/engine.getMaxRpm();
        //float soundRpm = rpm * ENGINE_SOUND_MAX_RPM / engine.getMaxRpm();
        engineRPM.setValue(soundRpm);
        tiresGround.setValue(isOnGround() ? 1 : 0);
        tiresFriction.setValue(frictionSound);
        tiresSpeed.setValue(forwardVelocity / maxSpeed);
        engineSpeed.setValue(forwardVelocity*3.6f);
        rumbleSpeed.setValue(forwardVelocity * 3.6f);
        engineLoad.setValue(inputs.throttle>0.5 ? 1 : 0);
        //Debug print
		/*if(nbUpdates%10==0)
		{
			Debug.Log((int)forwardVelocity*3.6+" "+(int)rpm+" "+transmission.getCurrentGear());
		}*/
	}

    void OnValidate()
    {
        if(Application.isPlaying) updateValues ();
    }
		
	public void updateValues()
	{
		// Get components
		engine = gameObject.GetComponent<Engine> ();
		if (engine == null)
		{
			Debug.LogError ("Car : no engine attached");
			return;
		}
		body=transform.FindChild("Body").GetComponent<Rigidbody>();
		CarControl[] controls=gameObject.GetComponents<CarControl>();
		if(controls.Length==0)
		{
			Debug.LogError ("Car : no control attached");
			return;
		}
		control=controls[0];
		control.init(0);
        railsControl = gameObject.GetComponent<RailsControl>();
		transmission=gameObject.GetComponent<Transmission>();
		if(transmission==null)
		{
			Debug.LogError ("Car : no transmission attached");
		}	

		wheels=new WheelCollider[4];
		wheels[0]=transform.FindChild("Body").FindChild("WheelFL").GetComponent<WheelCollider>();
		wheels[1]=transform.FindChild("Body").FindChild("WheelFR").GetComponent<WheelCollider>();
		wheels[2]=transform.FindChild("Body").FindChild("WheelRL").GetComponent<WheelCollider>();
		wheels[3]=transform.FindChild("Body").FindChild("WheelRR").GetComponent<WheelCollider>();
		centerOfMass=transform.FindChild("Body").FindChild("CenterOfMass").transform.localPosition;
		
		// Compute values
		float maxPower=engine.getMaxPower();
		maxSpeed=maxSpeedKmh/3.6f;
		mass=body.mass;
		dragCoef=(Mathf.Sqrt(maxSpeed*maxSpeed+ maxPower*2*0.01f/mass) - maxSpeed)*100 / (maxSpeed*maxSpeed);
		wheelRadius=wheels[0].radius;
		wheelRadius*=transform.FindChild("Body").FindChild("WheelFR").transform.lossyScale.y;
		engine.updateValues ();
        transmission.updateValues();
		acceleration2Torque=mass*wheelRadius;
		brakeTorque=brakeDecceleration*acceleration2Torque/2;
		wheelBase=body.transform.localScale.z;
		wheelTrack=body.transform.localScale.x;

        // Update center of weight
		body.centerOfMass=centerOfMass;
	}

	// Private methods
    private bool isOnGround()
    {
        bool ret = false;
        for (int i = 0; i < 4; i++) if (wheels[i].isGrounded) ret = true;
        return ret;
    }

    public void InstantSetSpeedKmh(float speedKmh)
    {
        InstantSetSpeed(speedKmh / 3.6f);
    }

    public void InstantSetSpeed(float speed)
    {
        if(isOnGround()) body.velocity = getForwardVector() * speed;
    }

	// Public getters
	public float getForwardVelocity()
	{
		Vector3 velocity=body.GetRelativePointVelocity(new Vector3(0,0,0));
		return Vector3.Dot(velocity,body.transform.forward);
	}
	public float getForwardVelocityKmh()
	{
        return getForwardVelocity() * 3.6f;
	}

    public Vector3 getVelocity()
    { 
		return body.GetRelativePointVelocity(new Vector3(0,0,0));
    }

	public Vector3 getPosition()
	{
		return body.transform.position;
	}

	public Vector3 getRightVector()
	{
		return body.transform.right;
	}

	public Vector3 getForwardVector()
	{
		return body.transform.forward;
	}

    public Vector3 getUpVector()
    {
        return body.transform.up;
    }
	
}


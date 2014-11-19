using UnityEngine;
using System.Collections;

public class Car : MonoBehaviour 
{

	// Inspector visible variables
	public float maxSpeedKmh=300; // km/h
	public float fwd; // Front wheels drive [0;1] 0=RWD 1=FWD
	public float brakeDecceleration=10; // m/s²
	public float brakesRepartition=0.6f; // 0=rear, 1=front
	
	// Components
	private Engine engine;
	private CarControl control;
	private Rigidbody body;
	private WheelCollider[] wheels;
	private Transmission transmission;
	
	// Private attributes
	private float dragCoef;
	private float mass;
	private float maxSpeed; // unit/s (aka m/s)
	private float wheelRadius;
	private int nbUpdates=0;
	private float acceleration2Torque;
	private float brakeTorque;
	private CarControl.CarInputs oldInputs;
	
	// MonoBehaviour methods
	void Start ()
	{
		updateValues ();
	}

	void FixedUpdate ()
	{
		CarControl.CarInputs inputs=control.getInputs();
		float dt=Time.fixedDeltaTime;
		int freq=(int) (1.0f/dt);
		nbUpdates=(nbUpdates+1)%freq;
		
		// Update transmission
		if(inputs.upshift && !oldInputs.upshift) transmission.upshift();
		if(inputs.downshift && !oldInputs.downshift) transmission.downshift();
				
		// Compute forward torque
		Vector3 velocity=body.GetRelativePointVelocity(new Vector3(0,0,0));
		float forwardVelocity=Vector3.Dot(velocity,new Vector3(0,0,1));
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
			if(!transmission.isDisengaged()) wheels[i].motorTorque=torque*accelMult/2;
		}		
		
		oldInputs=inputs;
		
		// Debug print
		if(nbUpdates%10==0)
		{
			Debug.Log((int)forwardVelocity*3.6+" "+(int)rpm);
		}
	}

	void OnValidate()
	{
		updateValues ();
	}

	// Private methods
	void updateValues()
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
		
		// Compute values
		float maxPower=engine.getMaxPower();
		maxSpeed=maxSpeedKmh/3.6f;
		mass=body.mass;
		dragCoef=(Mathf.Sqrt(maxSpeed*maxSpeed + maxPower) - maxSpeed) / (maxSpeed*maxSpeed);
		wheelRadius=wheels[0].radius;
		wheelRadius*=transform.FindChild("Body").FindChild("WheelFR").transform.lossyScale.y;
		engine.updateValues ();
		acceleration2Torque=mass*wheelRadius;
		brakeTorque=brakeDecceleration*acceleration2Torque/2;
	}
}


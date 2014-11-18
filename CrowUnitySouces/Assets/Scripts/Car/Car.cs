using UnityEngine;
using System.Collections;

public class Car : MonoBehaviour 
{

	// Inspector visible variables
	public float maxSpeedKmh=300; // km/h
	public float fwd; // Front wheels drive [0;1] 0=RWD 1=FWD
	
	// Private attributes
	private Engine engine;
	private CarControl control;
	private Rigidbody body;
	private float dragCoef;
	private float mass;
	private float maxSpeed; // unit/s (aka m/s)
	private float wheelRadius;
	private WheelCollider[] wheels;
	private int nbUpdates=0;

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
		
		// Compute forward torque
		float power=engine.getMaxPower ();
		Vector3 velocity=body.GetRelativePointVelocity(new Vector3(0,0,0));
		float forwardVelocity=Vector3.Dot(velocity,new Vector3(0,0,1));
		float acceleration=
			(Mathf.Sqrt(forwardVelocity*forwardVelocity+dt*power*2/mass)-forwardVelocity)/dt;
		float force=acceleration*mass;
		float torque=force*wheelRadius;
		
		// Apply torque to wheels
		for(int i=0;i<4;i++)
		{
			float mult= i<2 ? fwd : 1-fwd;
			wheels[i].motorTorque=torque*mult;	
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
	}
}

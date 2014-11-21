using UnityEngine;
using System.Collections;

public class RailsControl : CarControl
{

	public RoadChunk chunk;
	public bool stickToRails=true;
	public float currentRail=0;
	public float changeSpeed=1;
	public float setSpeedKmh=100;
	public float minSpeedKmh=50;
	public float pedalsInertia=0.1f;
	
	private Car car;
	private float throttleBrake=0; // 1=full throttle, -1=full brake
	
	void Start ()
	{
		car = gameObject.GetComponent<Car> ();
		if (car == null)
		{
			Debug.LogError ("AutomaticGearbox : no car attached");
			return;
		}
	}
	
	void FixedUpdate ()
	{
		// Manage speed
		float wantThrottleBrake=0;
		float brakeCommand=Input.GetAxis("Brake");
		float speedKmh=car.getForwardVelocity()*3.6f;
		
		// Determine the target pedals position
		if(speedKmh<minSpeedKmh)
		{
			wantThrottleBrake=1;
		}
		else if(speedKmh>setSpeedKmh)
		{
			wantThrottleBrake=-1;
		}
		else if(brakeCommand>0)
		{
			wantThrottleBrake=-brakeCommand;
		}
		else wantThrottleBrake=1;
		
		// Move smoothly the pedals
		float percent=Mathf.Pow(pedalsInertia,Time.fixedDeltaTime);
		throttleBrake=Mathf.Lerp(throttleBrake,wantThrottleBrake,percent);
	}

	public override CarInputs getInputs()
	{
		CarInputs ret;
		
		// Apply pedals inputs
		ret.throttle=throttleBrake>0 ? throttleBrake : 0;
		ret.brake=throttleBrake<0 ? -throttleBrake : 0;
		
		// Gear shift, in case of there isn't an automatic gearbox 
		ret.upshift=Input.GetAxis ("Upshift")>0;
		ret.downshift=Input.GetAxis ("Downshift")>0;		
		
		
		
		
		ret.steering=Input.GetAxis("Steering");
		return ret;
	}
}

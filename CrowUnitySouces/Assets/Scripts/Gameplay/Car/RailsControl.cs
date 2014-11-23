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
	public float steeringInertia=0.1f;
	public float targetDistMultiplier=0.5f; // Unit = seconds

	private Car car;
	private float throttleBrake=0; // 1=full throttle, -1=full brake
	private float steering=0;
	private float chunkProgress=0;
	private Vector3 target;

	
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

		// Steering
		updateProgress();
		Vector3 diff=target-car.getPosition();
		float rightDiff=Vector3.Dot(

			
	}

	public override CarInputs getInputs()
	{
		CarInputs ret;
		
		// Apply inputs
		ret.throttle=throttleBrake>0 ? throttleBrake : 0;
		ret.brake=throttleBrake<0 ? -throttleBrake : 0;
		ret.steering=steering;

		// Gear shift, in case of there isn't an automatic gearbox 
		ret.upshift=Input.GetAxis ("Upshift")>0;
		ret.downshift=Input.GetAxis ("Downshift")>0;
		
		return ret;
	}
	
	private void updateProgress()
	{
		Vector3 carPos=car.getPosition();
		float wantedTargetDist=car.getForwardVelocity()*targetDistMultiplier;
		float currentTargetDist=Vector3.Distance(carPos,target);
		if(currentTargetDist<wantedTargetDist)
		{
			float minProgress=chunkProgress;
			float maxProgress=chunkProgress+0.1f;
			Vector3 maxPos=fakeRails (currentRail,maxProgress);
			float dist=Vector3.Distance(carPos,maxPos);
			while(dist<wantedTargetDist)
			{
				maxProgress+=0.1f;
				maxPos=fakeRails (currentRail,maxProgress);
				dist=Vector3.Distance(carPos,maxPos);
			}
			for(int i=0;i<4;i++)
			{
				float midProgress=(minProgress+maxProgress)/2;
				Vector3 midPos=fakeRails (currentRail,midProgress);
				dist=Vector3.Distance(carPos,midPos);
				if(dist<wantedTargetDist)
					minProgress=midProgress;
				else maxProgress=midProgress;
			}
			chunkProgress=(minProgress+maxProgress)/2;
			target=fakeRails (currentRail,chunkProgress);
		}
	}
		
	private void OnValidate()
	{
		target=fakeRails (currentRail,chunkProgress);
	}

	// Temporary function to test fixed rails following
	private Vector3 fakeRails(float rail, float position)
	{
		return new Vector3(-5+5*rail,0+500*position);
	}
}

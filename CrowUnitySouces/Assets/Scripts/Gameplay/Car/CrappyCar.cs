using UnityEngine;
using System.Collections;

public class CrappyCar : MonoBehaviour {

	public float power=1;
	public float brakesDecceleration=20;
	public float maxSpeed=70;
	public float setSpeed=70*0.8f;
	public float minSpeed=15;
	public float latSpeed=5;
	public float latAccel=20;

	private const int NB_RAILS=4;
	private const int RAILS_POS=0;
	private const int RAILS_SPACING=4;

	private float speed=0;
	private float dragCoef;
	private float curLatSpeed=0;

	struct InputsValues
	{
		public float moveRight;
		public float acceleration;
	};
	private InputsValues currentInputs;
	private InputsValues lastInputs;
	private int tgtRail=1;

	void Start ()
	{
		OnValidate ();
	}
	
	void FixedUpdate ()
	{
		currentInputs.moveRight=Input.GetAxis("Steering");
		currentInputs.acceleration=Input.GetAxis("Throttle")-Input.GetAxis ("Brake");
		if(currentInputs.moveRight<0 && lastInputs.moveRight>=0 && tgtRail>0) tgtRail--;
		if(currentInputs.moveRight>0 && lastInputs.moveRight<=0 && tgtRail<NB_RAILS-1) tgtRail++;

		Vector3 location=transform.position;
		float posX=location.x;
		float diff=RAILS_POS+tgtRail*RAILS_SPACING-posX;
		float dist=0;
		float fixedUpdateRate=1.0f/Time.fixedDeltaTime;
		if(diff==0)
		{
			curLatSpeed=0;
		}
		else if(diff>0)
		{
			if(curLatSpeed*curLatSpeed/latAccel>diff)
			{
				curLatSpeed-=latAccel/fixedUpdateRate;
				if(curLatSpeed<0) curLatSpeed=0;
			}
			else
			{
				curLatSpeed+=latAccel/fixedUpdateRate;
				if(curLatSpeed>latSpeed) curLatSpeed=latSpeed;
			}
			dist=curLatSpeed/fixedUpdateRate;
			if(dist>diff) dist=diff;
		}
		else if(diff<0)
		{
			if(curLatSpeed*curLatSpeed/latAccel>-diff)
			{
				curLatSpeed+=latAccel/fixedUpdateRate;
				if(curLatSpeed>0) curLatSpeed=0;
			}
			else
			{
				curLatSpeed-=latAccel/fixedUpdateRate;
				if(curLatSpeed<-latSpeed) curLatSpeed=-latSpeed;
			}
			dist=curLatSpeed/fixedUpdateRate;
			if(dist<diff) dist=diff;
		}
		location+=new Vector3(dist,0,0);
		transform.position=location;

		float yaw=Mathf.Rad2Deg*Mathf.Atan2(curLatSpeed,speed);
		Vector3 rotation=transform.rotation.eulerAngles;
		rotation.y=yaw;
		transform.rotation=Quaternion.Euler(rotation);
		
		speed-=dragCoef*speed*speed;
		if(speed<0) speed=0;

		if(currentInputs.acceleration==-1 && speed>minSpeed)
		{
			speed-=brakesDecceleration/fixedUpdateRate;
			if(speed<0) speed=0;
		}
		else
		{
			if(speed<setSpeed) speed=Mathf.Sqrt(speed*speed+power);
		}
		transform.position=transform.position+(new Vector3(0,0,speed/fixedUpdateRate));

		lastInputs=currentInputs;
	}

	void OnValidate()
	{
		dragCoef=(Mathf.Sqrt(maxSpeed*maxSpeed+power)-maxSpeed)/(maxSpeed*maxSpeed);
	}
}

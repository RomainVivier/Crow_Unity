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
    public float steeringBaseSpeed = 10f;
	public float targetDistMultiplier=0.5f; // Unit = seconds
	public float steeringDeadZone=1;
	public float steeringFullZone=2;

	private Car car;
	private Rails rails;
	private float throttleBrake=0; // 1=full throttle, -1=full brake
	private float steering=0;
	private float chunkProgress=0;
	private Vector3 target;
    private float targetSpeed;
	private float oldSteeringInput=0;
	private int targetRail=0;
	private int nbUpdates=0;

    //Keybinding
    private float m_steering;
    private float m_brake;

    public float Steering
    {
        set { m_steering = value; }
    }

	void Start ()
	{
		car = gameObject.GetComponent<Car> ();
		if (car == null)
		{
			Debug.LogError ("AutomaticGearbox : no car attached");
			return;
		}

        //ajout lors de la mise en place des rails et de la generation de chunk
        rails = chunk.GetComponent<Rails>();
        target = rails.getPoint(currentRail, chunkProgress);

        //KeyBinder.Instance.DefineActions("Steering", new AxisActionConfig(KeyType.Movement, 0, (value) => { m_steering = value; }));
        //KeyBinder.Instance.DefineActions("Brake", new AxisActionConfig(KeyType.Movement, 0, (value) => { m_brake = value; }));

        //code pour changer de rails avec un click de zone droite/gauche
        TouchManager.Instance._touchStart +=
            () =>
            {
#if UNITY_STANDALONE
                if (Input.mousePosition.y > (Screen.height / 2))
                    m_steering = Input.mousePosition.x > (Screen.width / 2) ? 1f : -1f;
#elif UNITY_ANDROID
                if (Input.mousePosition.y > (Screen.height / 2))
                    m_steering = Input.touches[0].position.x > (Screen.width/2) ? 1f : -1f;
#endif
            };

//        TouchManager.Instance._touchEnd += () => { if (m_steering != 0f) m_steering = 0f; };


        //code pour le swipe
        //TouchManager.Instance._swipeLeft += () => { m_steering = 1f; };
        //TouchManager.Instance._swipeRight += () => { m_steering = -1f; };

	}
	
	void FixedUpdate ()
	{
		nbUpdates++;
		
		// Manage speed
		float wantThrottleBrake=0;
        //float brakeCommand = Input.GetAxis("Brake");
        float brakeCommand = m_brake;
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
		throttleBrake=Mathf.Lerp(wantThrottleBrake,throttleBrake,percent);

		// Move from one rail to another
        //float curSteeringInput=Input.GetAxis("Steering");
        float curSteeringInput = m_steering;
		if(stickToRails)
		{
			if(curSteeringInput>0.1f && oldSteeringInput<=0.1f) ShiftRail(1);
            if (curSteeringInput < -0.1f && oldSteeringInput >= -0.1f) ShiftRail(-1);
			if(targetRail!=currentRail)
			{
				if(targetRail>currentRail)
				{
					currentRail+=changeSpeed*Time.fixedDeltaTime;
					if(targetRail<currentRail) currentRail=targetRail;
				}
				else if(targetRail<currentRail)
				{
					currentRail-=changeSpeed*Time.fixedDeltaTime;
					if(targetRail>currentRail) currentRail=targetRail;				
				}
			}
		}
		else
		{
            ShiftRail(curSteeringInput*changeSpeed*Time.fixedDeltaTime);
		}

		// Steering
		updateProgress();
		Vector3 diff=target-car.getPosition();
		float rightDiff=Vector3.Dot(car.getRightVector(),diff);
		float rightDiffAbs=Mathf.Abs(rightDiff);
		float wantSteering=0;
		if(rightDiffAbs>steeringDeadZone)
		{
			wantSteering=rightDiff>0 ? 1 : -1;
			if(rightDiffAbs<steeringFullZone)
				wantSteering*=(rightDiffAbs-steeringDeadZone)/(steeringFullZone-steeringDeadZone);
		}
        if(steering<wantSteering)
        {
            steering += Time.fixedDeltaTime * steeringBaseSpeed;
            if (steering > wantSteering) steering = wantSteering;
        }
        if(steering>wantSteering)
        {
            steering -= Time.fixedDeltaTime * steeringBaseSpeed;
            if (steering < wantSteering) steering = wantSteering;
        }
		percent=Mathf.Pow(steeringInertia,Time.fixedDeltaTime);
		steering=Mathf.Lerp(wantSteering,steering,percent);
		
		
		oldSteeringInput=curSteeringInput;
		
		// Debug print
		/*if(nbUpdates%10==0)
		{
			Debug.Log (chunkProgress +" "+rightDiff+" "+steering);
			GameObject.Find ("Sphere").transform.position=target;
		}*/

	}

	public override CarInputs getInputs()
	{
		CarInputs ret;
		
		// Apply inputs
		ret.throttle=throttleBrake>0 ? throttleBrake : 0;
		ret.brake=throttleBrake<0 ? -throttleBrake : 0;
        ret.steering = steering;

		// Gear shift, in case of there isn't an automatic gearbox 
		ret.upshift=Input.GetAxis ("Upshift")>0;
		ret.downshift=Input.GetAxis ("Downshift")>0;
		
		return ret;
	}
	
	private void updateProgress()
	{
		Vector3 carPos=car.getPosition();
		Vector3 forward=car.getForwardVector();
		Vector3 diff=target-carPos;
		float wantedTargetDist=car.getForwardVelocity()*targetDistMultiplier;
		float currentTargetDist=Vector3.Dot (diff,forward);;
		if(currentTargetDist<wantedTargetDist)
		{
			float minProgress=chunkProgress;
			float maxProgress=chunkProgress+0.1f;
			Vector3 maxPos=rails.getPoint(currentRail,maxProgress);
			float dist=Vector3.Dot (maxPos-carPos,forward);
			while(dist<wantedTargetDist && maxProgress<1)
			{
				maxProgress+=0.1f;
				maxPos=rails.getPoint(currentRail,maxProgress);
				dist=Vector3.Dot(maxPos-carPos,forward);
			}
			for(int i=0;i<8;i++)
			{
				float midProgress=(minProgress+maxProgress)/2;
				Vector3 midPos=rails.getPoint(currentRail,midProgress);
				dist=Vector3.Dot(midPos-carPos,forward);
				if(dist<wantedTargetDist)
					minProgress=midProgress;
				else maxProgress=midProgress;
			}
			chunkProgress=(minProgress+maxProgress)/2;
			target=rails.getPoint(currentRail,chunkProgress);
            targetSpeed = rails.getSpeed(currentRail, chunkProgress,setSpeedKmh/3.6f);
			if(chunkProgress>=1) gotoNextChunk();
		}
	}
		
	private void gotoNextChunk()
	{
		if(chunk.NextChunk!=null)
		{
			int oldNbRails=rails.nbRails;
			chunk=chunk.NextChunk;
			rails=chunk._rails;
			int newNbRails=rails.nbRails;
			currentRail=(currentRail-(oldNbRails-1)/2f)*(newNbRails-1)/(oldNbRails-1)+(newNbRails-1)/2f;
			targetRail=Mathf.RoundToInt((targetRail-(oldNbRails-1)/2)*(newNbRails-1)/(oldNbRails-1)+(newNbRails-1)/2f);
			chunkProgress=0;
			updateProgress();
		}
	}
    
    public void ShiftRail(float delta)
    {
        if(stickToRails)
        {
            targetRail+=(int)delta;
            if(targetRail<0) targetRail=0;
            if (targetRail > rails.getNbRails() - 1) targetRail = rails.getNbRails() - 1;
        }
        else
        {
            currentRail += delta;
            if(currentRail<0) currentRail=0;
			if(currentRail>rails.getNbRails()-1) currentRail=rails.getNbRails()-1;
        }
    }

    //private void OnValidate()
    //{
    //    rails = chunk.GetComponent<Rails>();
    //    target = rails.getPoint(currentRail, chunkProgress);
    //}

}

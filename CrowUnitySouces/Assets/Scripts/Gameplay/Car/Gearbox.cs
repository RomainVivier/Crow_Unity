using UnityEngine;
using System.Collections;

public class Gearbox : Transmission
{
	public int nbGears=5;
	public float topGearRatio=1.25f; // [1;∞[ 1=highest top speed possible
	public float firstToSecondRatio=0.5f;
	public float nthToTopGearRatio=0.8f;
    public float declutchTime = 0.1f;
    public float clutchTime = 0.1f;
    public float shiftTime = 0.3f;
	
	private int currentGear=0;
	private float[] ratios;
	private Car car;
	private Engine engine;
	private bool disengaged=false;
	private float timeToReengage=0;
    private int lockedGear = -1;

	public override float getSpeed2Rpm()
	{
		return ratios[currentGear];
	}
	
	public override void updateValues()
	{
		car=gameObject.GetComponent<Car> ();
		if (car == null)
		{
			Debug.LogError ("Gearbox : no car attached");
			return;
		}
		engine = gameObject.GetComponent<Engine> ();
		if (engine == null)
		{
			Debug.LogError ("Gearbox : no engine attached");
			return;
		}
		if(nbGears<3)
		{
			Debug.LogError ("Gearbox : must have at least 3 gears");
			return;
		}
		
		ratios=new float[nbGears];
		ratios[nbGears-1]=topGearRatio*engine.getMaxPowerRpm()/(car.maxSpeedKmh/3.6f);
		for(int i=nbGears-2;i>=0;i--)
		{
			float pos=i/(nbGears-2.0f);
			float ratio=Mathf.Lerp(firstToSecondRatio,nthToTopGearRatio,pos);
			ratios[i]=ratios[i+1]/ratio;
		}
	}
	
	public override float isEngaged()
	{
        if (disengaged)
        {
            if (timeToReengage < clutchTime) return 1 - (timeToReengage / clutchTime);
            if (shiftTime - timeToReengage < declutchTime) return (shiftTime - timeToReengage) / clutchTime;
            return 0;
        }
        else return 1;
	}
	
	public override void upshift()
	{
		if(currentGear<nbGears-1)
		{
			currentGear++;
			disengaged=true;
			timeToReengage=shiftTime;
		}
	}
	
	public override void downshift()
	{
		if(currentGear>0 && lockedGear==-1)
		{
			currentGear--;
			disengaged=true;
			timeToReengage=shiftTime;
		}
	}
	
	void FixedUpdate()
	{
		if(disengaged)
		{
			timeToReengage-=Time.fixedDeltaTime;
			if(timeToReengage<=0) disengaged=false;
		}
	}
	
	public override bool canUpshift()
	{
		return currentGear<nbGears-1;
	}
	public override bool canDownshift()
	{
		return currentGear>0;
	}
	public override float getNextSpeed2Rpm() 
	{
		return ratios[currentGear+1];
	}
	public override float getPreviousSpeed2Rpm()
	{
		return ratios[currentGear-1];
	}
	
    public override float getMaxPossibleRPM(float speed, float maxRPM, out int newFakeGear)
    {
        int gear = 0;
        float rpm = speed * ratios[gear];
        while(rpm>maxRPM && gear<nbGears-1)
        {
            gear++;
            rpm = speed * ratios[gear];
        }
        newFakeGear = gear;
        return rpm;
    }

	public override int getCurrentGear()
	{
		return currentGear;
	}

    public override void lockGear(int gear)
    {
        lockedGear = gear;
        if(gear!=-1) currentGear = gear;
    }
}

using UnityEngine;
using System.Collections;

public class Gearbox : Transmission
{
	public int nbGears=5;
	public float topGearRatio=0.7f; // [1;∞[ 1=highest top speed possible
	public float firstToSecondRatio=0.6f;
	public float nthToTopGearRatio=0.9f;
	public float shiftTime=0.2f;
	
	private int currentGear=0;
	private float[] ratios;
	private Car car;
	private Engine engine;
	private bool disengaged=false;
	private float timeToReengage=0;
	
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
		ratios[nbGears-1]=engine.getMaxPowerRpm()/(car.maxSpeedKmh/3.6f);
		for(int i=nbGears-2;i>=0;i--)
		{
			float pos=i/(nbGears-2.0f);
			float ratio=Mathf.Lerp(firstToSecondRatio,nthToTopGearRatio,pos);
			ratios[i]=ratios[i+1]/ratio;
		}
	}
	
	public override bool isDisengaged()
	{
		return disengaged;
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
		if(currentGear>0)
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
}

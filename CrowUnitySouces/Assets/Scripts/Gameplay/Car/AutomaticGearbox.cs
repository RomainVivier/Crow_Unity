using UnityEngine;
using System.Collections;

public class AutomaticGearbox : CarControl {

	public float upshiftLag=0.3f;
	public float downshiftLag=0.1f;
	public float sensitivity=0.9f;
	//public float upshiftRpm=;
	
	private Car car;
	private Engine engine;
	private Transmission transmission;
	private bool waiting=false;
	private float waitingTime=0;
	
	//public float upshift
	// Use this for initialization
	void Start ()
	{
		car = gameObject.GetComponent<Car> ();
		if (car == null)
		{
			Debug.LogError ("AutomaticGearbox : no car attached");
			return;
		}
		engine = gameObject.GetComponent<Engine> ();
		if (engine == null)
		{
			Debug.LogError ("AutomaticGearbox : no engine attached");
			return;
		}
		transmission = gameObject.GetComponent<Transmission> ();
		if (transmission == null)
		{
			Debug.LogError ("AutomaticGearbox : no transmission attached");
			return;
		}
	}
	
	public void FixedUpdate()
	{
		if(waiting)
		{
			waitingTime-=Time.fixedDeltaTime;
		}
	}
	
	public override CarInputs getInputs()
	{
		CarInputs ret=nextControl.getInputs();
		ret.downshift=false;
		ret.upshift=false;
		float velocity=car.getForwardVelocity();
		float curPower=engine.getPower(transmission.getSpeed2Rpm()*velocity,ret.throttle);
		bool wantUpshift=false,wantDownshift=false;
		if(transmission.canUpshift())
		{
			float nextPower=engine.getPower (transmission.getNextSpeed2Rpm()*velocity,1)*sensitivity;
			if(nextPower>curPower) wantUpshift=true;
		}
		if(transmission.canDownshift())
		{
			float prevPower=engine.getPower (transmission.getPreviousSpeed2Rpm()*velocity,1)*sensitivity;
			if(prevPower>curPower) wantDownshift=true;
		}
		//Debug.Log (wantUpshift+" "+wantDownshift);
		
		if(wantUpshift != wantDownshift)
		{
			if(waiting)
			{
				if(waitingTime<=0)
				{
					ret.upshift=wantUpshift;
					ret.downshift= wantDownshift;
					waiting=false;
				}
			}
			else
			{
				waiting=true;
				waitingTime= wantUpshift ? upshiftLag : downshiftLag;
			}
		}
		else
		{
			waiting=false;
		}
		return ret;
	}
}

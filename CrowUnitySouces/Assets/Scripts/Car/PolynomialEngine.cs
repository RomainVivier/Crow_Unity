using UnityEngine;
using System.Collections;

public class PolynomialEngine : Engine {

	public float maxPowerKw=75;
	public float maxPowerRpm=6000;
	public float minRpm=1500;
	public float powerMinRpmKw=35;
	public float maxRpm=7000;
	
	// Power curve=ax^3+bx^2+cx
	private float curveA=0,curveB=0,curveC=0;
	
	void Start()
	{
		updateValues ();
	}
	
	public override float getMaxPower()
	{
		return maxPowerKw*1000;
	}
	
	public override void updateValues()
	{

	}
	
	public override float getPower(float rpm, float throttle)
	{
		return 0;
	}
}

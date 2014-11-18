using UnityEngine;
using System.Collections;

public class SimpleEngine : Engine {

	public float maxPowerKw;

	public override float getMaxPower()
	{
		return maxPowerKw*1000;
	}
	
	public override float getPower(float rpm, float throttle)
	{
		return getMaxPower()*throttle;
	}
}

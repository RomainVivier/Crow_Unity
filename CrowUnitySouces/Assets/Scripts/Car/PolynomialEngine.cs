using UnityEngine;
using System.Collections;

public class PolynomialEngine : Engine {

	public float maxPowerKw=100;
	public float maxPowerRpm=6000;
	public float minRpm=1500;
	public float powerMinRpmKw=50;
	public float maxRpm=7000;
	public float engineBrake=0.001f;
	
	// Power curve=ax^3+bx^2+cx
	private float curveA=0,curveB=0,curveC=0;
	
	
	public override float getMaxPower()
	{
		return maxPowerKw*1000;
	}
	
	public override void updateValues()
	{
		// Solve this equation to compute a, b and c
		// 3ap²+2bp+c=0       (1)
		// ap^3+bp²+cp=m      (2)
		// al^3+bl²+cl=n      (3)
		// Where m=maxPower, n=powerMinRpm, p=maxPowerRpm, l=minRpm
		
		// (2)-(p/l)(3) = at+bu=o (4)
		float ratio=maxPowerRpm/minRpm;
		float o=maxPowerKw-powerMinRpmKw*ratio;
		float t=maxPowerRpm*maxPowerRpm*maxPowerRpm-minRpm*minRpm*minRpm*ratio;
		float u=maxPowerRpm*maxPowerRpm-minRpm*minRpm*ratio;
		
		// (2)-p(1) = av+bw=m (5)
		float v=maxPowerRpm*maxPowerRpm*maxPowerRpm*-2;
		float w=maxPowerRpm*maxPowerRpm*-1;
		
		// (4)-(u/w)5 : as=k, a=k/s
		float s=t-(u/w)*v;
		float k=o-(u/w)*maxPowerKw;
		curveA=k/s;
		
		curveB=(maxPowerKw-(curveA*v))/w;
		curveC=(-curveA*3)*maxPowerRpm*maxPowerRpm+(-curveB*2)*maxPowerRpm;
	}
	
	public override float getPower(float rpm, float throttle)
	{
		if(rpm<minRpm) return getPower (minRpm,throttle);
		else if(rpm>maxRpm) return getPower (maxRpm,0);
		else
		{
			float low=-engineBrake*rpm;
			float high=curveA*rpm*rpm*rpm+curveB*rpm*rpm+curveC*rpm;
			return Mathf.Lerp(low,high,throttle)*1000;
		}
	}
	
	public override float getMaxPowerRpm()
	{
		return maxPowerRpm;
	}
}

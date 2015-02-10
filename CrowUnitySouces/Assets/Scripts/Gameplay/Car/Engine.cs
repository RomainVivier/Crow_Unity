using UnityEngine;
using System.Collections;
using System;

public abstract class Engine : MonoBehaviour
{
	public abstract float getMaxPower(); // Returns the max power in w
	public virtual float getMaxPowerRpm() { return 0;}
	public abstract float getPower(float rpm, float throttle);
    public virtual float getMaxRpm() { return 1; }
    public virtual float getMinRpm() { return 0; }

	void Start()
	{
		updateValues ();
	}
	public virtual void updateValues() {}
}
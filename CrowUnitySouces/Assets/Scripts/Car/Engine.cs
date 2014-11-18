using UnityEngine;
using System.Collections;
using System;

public abstract class Engine : MonoBehaviour
{
	public abstract float getMaxPower(); // Returns the max power in w
	public abstract float getPower(float rpm, float throttle);
	public virtual void updateValues() {}
}
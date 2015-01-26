using UnityEngine;
using System.Collections;

public abstract class Transmission : MonoBehaviour
{
	public virtual bool isDisengaged() {return false;}
	public abstract float getSpeed2Rpm();
	public virtual void upshift() {}
	public virtual bool canUpshift() { return false;}
	public virtual bool canDownshift() { return false;}
	public virtual float getNextSpeed2Rpm() { return getSpeed2Rpm();}
	public virtual float getPreviousSpeed2Rpm() { return getSpeed2Rpm();}
	public virtual void downshift() {}
	public virtual void updateValues() {}
    public abstract float getMaxPossibleRPM(float speed, float maxRPM, out int newFakeGear);

	void Start()
	{
		updateValues ();
	}
	
}

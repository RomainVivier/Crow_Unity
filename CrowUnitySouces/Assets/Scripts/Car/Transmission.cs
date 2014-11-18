using UnityEngine;
using System.Collections;

public abstract class Transmission : MonoBehaviour
{
	public virtual bool isDisengaged() {return false;}
	public abstract float getSpeed2Rpm();
	public virtual void upshift() {}
	public virtual void downshift() {}
	public virtual void updateValues() {}
	
	void Start()
	{
		updateValues ();
	}
	
}

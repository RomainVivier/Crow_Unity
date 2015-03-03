using UnityEngine;
using System.Collections;


public abstract class CarControl : MonoBehaviour
{	
	public struct CarInputs
	{
		public float steering;
		public float throttle;
		public float brake;
		public bool upshift;
		public bool downshift;
	}
	
	protected CarControl nextControl;

	public abstract CarInputs getInputs();
    public abstract Vector3 getForwardTarget();
    public abstract Vector3 getTarget();

	public void init(int pos) // Position in the chain
	{
		CarControl[] controls=gameObject.GetComponents<CarControl>();
		if(controls.Length>pos+1)
		{
			nextControl=controls[pos+1];
			nextControl.init(pos+1);
		}
		else nextControl=null;	
	}
}

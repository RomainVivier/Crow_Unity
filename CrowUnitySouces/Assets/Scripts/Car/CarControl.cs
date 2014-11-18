using UnityEngine;
using System.Collections;


public abstract class CarControl : MonoBehaviour
{	
	public struct CarInputs
	{
		public float steering;
		public float throttle;
		public float brake;
	}
	
	CarControl nextControl;

	public abstract CarInputs getInputs();
		
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

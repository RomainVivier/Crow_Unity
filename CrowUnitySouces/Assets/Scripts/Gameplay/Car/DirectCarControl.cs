using UnityEngine;
using System.Collections;

public class DirectCarControl : CarControl {

	public override CarInputs getInputs()
	{
		CarInputs ret;
		ret.steering=Input.GetAxis("Steering");
		ret.throttle=Input.GetAxis("Throttle");
		ret.brake=Input.GetAxis("Brake");
		ret.upshift=Input.GetAxis ("Upshift")>0;
		ret.downshift=Input.GetAxis ("Downshift")>0;		
		return ret;
	}
}

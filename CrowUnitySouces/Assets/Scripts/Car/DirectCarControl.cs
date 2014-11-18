using UnityEngine;
using System.Collections;

public class DirectCarControl : CarControl {

	public override CarInputs getInputs()
	{
		CarInputs ret;
		ret.steering=Input.GetAxis("Steering");
		ret.throttle=Input.GetAxis("Throttle");
		ret.brake=Input.GetAxis("Brake");
		return ret;
	}
}

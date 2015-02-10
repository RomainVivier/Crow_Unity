using UnityEngine;
using System.Collections;

public class CameraShake : MonoBehaviour { 
	public bool Shaking; 

	private float ShakeDecay; 
	private float ShakeIntensity;

	private Vector3 OriginalPos;
	private Quaternion OriginalRot;
	
	void Start()
	{
		Shaking = false;    
	}
	
	
	// Update is called once per frame
	void Update () 
	{

		if(ShakeIntensity > 0)
		{
			transform.localPosition = OriginalPos + Random.insideUnitSphere * ShakeIntensity;
			transform.localRotation = new Quaternion(OriginalRot.x + Random.Range(-ShakeIntensity, ShakeIntensity)*.2f,
			                                    OriginalRot.y + Random.Range(-ShakeIntensity, ShakeIntensity)*.2f,
			                                    OriginalRot.z + Random.Range(-ShakeIntensity, ShakeIntensity)*.2f,
			                                    OriginalRot.w + Random.Range(-ShakeIntensity, ShakeIntensity)*.2f);
			
			ShakeIntensity -= ShakeDecay;
		}
		else if (Shaking)
		{
			transform.localPosition = OriginalPos;
			transform.localRotation = OriginalRot;
			Shaking = false;    
		}
	}
	
	public void DoShake()
	{
		if (!Shaking) {
			OriginalPos = transform.localPosition;
			OriginalRot = transform.localRotation;
		}
		ShakeIntensity = 0.07f;
		ShakeDecay = 0.003f;
		Shaking = true;
	}
}

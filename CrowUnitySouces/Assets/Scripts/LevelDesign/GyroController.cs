using UnityEngine;
using System.Collections;

public class GyroController : MonoBehaviour {

	public float _flareRange;
	public float _flareMaxBrightness;
	public float _leftFlareBrightnessMultiplier;
	public float _rightFlareBrightnessMultiplier;
	public float _flareDistanceExponent = 2.0f;

	LensFlare[] m_flares;


	// Use this for initialization
	void Start () {
		m_flares = GetComponentsInChildren<LensFlare>();
	}
	
	// Update is called once per frame
	void Update () {
		Camera cam = Camera.current;
		if(cam == null)
			cam = Camera.main;
		float distance = Vector3.Distance(cam.transform.position, transform.position);
		float distanceRatio = 1 - Mathf.Clamp(distance / _flareRange, 0, 1);
		distanceRatio = Mathf.Pow(distanceRatio, _flareDistanceExponent);
		float distanceBrightness = distanceRatio * _flareMaxBrightness;
		m_flares[0].brightness = distanceBrightness * _rightFlareBrightnessMultiplier;
		m_flares[1].brightness = distanceBrightness * _leftFlareBrightnessMultiplier;
	}
}

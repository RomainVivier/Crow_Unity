using UnityEngine;
using System.Collections;

public class BigBaddieController : MonoBehaviour {

	Vector3 m_target, m_startPosition;
	float m_interpolationTime = 1.0f;
	float m_interpolationTimer;

	// Use this for initialization
	void Start () {
		SetInterpolationTime (45.0f);
		MoveTo (new Vector3(1700, 0, -190));
		m_target = transform.position;
	}
	
	// Update is called once per frame
	void Update () {
		m_interpolationTimer += Time.deltaTime;
		float t = Mathf.Min(1, m_interpolationTimer / m_interpolationTime);
		transform.position = Vector3.Lerp (m_startPosition, m_target, t);
	}
	public void SetInterpolationTime(float time)
	{
		m_interpolationTime = time;
	}
	public void MoveTo(Transform target)
	{
		MoveTo (target.position);
	}
	public void MoveTo(Vector3 target)
	{
		m_startPosition = transform.position;
		m_target = target;
		m_interpolationTimer = 0;
	}

	public void Shoot()
	{
		GetComponent<Animator> ().SetTrigger ("Shoot");
	}
}

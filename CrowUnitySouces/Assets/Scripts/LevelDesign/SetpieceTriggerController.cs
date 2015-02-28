using UnityEngine;
using System.Collections;

public class SetpieceTriggerController : MonoBehaviour {

	BigBaddieController m_baddieController;

	// Use this for initialization
	void Start () {
		m_baddieController = GameObject.Find ("Baddie").GetComponent<BigBaddieController>();
	}

	public void SetBaddieInterpolationTime(float time)
	{
		m_baddieController.SetInterpolationTime (time);
	}
	public void SetBaddieTarget(Transform target)
	{
		m_baddieController.MoveTo (target);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}

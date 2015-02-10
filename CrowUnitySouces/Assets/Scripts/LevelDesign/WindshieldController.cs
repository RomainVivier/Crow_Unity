using UnityEngine;
using System.Collections;

public class WindshieldController : MonoBehaviour {


	public int _hp = 3;
	SpriteRenderer[] m_impacts;
	Animator m_lightAnimator;

	void Start()
	{
		m_impacts = GetComponentsInChildren<SpriteRenderer>();
		m_lightAnimator = GetComponentInChildren<Animator>();
	}

	public void Hit()
	{
		_hp--;

		if(_hp < 0)
			return;

		if(_hp < m_impacts.Length)
			m_impacts[_hp].enabled = true;
		if(_hp == 0)
			m_lightAnimator.SetTrigger("Blink");
	}
}

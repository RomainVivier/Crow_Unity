using UnityEngine;
using System.Collections;

public class WindshieldController : MonoBehaviour {


	public int _hp = 3;
	public bool _isInvincible;
	SpriteRenderer[] m_impacts;
	Animator m_lightAnimator;

	void Start()
	{
		m_impacts = GetComponentsInChildren<SpriteRenderer>();
		m_lightAnimator = GetComponentInChildren<Animator>();
	}

	public void Hit()
	{
		if (_isInvincible)
			return;

		_hp--;

		if(_hp < 0)
		{
			//GameOverController.Instance.Show();
			return;
		}

		if(_hp < m_impacts.Length)
			m_impacts[_hp].enabled = true;
		if(_hp == 0)
			m_lightAnimator.SetTrigger("Blink");
	
	}

    public void Kill()
    {
		if (_isInvincible)
			return;

		_hp=0;

	    //GameOverController.Instance.Show();
        for (int i = 0; i < m_impacts.Length; i++) m_impacts[i].enabled = true;

        if (_hp == 0)
            m_lightAnimator.SetTrigger("Blink");
    }
}

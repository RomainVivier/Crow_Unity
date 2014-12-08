using UnityEngine;
using System.Collections;

public class ButtonGadget : Gadget {

    public GameObject m_button;
    public float _animSpeed = 4;
    public float _relScale = 0.5f;
    private float m_animTime = 1;
    //private Vector3 baseScale;

	void Start ()
    {
        //baseScale = new Vector3(100f, 100f, 100f);//m_button.transform.localScale;	
	}
	
	public void Update ()
    {
        m_animTime += Time.deltaTime * _animSpeed;
        if (m_animTime > 1) m_animTime = 1;
        float animPos = m_animTime > 0.5f ? 2f * (1f - m_animTime) : m_animTime * 2f;
        m_button.transform.localScale = new Vector3(100f, 100f*Mathf.Lerp(1, _relScale, animPos),100f);
	}

    public override void Play()
    {
        base.Play();
        if (IsReady)
            m_animTime = 0;
    }
}

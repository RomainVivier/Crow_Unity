using UnityEngine;
using System.Collections;

public class ButtonGadget : Gadget {

    public Animator m_button;
    public float _animSpeed = 4;
    public float _relScale = 0.5f;
    private float m_animTime = 1;
    private Material unlitMat;
    private Material litMat;
    //private Vector3 baseScale;

	public virtual void Start ()
    {
        unlitMat=Resources.Load("ButtonUnlit", typeof(Material)) as Material;
        litMat=Resources.Load("ButtonLit", typeof(Material)) as Material;
        m_playSound = "event:/SFX/Buttons/ButtonSmall/ButtonPushSmall/buttonPushSmallValidated";
        m_cantPlaySound = "event:/SFX/Buttons/ButtonSmall/ButtonPushSmall/buttonPushSmallDenied";
    }
	
	public virtual void Update ()
    {
        //m_animTime += Time.deltaTime * _animSpeed;
        //if (m_animTime > 1) m_animTime = 1;
        //float animPos = m_animTime > 0.5f ? 2f * (1f - m_animTime) : m_animTime * 2f;
        //m_button.transform.localScale = new Vector3(100f, 100f*Mathf.Lerp(1, _relScale, animPos),100f);
        //m_button.GetComponent<MeshRenderer>().material = IsReady ? litMat : unlitMat; 
    }

    public override void Play()
    {
        base.Play();
        //if (IsReady)
            //m_animTime = 0;
		m_button.SetTrigger("Press");
    }

	public override void Stop()
	{
		base.Stop();
		m_button.SetTrigger("Unpress");
	}

}

using UnityEngine;
using System.Collections;

public class Suppository : Gadget
{

    #region Members

    public Animator _anim;
	//public float _delay = 2.7f;

    private Timer m_timer;

    #endregion 

    #region Monobehaviour

    public override void Awake()
    {
        //GadgetManager.Instance.Register("Suppository", this);
        m_timer = new Timer();
        base.Awake();
    }

    public override void Update()
    {
        if (m_timer.IsElapsedOnce)
        {
            Stop();
        }
        base.Update();
    }

    #endregion 

    #region Overrided Functions

    public override void Play()
    {
        base.Play();
        IsReady = false;

        m_timer.Reset(5f);
        //TODO Play sound here
		Invoke ("Insert", 3.3f);
        Invoke("End", 2.7f);
        FMOD_StudioSystem.instance.PlayOneShot("event:/Dialog/IA/AI Gadgets/AI_Suppo", transform.position);
    }

	void Insert()
	{
		_anim.SetTrigger("Suppository");
	}

    void End()
    {
		FinalScreenController.Instance.Show();
    }

    public override void Stop()
    {
        base.Stop();
        //IsReady = true;
    }

    #endregion 
}

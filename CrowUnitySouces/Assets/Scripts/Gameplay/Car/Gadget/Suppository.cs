using UnityEngine;
using System.Collections;

public class Suppository : Gadget
{

    #region Members

    public Animator _anim;
	public float _delay = 1.0f;

    private Timer m_timer;

    #endregion 

    #region Monobehaviour

    public override void Awake()
    {
        GadgetManager.Instance.Register("Suppository", this);
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

        m_timer.Reset(2f);
        //TODO Play sound here
		Invoke ("Insert", _delay);
		FinalScreenController.Instance.Show();
    }

	void Insert()
	{
		_anim.SetTrigger("Suppository");
	}

    public override void Stop()
    {
        base.Stop();
        IsReady = true;
    }

    #endregion 
}

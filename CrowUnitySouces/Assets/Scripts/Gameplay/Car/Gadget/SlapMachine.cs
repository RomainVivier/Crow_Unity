using UnityEngine;
using System.Collections;

public class SlapMachine : ButtonGadget {



    #region Members
    public Animator _cameraAnimator;

    private Timer m_timer;

    #endregion

    #region MonoBehaviour

    void Start()
    {
        base.Start();
        GadgetManager.Instance.Register("SlapMachine", this);
        m_timer = new Timer();
        base.Start();
    }

    void Update()
    {
        base.Update();
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

        m_timer.Reset(1f);
        FMOD_StudioSystem.instance.PlayOneShot("event:/SFX/Gadgets/Punch/gadgetPunchExecute", transform.position);
        int value = Random.Range(0,5);
        _cameraAnimator.SetTrigger("Slap_"+value);

    }

    public override void Stop()
    {
        base.Stop();
        IsReady = true;
    }

    #endregion

}

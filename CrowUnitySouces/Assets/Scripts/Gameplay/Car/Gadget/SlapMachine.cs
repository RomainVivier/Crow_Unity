using UnityEngine;
using System.Collections;

public class SlapMachine : Gadget {



    #region Members
    public Animator _cameraAnimator;

    private Timer m_timer;

	const float COOLDOWN = 5.0f;

    #endregion

    #region MonoBehaviour

    public override void Awake()
    {
        GadgetManager.Instance.Register("SlapMachine", this);
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

        m_timer.Reset(COOLDOWN);
        FMOD_StudioSystem.instance.PlayOneShot("event:/SFX/Gadgets/Punch/gadgetPunchExecute", transform.position);
        int value = Random.Range(4,5);
        _cameraAnimator.SetTrigger("Slap_"+value);

		//Juste pour la démo, tue-moi
		transform.GetChild (0).GetComponent<Animator>().SetTrigger("Slap");
		Invoke ("AddToScore", 0.4f);
    }
	void AddToScore()
	{
		Score.Instance.AddToScore(500);
	}

    public override void Stop()
    {
        base.Stop();
        IsReady = true;
    }

    #endregion

}

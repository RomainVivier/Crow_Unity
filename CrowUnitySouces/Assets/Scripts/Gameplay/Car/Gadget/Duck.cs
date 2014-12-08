using UnityEngine;
using System.Collections;

public class Duck : ButtonGadget
{

    private FMOD.Studio.EventInstance duckSound;
    private float currentCooldown = 0;
    private const int COOLDOWN = 5;

	void Start ()
    {
        duckSound=FMOD_StudioSystem.instance.GetEvent("event:/SFX/Gadgets/Duck/gadgetDuckExecute");
        GadgetManager.Instance.Register("Duck", this);
        base.Start();
    }
	
	void Update ()
    {
        /*if(currentCooldown==float.PositiveInfinity)
        {
            FMOD.Studio.PLAYBACK_STATE state;
            duckSound.getPlaybackState(out state);
            if (state == FMOD.Studio.PLAYBACK_STATE.STOPPED)
            {
                currentCooldown = COOLDOWN;
                Stop();
            }
        }*/
        currentCooldown -= Time.deltaTime;
        if(currentCooldown<=0 && !IsReady)
        {
            IsReady = true;
        }
        base.Update();
    }

    public override void Play()
    {
        base.Play();
        if (currentCooldown <= 0)
        {
            duckSound.start();
            currentCooldown = COOLDOWN;//float.PositiveInfinity;
            IsReady = false;
            Stop();
        }
    }
}

using UnityEngine;
using System.Collections;

public class Duck : Gadget
{

    private FMOD.Studio.EventInstance duckSound;
    private float currentCooldown = 0;
    private const int COOLDOWN = 2;

	public override void Awake ()
    {
        duckSound=FMOD_StudioSystem.instance.GetEvent("event:/SFX/Gadgets/Duck/gadgetDuckExecute");
        GadgetManager.Instance.Register("Duck", this);
        base.Awake();
    }
	
	public override void Update ()
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
			Stop();
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
            
        }
    }
}

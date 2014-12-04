using UnityEngine;
using System.Collections;

public class Duck : Gadget
{

    private FMOD.Studio.EventInstance duckSound;
    private float currentCooldown = 0;
    private const int COOLDOWN = 5;

	void Start ()
    {
        duckSound=FMOD_StudioSystem.instance.GetEvent("event:/SFX/Gadgets/Duck/gadgetDuckExecute");
        GadgetManager.Instance.Register("Duck", this);
	}
	
	void Update ()
    {
        if(currentCooldown==float.PositiveInfinity)
        {
            FMOD.Studio.PLAYBACK_STATE state;
            duckSound.getPlaybackState(out state);
            if (state == FMOD.Studio.PLAYBACK_STATE.STOPPED)
            {
                currentCooldown = COOLDOWN;
                IsReady = true;
            }
        }
        currentCooldown -= Time.deltaTime;
	}

    public override void Play()
    {
        if (currentCooldown <= 0)
        {
            duckSound.start();
            currentCooldown = float.PositiveInfinity;
            IsReady = false;
        }
    }
}

using UnityEngine;
using System.Collections;

public class Duck : Gadget
{

    private FMOD.Studio.EventInstance duckSound;
    private float currentCooldown = 0;
    private int nbUses = 0;

	public override void Awake ()
    {
        duckSound=FMOD_StudioSystem.instance.GetEvent("event:/SFX/Gadgets/Duck/gadgetDuckExecute");
        GadgetManager.Instance.Register("Duck", this);
        base.Awake();
    }
	
	public override void Update ()
    {
        currentCooldown -= Time.deltaTime;
        base.Update();
    }

    public override void Play()
    {
        base.Play();
        if (currentCooldown <= 0)
        {
            duckSound.start();
            IsReady = false;
            nbUses++;
            if(nbUses==2) (GameObject.FindObjectOfType<AI>() as AI).playDialog("AI Gadgets/AI_Duck");
        }
    }
}

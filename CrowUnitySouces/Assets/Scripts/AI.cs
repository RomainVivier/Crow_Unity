using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AI : MonoBehaviour
{
    public float blinkPeriod=0.2f;
    
    private FMOD.Studio.EventInstance currentSound=null;
    private bool playingSound = false;
    private List<GameObject> buttons;
    private float timeTillNextBlink = 0;

	void Start ()
    {
	
	}
	
	void FixedUpdate ()
    {
        if (currentSound == null) return;
        FMOD.Studio.PLAYBACK_STATE state;
        currentSound.getPlaybackState(out state);
        if (state==FMOD.Studio.PLAYBACK_STATE.STOPPED) timeTillNextBlink = blinkPeriod;
        else
        {
            timeTillNextBlink -= Time.fixedDeltaTime;
            if(timeTillNextBlink<=0)
            {
                timeTillNextBlink = blinkPeriod;
                foreach(GameObject b in buttons)
                {
                    setButtonState(b,Random.Range(0,1)==1);
                }
            }
        }
	}

    public void playDialog(string name)
    {
        if(playingSound)
        {
            currentSound.stop(FMOD.Studio.STOP_MODE.IMMEDIATE); 
        }
        currentSound=FMOD_StudioSystem.instance.GetEvent("event:/Dialog/IA/"+name);
        currentSound.start();
        /*currentSound.setCallback(delegate(FMOD.Studio.EVENT_CALLBACK_TYPE type,
                                                       System.IntPtr eventInstance,
                                                       System.IntPtr parameters)
        {
            if(type==FMOD.Studio.EVENT_CALLBACK_TYPE.STOPPED)
                playingSound = false;
            return FMOD.RESULT.OK;
        });*/
        playingSound = true;
    }

   

    void OnValidate()
    {
        updateButtonsArray();
    }

    void updateButtonsArray()
    {
        Transform carBody=GameObject.Find("Car").transform.Find("Body");
        int nbChildren=carBody.childCount;
        buttons = new List<GameObject>();
        for(int i=0;i<nbChildren;i++)
        {
            GameObject g = carBody.GetChild(i).gameObject;
            if (g.name.Contains("button") || g.name.Contains("Button")) buttons.Add(g);
        }
    }

    void setButtonState(GameObject button, bool state)
    {
        button.GetComponent<MeshRenderer>().material
           .SetColor("_Color", state ? new Color(1, 1, 0, 1) : new Color(1, 1, 1, 1));
    }
}

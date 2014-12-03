using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AI : MonoBehaviour
{
    public float blinkPeriod=0.02f;
    
    private FMOD.Studio.EventInstance currentSound=null;
    private bool playingSound = false;
    private List<GameObject> buttons;
    private float timeTillNextBlink = 0;
    private List<int> history;
    private const int HISTORY_NB_TRIES=6;

	void Start ()
    {
	
	}
	
	void FixedUpdate ()
    {
        if (currentSound == null) return;
        FMOD.Studio.PLAYBACK_STATE state;
        currentSound.getPlaybackState(out state);
        if (state == FMOD.Studio.PLAYBACK_STATE.STOPPED)
        {
            timeTillNextBlink = blinkPeriod;
            foreach (GameObject b in buttons)
            {
                setButtonState(b, false);
            }
        }
        else
        {
            timeTillNextBlink -= Time.fixedDeltaTime;
            if (timeTillNextBlink <= 0)
            {
                timeTillNextBlink = blinkPeriod;
                int btnIndex = Random.Range(0, buttons.Count);
                int tries = HISTORY_NB_TRIES - 1;
                while(tries>0 && history.Contains(btnIndex))
                {
                    btnIndex = Random.Range(0, buttons.Count);
                    tries--;
                }
                history.RemoveAt(0);
                history.Add(btnIndex);
                setButtonState(buttons[btnIndex], Random.Range(0, 2) == 1);
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
        playingSound = true;
    }

   

    void OnValidate()
    {
        updateButtonsArray();
        updateHistory();
    }

    void updateButtonsArray()
    {
        Transform carBody=GameObject.Find("Car").transform.Find("Body/CarModel");
        int nbChildren=carBody.childCount;
        buttons = new List<GameObject>();
        for(int i=0;i<nbChildren;i++)
        {
            GameObject g = carBody.GetChild(i).gameObject;
            if (g.name.Contains("button") || g.name.Contains("Button")) buttons.Add(g);
        }
    }

    void updateHistory()
    {
        history = new List<int>();
        for (int i = 0; i < buttons.Count / 2; i++) history.Add(i);
    }

    void setButtonState(GameObject button, bool state)
    {
        // Placeholder effect
        button.transform.localScale = state ? new Vector3(150, 150, 150) : new Vector3(100, 100, 100);
        //button.GetComponent<MeshRenderer>().material
        //   .SetColor("_Color", state ? new Color(1, 1, 0, 1) : new Color(1, 1, 1, 1));
    }
}

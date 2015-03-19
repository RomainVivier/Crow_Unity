using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DialogsManager : MonoBehaviour
{

    #region attributes
    const int NB_SEGMENTS=6;
    
    [System.Serializable]
    public class DialogInfos
    {
        public string[] sounds;
        public enum EventType { GADGET_USE, OBSTACLE_DESTRUCTION, DESTRUCTION_WITH_GADGET, CAR_DAMAGE, CAR_HP, TRIGGER, OBJECT_PROXIMITY };
        public EventType eventType;
        public string eventStringParam;
        public float eventNumberParam;
        public int cooldown;
        public bool forceFirstCooldown;
        public float probability;
        public float preOffset;
        public float postOffset;
        public enum PlayMode {READ_LINEAR_LOOP,READ_LINEAR_ONCE,SHUFFLE_ONCE,SHUFFLE_LOOP,RADNOM_PLAY_ONCE};
        public PlayMode playMode;
    }

    public static DialogsManager _instance;
    public DialogInfos[] _dialogInfos;
    
    private struct InternalDialogInfos
    {
        public int currentCooldown;
        public int[] playList;
        public int pos;
    }

    private InternalDialogInfos[] m_dialogInfos;
    private int m_nbDialogs;
    private FMOD.Studio.EventInstance m_currentEvent;
    private Timer m_timer;
    private float m_afterTimer;
    private Transform m_carTransform=null;
    private FMOD.DSP m_dsp;
    private MeshRenderer[] m_segmentRenderers;
    #endregion

    #region mono
    void Start ()
    {
        m_nbDialogs = _dialogInfos.Length;

        // Init internal dialog infos
        m_dialogInfos = new InternalDialogInfos[m_nbDialogs];
        for(int i=0;i<m_nbDialogs;i++)
        {
            m_dialogInfos[i].currentCooldown = _dialogInfos[i].forceFirstCooldown ? _dialogInfos[i].cooldown : 0;
            if (isRandomPlayMode(_dialogInfos[i].playMode)) shufflePlayList(i);
            m_dialogInfos[i].pos = 0;
        }
        
        // Get segment renderers
        m_segmentRenderers=new MeshRenderer[6];
        for(int i=0;i<NB_SEGMENTS;i++) m_segmentRenderers[i]=GameObject.Find ("Screen_0"+i).GetComponent<MeshRenderer>();
       
        // Init other things
        m_currentEvent = null;
        m_timer = null;
        _instance = this;
	}
	
	void Update ()
    {
    	float volume=0;
    	
    	// Start event if pre-offset finished
        if(m_timer!=null && m_timer.IsElapsedOnce)
        {
            if (m_currentEvent != null) startEvent();
            else m_timer = null;
        }
        if(m_currentEvent!=null)
        {	
			// Delete event and start post-offset if it's finished
            FMOD.Studio.PLAYBACK_STATE state;
            m_currentEvent.getPlaybackState(out state);
            if (state == FMOD.Studio.PLAYBACK_STATE.STOPPED)
           	{
           		m_currentEvent.release();
           		m_currentEvent=null;
            	if (m_afterTimer != 0) m_timer = null;
            	else m_timer.Reset(m_afterTimer);
            }
            else if(m_timer!=null)
            {
				volume=Mathf.PerlinNoise(Time.timeSinceLevelLoad*4,Time.timeSinceLevelLoad*2);
				// Update bow tie
				/*if(m_dsp==null)
				{
					FMOD.ChannelGroup cg=null;				
					FMOD.Studio.System sys=FMOD_StudioSystem.instance.System;
					FMOD.Studio.Bus bus;
					
					sys.getBus("bus:/PreMaster/Voice",out bus);
					if(bus!=null) bus.getChannelGroup(out cg);
					if(cg!=null)
					{
						System.Text.StringBuilder name=new System.Text.StringBuilder();
						cg.getName(name,100);
						Debug.Log (name);
						cg.getDSP(0,out m_dsp);
						m_dsp.setMeteringEnabled(false,true);
					}
				}
				
				if(m_dsp!=null)
				{
					FMOD.DSP_METERING_INFO infos;
					m_dsp.getMeteringInfo(out infos);
            		Debug.Log (infos.numchannels);
            	}*/
            	
            }
            
            // Update bow-tie
            for(int i=0;i<NB_SEGMENTS;i++)
            {
            	m_segmentRenderers[i].material.color= volume > i/((float) NB_SEGMENTS) ? new Color(255,0,0) : new Color(0,0,0);
            }
        }
       
		// on veut get le groupe VX (pas le master)
		
		//		// Get the master channel group
		//		FMOD::ChannelGroup* master;
		//		system->getMasterChannelGroup(&master);
		//		
		//		// Get the DSP unit that is at the head of the DSP chain of the master channel group
		//		FMOD::DSP* masterHead;
		//		master->getDSP(FMOD_CHANNELCONTROL_DSP_HEAD, &masterHead);
		//		// enable output metering
		//		masterHead->setMeteringEnabled(false, true);
		//		
		//		// Call this at regular intervals to fetch the output meter
		//		FMOD_DSP_METERING_INFO outputmeter = {};
		//		masterHead->getMeteringInfo(0, &outputmeter);
		//		
		//		// stereo on iOS
		//		assert(outputmeter.numchannels == 2);
		//		printf("Power over the last %d samples: left = %f, right = %f \n", outputmeter.numsamples, outputmeter.rmslevel[0], outputmeter.rmslevel[1]);
		
    }
    #endregion

    #region public methods
    public void triggerEvent(DialogInfos.EventType type, float numberParam=0)
    {
        triggerEvent(type, "", numberParam);
    }
    
    public void triggerEvent(DialogInfos.EventType type, string stringParam, float numberParam=0)
    {
        for(int i=0;i<m_nbDialogs;i++)
        {
            if (_dialogInfos[i].eventType == type
                && (!usesStringParam(type) || _dialogInfos[i].eventStringParam == stringParam)
                && (!usesNumberParam(type) || _dialogInfos[i].eventNumberParam == numberParam))
                triggerEvent(i);

        }
    }

    public void triggerProximityEvent(string name, Vector3 pos)
    {
        if(m_carTransform==null) m_carTransform = GameObject.FindObjectOfType<Car>().transform.Find("Body");
        for(int i=0;i<m_nbDialogs;i++)
        {
            if (_dialogInfos[i].eventType == DialogInfos.EventType.OBJECT_PROXIMITY
                && _dialogInfos[i].eventStringParam == name)
            {
                float dist = (m_carTransform.position - pos).magnitude;
                if (dist <= _dialogInfos[i].eventNumberParam) triggerEvent(i);
            }
        }
    }
    #endregion

    #region private methods
    private bool isRandomPlayMode(DialogInfos.PlayMode playMode)
    {
        return playMode == DialogInfos.PlayMode.RADNOM_PLAY_ONCE
                || playMode == DialogInfos.PlayMode.SHUFFLE_LOOP
                || playMode == DialogInfos.PlayMode.SHUFFLE_ONCE;
    }

    private bool usesStringParam(DialogInfos.EventType type)
    {
        return type == DialogInfos.EventType.GADGET_USE
            || type == DialogInfos.EventType.OBSTACLE_DESTRUCTION
            || type == DialogInfos.EventType.DESTRUCTION_WITH_GADGET
            || type == DialogInfos.EventType.CAR_DAMAGE
            || type == DialogInfos.EventType.TRIGGER
            || type == DialogInfos.EventType.OBJECT_PROXIMITY;
    }

    private bool usesNumberParam(DialogInfos.EventType type)
    {
        return type == DialogInfos.EventType.CAR_HP
            || type == DialogInfos.EventType.OBJECT_PROXIMITY;
    }

    private bool isOnce(DialogInfos.PlayMode mode)
    {
        return mode == DialogInfos.PlayMode.READ_LINEAR_ONCE
            || mode == DialogInfos.PlayMode.RADNOM_PLAY_ONCE;
    }

    private void shufflePlayList(int dialog)
    {
        int nbSounds = _dialogInfos[dialog].sounds.Length;
        List<int> unsortedList=new List<int>();
        for (int i = 0; i < nbSounds; i++) unsortedList.Add(i);
            m_dialogInfos[dialog].playList = new int[nbSounds];
        int pos = 0;
        while (unsortedList.Count>0)
        {
            int take = Random.Range(0, unsortedList.Count);
            m_dialogInfos[dialog].playList[pos++] = unsortedList[take];
            unsortedList.RemoveAt(take);
        }
    }

    private void triggerEvent(int dialog)
    {
        if(m_timer==null && m_currentEvent==null)
        {
            if(m_dialogInfos[dialog].currentCooldown>0) m_dialogInfos[dialog].currentCooldown--;
            else if(Random.Range(0f,1f)<=_dialogInfos[dialog].probability)
            {
                if(!isOnce(_dialogInfos[dialog].playMode) || m_dialogInfos[dialog].pos<_dialogInfos[dialog].sounds.Length)
                {
                    if(m_dialogInfos[dialog].pos>=_dialogInfos[dialog].sounds.Length)
                    {
                        m_dialogInfos[dialog].pos = 0;
                        if (_dialogInfos[dialog].playMode == DialogInfos.PlayMode.SHUFFLE_LOOP) shufflePlayList(dialog);
                    }
                    int playedDialog = m_dialogInfos[dialog].pos;
                    if(isRandomPlayMode(_dialogInfos[dialog].playMode)) playedDialog=m_dialogInfos[dialog].playList[playedDialog];
                    
                    // Get event
                    m_currentEvent=FMOD_StudioSystem.instance.GetEvent("event:/"+_dialogInfos[dialog].sounds[playedDialog]);
					
					// Update playlist pos
                    m_dialogInfos[dialog].pos++;
                    m_dialogInfos[dialog].currentCooldown = _dialogInfos[dialog].cooldown;

					// Start pre-offset timer
					m_afterTimer=_dialogInfos[dialog].postOffset;
					m_timer=new Timer();
					if(_dialogInfos[dialog].preOffset==0) startEvent();
                    else
                    {
                        m_timer.Reset(_dialogInfos[dialog].preOffset);
                    }
                }
            }
        }
    }
    
    private void startEvent()
    {
		m_currentEvent.start();
		m_dsp=null;
	}
	
	#endregion
}	
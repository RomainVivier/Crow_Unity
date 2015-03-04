using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DialogsManager : MonoBehaviour
{

    #region attributes
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
    
    private class InternalDialogInfos
    {
        public int currentCooldown;
        public int[] playList;
        public int pos = 0;
    }
    private InternalDialogInfos[] m_dialogInfos;
    private int m_nbDialogs;
    private FMOD.Studio.EventInstance m_currentEvent;
    private Timer m_timer;
    private float m_afterTimer;

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
        }

        // Init other things
        m_currentEvent = null;
        m_timer = null;
        _instance = this;
	}
	
	void Update ()
    {
        if(m_timer!=null && m_timer.IsElapsedOnce)
        {
            if (m_currentEvent != null) m_currentEvent.start();
            else m_timer = null;
        }
        if(m_currentEvent!=null)
        {
            FMOD.Studio.PLAYBACK_STATE state;
            m_currentEvent.getPlaybackState(out state);
            if (state == FMOD.Studio.PLAYBACK_STATE.STOPPED) m_currentEvent.release();
            if (m_afterTimer != 0) m_timer = null;
            else m_timer.Reset(m_afterTimer);
        }
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
        if(m_timer!=null && m_currentEvent!=null)
        {
            if(m_dialogInfos[dialog].currentCooldown>0) m_dialogInfos[dialog].currentCooldown--;
            else if(m_currentEvent==null && m_timer==null && Random.Range(0f,1f)<=_dialogInfos[dialog].probability)
            {
                if(isOnce(_dialogInfos[dialog].playMode) || m_dialogInfos[dialog].pos<_dialogInfos[dialog].sounds.Length)
                {
                    if(m_dialogInfos[dialog].pos>=_dialogInfos[dialog].sounds.Length)
                    {
                        m_dialogInfos[dialog].pos = 0;
                        if (_dialogInfos[dialog].playMode == DialogInfos.PlayMode.SHUFFLE_LOOP) shufflePlayList(dialog);
                    }
                    m_currentEvent=FMOD_StudioSystem.instance.GetEvent("event:/"+_dialogInfos[dialog].sounds[m_dialogInfos[dialog].pos]);
                    m_afterTimer=_dialogInfos[dialog].postOffset;
                    if(_dialogInfos[dialog].preOffset==0) m_currentEvent.start();
                    else
                    {
                        m_timer=new Timer();
                        m_timer.Reset(_dialogInfos[dialog].preOffset);
                    }
                }
            }
        }
    }
    #endregion
}

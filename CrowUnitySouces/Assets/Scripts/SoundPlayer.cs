﻿using UnityEngine;
using System.Collections;
using FMOD.Studio;

public class SoundPlayer : MonoBehaviour
{

    #region Members

    public string _soundName;
    public string _soundNameRight;
    public string _soundNameExit;
    public string _soundNameExitRight;
    public bool _is3D=false;
    public bool _onlyOnce = true;
    public float proba = 1;
    public float minSpeedKmh = 80;
    public int forcePan = 0;
    public bool useTrigger = true;

    private FMOD.Studio.EventInstance m_fmodEvent;
    private FMOD.Studio.EventInstance m_fmodEventRight=null;
    private FMOD.Studio.EventInstance m_fmodEventExit = null;
    private FMOD.Studio.EventInstance m_fmodEventExitRight = null;

    private bool m_alreadyPlayed;
    #endregion

    #region MonoBehaviour

    void Start()
    {
        // Init fmod events
        m_fmodEvent=FMOD_StudioSystem.instance.GetEvent("event:/"+_soundName);
        if(_soundNameRight!="") m_fmodEventRight=FMOD_StudioSystem.instance.GetEvent("event:/"+_soundNameRight);
        if(_soundNameExit!="") m_fmodEventRight=FMOD_StudioSystem.instance.GetEvent("event:/"+_soundNameExit);
        if(_soundNameExitRight!="") m_fmodEventRight=FMOD_StudioSystem.instance.GetEvent("event:/"+_soundNameExitRight);
        
        m_alreadyPlayed = false;
    }

    void Update()
    {
        // Update only if it don't use trigger
        if (useTrigger) return;

        // Restart event if stopped
        FMOD.Studio.PLAYBACK_STATE state;
        m_fmodEvent.getPlaybackState(out state);
        if (state == FMOD.Studio.PLAYBACK_STATE.STOPPED) m_fmodEvent.start();

        // Set 3D attributes
        _3D_ATTRIBUTES threeDeeAttr = new _3D_ATTRIBUTES();
        threeDeeAttr.position = UnityUtil.toFMODVector(transform.position);
        threeDeeAttr.up = UnityUtil.toFMODVector(transform.up);
        threeDeeAttr.forward = UnityUtil.toFMODVector(transform.forward);
        threeDeeAttr.velocity = UnityUtil.toFMODVector(-GameObject.FindObjectOfType<Car>().gameObject.transform.Find("Body").rigidbody.velocity);
        m_fmodEvent.set3DAttributes(threeDeeAttr);
    }
    #endregion

    #region Collider
    void OnTriggerEnter(Collider other)
    {
        handleTrigger(other, m_fmodEvent, m_fmodEventRight);
    }

    void OnTriggerExit(Collider other)
    {
        if (m_fmodEventExit != null) handleTrigger(other, m_fmodEventExit, m_fmodEventExitRight);
    }

    private void handleTrigger(Collider other, FMOD.Studio.EventInstance soundLeft, FMOD.Studio.EventInstance soundRight)
    {
        if (!useTrigger) return;
        if(other.gameObject.name=="Body" && other.gameObject.transform.parent.GetComponent<Car>()!=null
            && (!_onlyOnce || !m_alreadyPlayed))
        {
            m_alreadyPlayed=true;
            if(Random.Range(0f,1f)<proba)
            {
                float speedKmh = other.gameObject.transform.parent.GetComponent<Car>().getForwardVelocity() * 3.6f;
                if (speedKmh >= minSpeedKmh)
                {
                    if (!_is3D)
                    {
                        Vector3 dpos = transform.position - other.transform.position;
                        dpos.Normalize();
                        Vector3 right = other.transform.right;
                        float diff = Vector3.Dot(dpos, right);
                        // Use left or right event if the right event is set
                        if(soundRight!=null)
                        {
                            if (diff > 0) soundRight.start();
                            else soundLeft.start();
                        }
                        else // Pan if only one event is set
                        {
                            soundLeft.start();
                            FMOD.Studio.ParameterInstance param;
                            m_fmodEvent.getParameter("Pan", out param);
                            if (forcePan == 0) param.setValue(diff > 0 ? 1 : 0);
                            else param.setValue(forcePan > 0 ? 1 : 0);
                        }
                        
                    }
                    else
                    {
                        soundLeft.start();

                        // Set 3D attributes
                        _3D_ATTRIBUTES threeDeeAttr = new _3D_ATTRIBUTES();
                        threeDeeAttr.position = UnityUtil.toFMODVector(transform.position);
                        threeDeeAttr.up = UnityUtil.toFMODVector(transform.up);
                        threeDeeAttr.forward = UnityUtil.toFMODVector(transform.forward);
                        threeDeeAttr.velocity = UnityUtil.toFMODVector(-other.gameObject.rigidbody.velocity);
                        m_fmodEvent.set3DAttributes(threeDeeAttr);
                    }
                }
            }
        }
    }
    #endregion
}


///Le cube lilian le cube !!!
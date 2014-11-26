using UnityEngine;
using System.Collections;

public class Radio : MonoBehaviour {


    public float _switchDuration;

    private int m_radioState = 0;
    private float m_currentFrequency;
    private int m_startFrequency;
    private int m_targetFrequency;
    private Timer m_timer;

    public int RadioState
    {
        get { return m_radioState; }
        set 
        { 
            m_radioState = value;
            //FMOD_StudioSystem.instance.PlayOneShot("event");
        }
    }

	void Start()
    {
        m_timer = new Timer();
        m_startFrequency = 6;
        m_targetFrequency = 6;
        m_currentFrequency = 6f;
        FMOD_StudioSystem.instance.PlayOneShot("event:/Music/Radio/RadioStream", transform.position);
	}
	
	void Update()
    {
        if(Input.GetKeyDown(KeyCode.A))
        {
            SwitchFrequencyDown();
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            SwitchFrequencyUp();
        }

        if(m_timer.IsElapsedOnce)
        {
            if(m_targetFrequency == 1)
            {
                m_currentFrequency = 7f;
                m_startFrequency = 7;
                m_targetFrequency = 6;
                m_timer.Reset(_switchDuration / 2);
            }
            else if(m_targetFrequency == 7)
            {
                m_currentFrequency = 1f;
                m_startFrequency = 1;
                m_targetFrequency = 2;
                m_timer.Reset(_switchDuration / 2);
            }
            else
            {
                m_currentFrequency = m_targetFrequency;
            }

        }

	    if(m_timer.IsElapsedLoop)
        {
            return;
        }

        m_currentFrequency = Mathf.Lerp((float)m_startFrequency, (float)m_targetFrequency, 1 - m_timer.CurrentNormalized);

        Debug.Log("start = " + m_startFrequency + " :: target = " + m_targetFrequency + " :: current value = " + m_currentFrequency);
        //set the value here
	}

    void SwitchFrequencyUp()
    {
        if(!m_timer.IsElapsedLoop)
        {
            return;
        }

        m_startFrequency = (int)m_currentFrequency;

        if(m_currentFrequency == 6 )
        {
            m_targetFrequency = 7;
            m_timer.Reset(_switchDuration / 2);
        }
        else
        {
            m_targetFrequency = (int)m_currentFrequency + 2;
            m_timer.Reset(_switchDuration);
        }
    }

    void SwitchFrequencyDown()
    {
        if (!m_timer.IsElapsedLoop)
        {
            return;
        }

        m_startFrequency = (int)m_currentFrequency;

        if (m_currentFrequency == 2)
        {
            m_targetFrequency = 1;
            m_timer.Reset(_switchDuration / 2);
        }
        else
        {
            m_targetFrequency = (int)m_currentFrequency - 2;
            m_timer.Reset(_switchDuration);
        }
    }
}

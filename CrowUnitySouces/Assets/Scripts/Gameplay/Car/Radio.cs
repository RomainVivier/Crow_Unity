using UnityEngine;
using System.Collections;

public class Radio : MonoBehaviour {


    public float _switchDuration;
	public float _pickupSpeed;
	
    private int m_radioState = 1;
    private float m_currentFrequency;
    private int m_startFrequency;
    private int m_targetFrequency;
	private float m_currentBoost;
    private Timer m_timer;
	private float m_radioPickup=0;
	
    private FMOD.Studio.EventInstance m_radio;
    private FMOD.Studio.ParameterInstance m_fmodRadioFreq;
    private FMOD.Studio.ParameterInstance m_fmodRadioState;
    //private FMOD.Studio.ParameterInstance m_fmodRadioBoost;
    private FMOD.Studio.ParameterInstance m_fmodRadioPickup;

    public FMOD_StudioEventEmitter _emitter;

    public int RadioState
    {
        get { return m_radioState; }
        set 
        { 
            m_radioState = value;
            m_fmodRadioState.setValue(m_radioState);
        }
    }

	public int Frequency
	{
		get { return m_targetFrequency;}
	}
	
	void Start()
    {
        m_timer = new Timer();
        m_startFrequency = 2;
        m_targetFrequency = 2;
        m_currentFrequency = 2f;
		m_radioPickup=0;
		
        m_radio = FMOD_StudioSystem.instance.GetEvent("event:/Music/Radio/radioStream");
        m_radio.start();

        m_radio.getParameter("radioFrequency", out m_fmodRadioFreq);
        m_radio.getParameter("radioPickup", out m_fmodRadioPickup);
        m_radio.getParameter("radioState", out m_fmodRadioState);
        //m_radio.getParameter("Boost", out m_fmodRadioBoost);

        RadioState = 0;
	}
	
	void Update()
    {

        /*if (Input.GetKeyDown(KeyCode.Return))
        {
            if (RadioState == 1)
                RadioState = 0;
            else
                RadioState = 1;
        }*/

		m_radioPickup-=_pickupSpeed*Time.deltaTime;
		if(m_radioPickup<0) m_radioPickup=0;
		
        if(m_timer.IsElapsedOnce)
        {
            if(m_targetFrequency == 1)
            {
                m_currentFrequency = 9f;
                m_startFrequency = 9;
                m_targetFrequency = 8;
                m_timer.Reset(_switchDuration / 2);
            }
            else if(m_targetFrequency == 9)
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

        //Debug.Log("start = " + m_startFrequency + " :: target = " + m_targetFrequency + " :: current value = " + m_currentFrequency);
        m_fmodRadioFreq.setValue(m_currentFrequency);
        //m_fmodRadioBoost.setValue(m_currentBoost);
        m_fmodRadioPickup.setValue(m_radioPickup);
	}

    public void SwitchFrequencyUp()
    {
        if(!m_timer.IsElapsedLoop)
        {
            return;
        }

        m_startFrequency = (int)m_currentFrequency;

        if(m_currentFrequency == 8 )
        {
            m_targetFrequency = 9;
            m_timer.Reset(_switchDuration / 2);
        }
        else
        {
            m_targetFrequency = (int)m_currentFrequency + 2;
            m_timer.Reset(_switchDuration);
        }
    }

    public void SwitchFrequencyDown()
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

	public void ResetPickup()
	{
		m_radioPickup=1;
	}
	
    //public void SwitchPitchUp()
    //{
    //    m_currentBoost = 1;
    //}

    //public void SwitchPitchDown()
    //{
    //    m_currentBoost = 0;
    //}
}

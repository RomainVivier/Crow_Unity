using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GadgetManager : MonoBehaviour {

	#region Members

	/// <summary>
	/// key = event name
	/// value = gadget
	/// </summary>
	private  Dictionary<string, Gadget> m_gadgets;
	private static GadgetManager m_instance;
    private string m_lastGadget;
    private bool m_hasOneGadgetPlaying;
    private Timer m_timer;

	#endregion

    #region Properties

    public bool HasOneGadgetPlaying
    {
        get { return m_hasOneGadgetPlaying; }
        set { m_hasOneGadgetPlaying = value; }
    }

    #endregion 

    #region Singleton

    public static GadgetManager Instance
	{
		get
		{
			if (m_instance == null)
			{
				m_instance = GameObject.FindObjectOfType<GadgetManager>();
			}
			
			return m_instance;
		}
	}
	
	void Awake()
	{
		if (m_instance == null)
		{
			m_instance = this;
			m_instance.Init();
		}
		else
		{
			if (this != m_instance)
				Destroy(this.gameObject);
		}
	}

	private void Init()
	{
		m_gadgets = new Dictionary<string, Gadget>();
        m_hasOneGadgetPlaying = false;
        m_timer = new Timer();
	}

	#endregion

    #region Functions

    public void Register(string name, Gadget gadget)
	{
		if(gadget == null || name == null || name == "" )
		{
			Debug.LogError("can't register null gadget or name can't be null or empty");
			return;
		}

		m_gadgets.Add(name, gadget);
	}

	public void PlayGadget(string name)
	{
        if(m_timer.IsElapsedLoop && HasOneGadgetPlaying && m_lastGadget != name)
        {
            Debug.Log("Can't play button, timer elapsed = " + m_timer.IsElapsedLoop + ", gadget playing = " + HasOneGadgetPlaying + ", last gagdet : " + m_lastGadget);
            return;
        }

		if(m_gadgets.ContainsKey(name) && m_gadgets[name].IsReady)
		{
			m_gadgets[name].Play();
            m_lastGadget = name;
            m_timer.Reset(0.5f);
            if(m_gadgets[name].PlaySound!="") FMOD_StudioSystem.instance.PlayOneShot(m_gadgets[name].PlaySound,transform.position);
		}else{
            if(m_gadgets[name].CantPlaySound!="") FMOD_StudioSystem.instance.PlayOneShot(m_gadgets[name].CantPlaySound,transform.position);
			Debug.Log("no gadget has been registered to this name or gadget is not ready !");
		}
	}

	#endregion

}

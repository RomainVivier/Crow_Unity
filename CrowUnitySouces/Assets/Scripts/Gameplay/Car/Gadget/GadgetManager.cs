using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class GadgetManager : MonoBehaviour {

	#region Members

	/// <summary>
	/// key = event name
	/// value = gadget
	/// </summary>
	private  Dictionary<string, Gadget> m_gadgets;
    private List<string> m_assignGadgets;
	private static GadgetManager m_instance;
    private string m_lastGadget;
    private bool m_hasOneGadgetPlaying;
    private Timer m_timer;

    private CardPopup m_cardPopup;
	#endregion

    #region Properties

    public bool HasOneGadgetPlaying
    {
        get { return m_hasOneGadgetPlaying; }
        set { m_hasOneGadgetPlaying = value; }
    }

    public CardPopup CardPopup
    {
        set { m_cardPopup = value; }
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
        m_cardPopup = GameObject.FindObjectOfType<CardPopup>();
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

	public bool PlayGadget(string name)
	{
        if(!m_timer.IsElapsedLoop/* && HasOneGadgetPlaying && m_lastGadget != name*/)
        {
            Debug.Log("Can't play button, timer elapsed = " + m_timer.IsElapsedLoop + ", gadget playing = " + HasOneGadgetPlaying + ", last gagdet : " + m_lastGadget);
            return false;
        }

		if(m_gadgets.ContainsKey(name) && m_gadgets[name].IsReady)
		{
            if(m_gadgets[name]._cardMaterial!=null && !m_gadgets[name]._invertGesture) m_cardPopup.popup(m_gadgets[name]._cardMaterial);
			m_gadgets[name].Play();
            m_lastGadget = name;
            m_timer.Reset(0.5f);
            if(m_gadgets[name].PlaySound!="") FMOD_StudioSystem.instance.PlayOneShot(m_gadgets[name].PlaySound,transform.position);
            DialogsManager._instance.triggerEvent(DialogsManager.DialogInfos.EventType.GADGET_USE, name);
			return true;
        }else{
            if(m_gadgets[name].CantPlaySound!="") FMOD_StudioSystem.instance.PlayOneShot(m_gadgets[name].CantPlaySound,transform.position);
			Debug.Log("no gadget has been registered to this name or gadget is not ready !");
			return false;
		}
	}
    
    public Gadget GetGadget(string id)
    {
        if(!m_gadgets.ContainsKey(id))
        {
            return null;
        }
        else return m_gadgets[id];
    }

    public string RandomUnassignGadget()
    {
        string gadgetID;

        var gadgets = m_gadgets.Where(g => g.Value._isAssign == false).Select(g => g.Key).ToList();

        if (gadgets.Count == 0)
        {
            return null;
        }

        gadgetID = gadgets[Random.Range(0, gadgets.Count)];

        if (m_gadgets.ContainsKey(gadgetID))
        {
            m_gadgets[gadgetID]._isAssign = true;
        }

        return gadgetID;
    }

    public Gadget getGadgetById(string id)
    {
        return m_gadgets[id];
    }

    public GadgetAbility[] GadgetAbilities(string id)
    {
        if(!m_gadgets.ContainsKey(id))
        {
            return null;
        }

        return m_gadgets[id]._abilities;
    }

	#endregion



}

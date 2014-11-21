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
		if(m_gadgets.ContainsKey(name))
		{
			m_gadgets[name].Play();
		}else{
			Debug.Log("no gadget has been registered to this name");
		}
	}

	#endregion

}

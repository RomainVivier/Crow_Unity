using UnityEngine;
using System.Collections;

public class Gadget : MonoBehaviour {

	private bool m_isReady = true;

	public bool IsReady
	{
		get{ return m_isReady; }
		set{ m_isReady = value; }
	}

	public virtual void Play()
	{
		IsReady = false;
	}

	public virtual void Stop()
	{
		IsReady = true;
	}
}

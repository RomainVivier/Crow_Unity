using UnityEngine;
using System.Collections;

public class Gadget : MonoBehaviour
{

    #region Members

    private bool m_isReady = true;
    protected string m_playSound = "";
    protected string m_cantPlaySound = "";
    #endregion 

    #region Properties

    public bool IsReady
	{
		get{ return m_isReady; }
		set{ m_isReady = value; }
	}

    public string PlaySound
    {
        get { return m_playSound; }
    }
    
    public string CantPlaySound
    {
        get { return m_cantPlaySound; }
    }
    #endregion 

    #region Virtual Functions

    public virtual void Play()
	{
        GadgetManager.Instance.HasOneGadgetPlaying = true;
	}

	public virtual void Stop()
	{
        GadgetManager.Instance.HasOneGadgetPlaying = false;
    }

    #endregion

}

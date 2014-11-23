using UnityEngine;
using System.Collections;

public class Gadget : MonoBehaviour
{

    #region Members

    private bool m_isReady = true;

    #endregion 

    #region Properties

    public bool IsReady
	{
		get{ return m_isReady; }
		set{ m_isReady = value; }
	}

    #endregion 

    #region Virtual Functions

    public virtual void Play()
	{
        IsReady = false;
        GadgetManager.Instance.HasOneGadgetPlaying = true;
	}

	public virtual void Stop()
	{
		IsReady = true;
        GadgetManager.Instance.HasOneGadgetPlaying = false;
    }

    #endregion

}

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
        GadgetManager.Instance.HasOneGadgetPlaying = true;
	}

	public virtual void Stop()
	{
        GadgetManager.Instance.HasOneGadgetPlaying = false;
    }

    #endregion

}

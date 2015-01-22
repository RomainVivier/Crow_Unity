using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Gadget : MonoBehaviour
{

    #region Members

    private bool m_isReady = true;
    protected GadgetFamily m_gadgetFamily;
    protected List<GadgetAbilitie> m_abilities;

    #endregion 

    #region Properties

    public virtual void Start()
    {
        m_abilities = new List<GadgetAbilitie>();
    }

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

public enum GadgetFamily
{
    Light,
    Contact,
    Distance
}

public enum GadgetAbilitie
{
    Light,
    Noise,
    Slice,
    Destruction,
    HeavyProjection,
    LightProjection,
    Weather,
    SpeedBoost,
    Jump,
    Hover,
    Slow,
    Useless,
    Combo
}
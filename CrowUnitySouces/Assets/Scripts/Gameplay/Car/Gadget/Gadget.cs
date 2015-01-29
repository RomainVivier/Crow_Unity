using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Gadget : MonoBehaviour
{

    #region Members

    public Animator _buttonAnim;

    private bool m_isReady = true;
    protected GadgetFamily m_gadgetFamily;
    protected List<GadgetAbility> m_abilities;

    #endregion 

    #region Properties

    public bool IsReady
	{
		get{ return m_isReady; }
		set{ m_isReady = value; }
	}

    #endregion 

    #region Mono Function

    public virtual void Start()
    {
        m_abilities = new List<GadgetAbility>();
    }

    public virtual void Update()
    {
    }

    #endregion

    #region Virtual Functions

    public virtual void Play()
	{
        GadgetManager.Instance.HasOneGadgetPlaying = true;
        if (_buttonAnim != null)
        {
            _buttonAnim.SetTrigger("Engage");
        }
	}

	public virtual void Stop()
	{
        GadgetManager.Instance.HasOneGadgetPlaying = false;
        if (_buttonAnim != null)
        {
            _buttonAnim.SetTrigger("Engage");
        }
    }

    #endregion

}

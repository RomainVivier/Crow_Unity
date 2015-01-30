using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Gadget : MonoBehaviour
{

    #region Members

    public Animator _buttonAnim;
    public GadgetAbility[] _abilities;
    public bool _isAssign = false;

    private bool m_isReady = true;
    protected string m_playSound = "";
    protected string m_cantPlaySound = "";
    protected GadgetFamily m_gadgetFamily;

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

    #region Mono Function

    public virtual void Start()
    {
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
            _buttonAnim.speed = 10;
            _buttonAnim.SetBool("Engage", true);
            //_buttonAnim.SetTrigger("Engage");
        }
	}

	public virtual void Stop()
	{
        GadgetManager.Instance.HasOneGadgetPlaying = false;
        if (_buttonAnim != null)
        {
            _buttonAnim.speed = 10;
            _buttonAnim.SetBool("Engage", false);
            //_buttonAnim.SetTrigger("Engage");
        }
    }

    #endregion

}

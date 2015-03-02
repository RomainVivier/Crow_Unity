using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Gadget : MonoBehaviour
{

    #region Members

    public Animator _buttonAnim;
    public GadgetAbility[] _abilities;
    public bool _isAssign = false;
    public bool _invertGesture = false;
    public Material _cardMaterial;
    public int _score=500;
    public int _combo=1;
	//public int _heatValue = 1;
    public float _cooldown;

    private bool m_isReady = true;
    protected string m_playSound = "event:/SFX/Buttons/ButtonSmall/buttonPushSmallValidated";
    protected string m_cantPlaySound = "event:/SFX/Buttons/ButtonSmall/buttonPushSmallDenied";
    protected GadgetFamily m_gadgetFamily;
    protected Timer m_gadgetCooldownTimer;
    #endregion 

    #region Properties

    public bool IsReady
	{
		get{ return m_isReady; }
		set
        {
            m_isReady = value;
            if (_buttonAnim != null)
            {
                _buttonAnim.speed = 10;
                _buttonAnim.SetBool("Engage", !value);
                //_buttonAnim.SetTrigger("Engage");
            }
        }
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

    public virtual void Awake()
    {
        m_gadgetCooldownTimer = new Timer();
    }

    public virtual void Update()
    {
        if (m_gadgetCooldownTimer.IsElapsedOnce) IsReady = true;

    }

    #endregion

    #region Virtual Functions

    public virtual void Play()
	{
		//HeatBar.Instance.AddHeatUnits (_heatValue);
        if (_buttonAnim != null)
        {
            _buttonAnim.speed = 10;
            _buttonAnim.SetBool("Engage", true);
			//Score.Instance._gadgetsUsed++;
            //_buttonAnim.SetTrigger("Engage");
        }
	}

	public virtual void Stop()
	{
        GadgetManager.Instance.HasOneGadgetPlaying = false;

        if(_cooldown>=0) m_gadgetCooldownTimer.Reset(_cooldown);
    }

    #endregion

    #region Functions
    protected void addScore()
    {
        Score.Instance.AddScore(_score,_combo);
    }
    #endregion

}

using UnityEngine;
using System.Collections;

public class RadioButton : Gadget {

	public Radio _radio;
    public bool _switchUp;

    public override void Awake()
    {
        if(_switchUp)
        {
            GadgetManager.Instance.Register("RadioUp", this);
        }
        else
        {
            GadgetManager.Instance.Register("RadioDown", this);
        }
        base.Awake();
    }

    public override void Play()
    {
        if(_radio == null)
        {
            return;
        }

        if (_switchUp)
        {
            _radio.SwitchFrequencyUp();
        }
        else
        {
            _radio.SwitchFrequencyDown();
        }
    }
}

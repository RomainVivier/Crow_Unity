using UnityEngine;
using System.Collections;

public class RadioButton : Gadget {

	const int NB_RADIOS=4;
	
	public Radio _radio;
    public bool _switchUp;
    public float _scrollSpeed=1;
    public MeshRenderer _radioScreen;
    
	private float pos=0;
	private float tgtPos=0;
	
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
        	if(_radio.RadioState==0) _radio.RadioState=1;
        	else
        	{
        		_radio.SwitchFrequencyUp();
        		if(_radio.Frequency==6) _radio.RadioState=0;
        	}
			tgtPos+=1f/(1f+NB_RADIOS);
		}
        /*else
        {
            _radio.SwitchFrequencyDown();
        }*/
    }
    
    public override void Update()
    {
    	if(pos<tgtPos)
    	{
    		pos+=Time.deltaTime*_scrollSpeed;
    		if(pos>tgtPos) pos=tgtPos;
    	}
    	_radioScreen.material.mainTextureOffset=new Vector2(pos, 0);
    }
}

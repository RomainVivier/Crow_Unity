using UnityEngine;
using System.Collections;

public class DirectionButton : Gadget 
{
    public RailsControl _rc;
    public float _dirValue;

    public override void Play()
    {
        _rc.Steering = _dirValue;

        base.Play();
        Stop();
    }
}

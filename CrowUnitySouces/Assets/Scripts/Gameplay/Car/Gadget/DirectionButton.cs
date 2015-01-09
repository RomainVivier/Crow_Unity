using UnityEngine;
using System.Collections;

public class DirectionButton : ButtonGadget 
{
    public RailsControl _rc;
    public float _dirValue;

    #region MonoBehaviour

    public override void Start()
    {
        GadgetManager.Instance.Register(_dirValue > 0 ? "Right" : "Left", this);
        base.Start();
    }

    #endregion

    public override void Play()
    {
        base.Play();
        _rc.ShiftRail(_dirValue);
        Stop();
    }
}

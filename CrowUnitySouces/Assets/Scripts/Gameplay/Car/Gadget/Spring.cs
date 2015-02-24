using UnityEngine;
using System.Collections;

public class Spring : Gadget {


    private Timer m_timer;
    private Timer m_cooldown;

    public override void Awake()
    {
        m_timer = new Timer();
        m_cooldown = new Timer();
        GadgetManager.Instance.Register("Spring", this);
        base.Awake();
    }

    public override void Update()
    {
        if (m_cooldown.IsElapsedOnce)
        {
            IsReady = true;
        }                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                      
        base.Update();
    }

    public override void Play()
    {
        base.Play();
    }

    public override void Stop()
    {
        base.Stop();
    }
}

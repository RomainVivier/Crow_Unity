using UnityEngine;
using System.Collections;

public class Spring : Gadget {

    //private enum State { ASCENDING, GLIDING, }
    private Timer m_timer;
    private Timer m_cooldown;
    private Vector3 m_basePos;
    private Vector3 m_baseForward;

    private Car m_car;
    private Transform m_carBodyTransform;

    public override void Awake()
    {
        m_timer = new Timer();
        m_cooldown = new Timer();
        GadgetManager.Instance.Register("Spring", this);
        base.Awake();
    }

    void Start()
    {
        m_car = GameObject.FindObjectOfType<Car>();
        m_carBodyTransform = m_car.gameObject.transform.Find("Body");
    }

    public override void Update()
    {                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                    
        base.Update();
    }

    public override void Play()
    {
        base.Play();
        m_basePos = m_carBodyTransform.position;
        m_baseForward = m_carBodyTransform.forward;
        m_baseForward.y = 0;
        m_baseForward.Normalize();
    }

    public override void Stop()
    {
        base.Stop();
    }
}

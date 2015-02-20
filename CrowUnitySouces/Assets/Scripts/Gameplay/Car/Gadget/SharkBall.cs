using UnityEngine;
using System.Collections;

public class SharkBall : Gadget
{
#region Members

    public GameObject _ball;
    public GameObject _shark;

    public float _speed;
    public float _DistanceToRun;
    public RailsControl _railsControl;

    public float _cooldown;
    private Timer m_cooldown;

    enum State
    {
        Inactivated,
        Running,
        Deployed,
        Destroyed
    }

    private State m_state;

    private float m_distanceRun;
    private Rails m_rails;
    private float m_railsIndex;

#endregion

#region MonoBehaviour

    public override void Awake()
    {
        GadgetManager.Instance.Register("SharkBall", this);
        m_cooldown = new Timer();
        gameObject.SetActive(false);
        IsReady = true;
        m_state = State.Inactivated;
    }

    public override void Update()
    {
        //TODO 

        if(m_state != State.Running)
        {
            return;
        }

        /*
         * faire l'avancé en fonction du rails et le changement au besoin
         */
    }

    void OnTriggerEnter(Collider other)
    {
        //TODO

        /*
         * Detecter collision
         * 
         */
    }

#endregion

#region Overrided Functions

    public override void Play()
    {
        base.Play();

        gameObject.SetActive(true);
        m_state = State.Running;
        _ball.SetActive(true);
        _buttonAnim.SetBool("Engage", true);

        if(_railsControl == null)
        {
            Debug.LogError("RailsControl can't be null for this gadget.");
            return;
        }

        m_rails = _railsControl.Rails;
        m_railsIndex = _railsControl.currentRail;

    }

    public override void Stop()
    {
        base.Stop();
        gameObject.SetActive(false);
        _buttonAnim.SetBool("Engage", false);
    }

#endregion
    
}

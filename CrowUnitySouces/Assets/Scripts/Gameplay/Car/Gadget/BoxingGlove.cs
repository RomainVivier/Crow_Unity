using UnityEngine;
using System.Collections;

public class BoxingGlove : Gadget
{
    #region Members

    public Animator _anim;
    public float _punchPower;

    private BoxCollider m_collider;
    private Timer m_timer;
    private Car m_car;
    private GameObject m_childObject;
    #endregion

    #region MonoBehaviour

    public override void Awake()
    {
        GadgetManager.Instance.Register("BoxingGlove", this);
        m_timer = new Timer();
        m_childObject = transform.GetChild(0).gameObject;
        collider.enabled = false;
        m_childObject.SetActive(false);
        //gameObject.SetActive(false);
        m_car = transform.parent.parent.parent.gameObject.GetComponent<Car>();
        base.Awake();
    }

    public override void Update()
    {
        if (m_timer.IsElapsedOnce)
        {
            Stop();
        }
        base.Update();
    }

    #endregion

    public override void Play()
    {
        base.Play();
        collider.enabled = true;
        m_childObject.SetActive(true);
        //gameObject.SetActive(true);
        _anim.SetTrigger("Engage");
        m_timer.Reset(1.1f);
        FMOD_StudioSystem.instance.PlayOneShot("event:/SFX/Gadgets/boxingGlove/gadgetBoxingGloveEngage", transform.position);
        m_car.updateValues();
        IsReady = false;
    }

    public override void Stop()
    {
        base.Stop();
        collider.enabled = false;
        m_childObject.SetActive(false);
        //gameObject.SetActive(false);
        m_car.updateValues();
        //IsReady = true;

    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.collider.tag);
        if (other.collider.CompareTag("Obstacle"))
        {
            Vector3 forceDirection = (m_car.getForwardVector() + Vector3.up).normalized;
            other.rigidbody.AddForce(forceDirection * _punchPower);
            other.gameObject.AddComponent<ObstacleDestroyer>();
            addScore();
            FMOD_StudioSystem.instance.PlayOneShot("event:/SFX/Gadgets/boxingGlove/gadgetBoxingGloveSuccess", transform.position);
            DialogsManager._instance.triggerEvent(DialogsManager.DialogInfos.EventType.OBSTACLE_DESTRUCTION, other.gameObject.name);
            DialogsManager._instance.triggerEvent(DialogsManager.DialogInfos.EventType.DESTRUCTION_WITH_GADGET, "BoxingGlove");
        }
        
    }

    
}

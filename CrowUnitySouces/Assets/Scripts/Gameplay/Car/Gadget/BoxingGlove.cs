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
    #endregion

    #region MonoBehaviour

    public override void Awake()
    {
        GadgetManager.Instance.Register("BoxingGlove", this);
        m_timer = new Timer();
        gameObject.SetActive(false);
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
        gameObject.SetActive(true);
        _anim.SetTrigger("Engage");
        m_timer.Reset(1.1f);
        m_car.updateValues();
        IsReady = false;
    }

    public override void Stop()
    {
        base.Stop();
        gameObject.SetActive(false);
        m_car.updateValues();
        IsReady = true;

    }

    void OnCollisionEnter(Collision collision)
    {
        //Debug.Log("Boxing glove collides "+ collision.collider.name);
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.name);
        if (other.collider.CompareTag("Obstacle"))
        {
            //Debug.Log("punch out baby !");
            other.rigidbody.AddForce(m_car.getForwardVector() * _punchPower, ForceMode.Impulse);
            other.gameObject.AddComponent<ObstacleDestroyer>();
            
            Score.Instance.AddScore(500);
        }
        
    }

    
}

﻿using UnityEngine;
using System.Collections;

public class BoxingGlove : Gadget
{
    #region Members

    public Animator _anim;
    public float _punchPower;

    private BoxCollider m_collider;
    private Timer m_timer;

    #endregion

    #region MonoBehaviour

    public override void Awake()
    {
        GadgetManager.Instance.Register("BoxingGlove", this);
        m_timer = new Timer();
        gameObject.SetActive(false);
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
        IsReady = false;
    }

    public override void Stop()
    {
        base.Stop();
        gameObject.SetActive(false);
        IsReady = true;

    }

    void OnCollisionEnter(Collision collision)
    {
        Debug.Log(collision.collider.name);
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.name);
        if (other.collider.CompareTag("Obstacle"))
        {
            Debug.Log("punch out baby !");
            other.rigidbody.AddForce(Vector3.forward * _punchPower, ForceMode.Impulse);
            
            Score.Instance.AddScore(500);
        }
        
    }

    
}
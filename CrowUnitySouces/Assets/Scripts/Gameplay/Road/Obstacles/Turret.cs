using UnityEngine;
using System.Collections;

public class Turret : Obstacle
{
    public Animator _anim;
    public float _fireRate;
    public Transform _canon;
    public GameObject _projectile;

    private Timer m_timer;

    public override void Start()
    {
        base.Start();
        m_timer = new Timer();
    }

    public override void Update()
    {
        base.Update();

        if(!m_activated)
        {
            return;
        }

        if(m_timer.IsElapsedOnce)
        {
            Fire();
        }
    }

    public override void Activate()
    {
        base.Activate();
        _anim.SetTrigger("Engage");
        m_timer.Reset(_fireRate);
        m_activated = true;
    }

    void Fire()
    {
        GameObject go = GameObject.Instantiate(_projectile, _canon.position, _canon.rotation) as GameObject;
        Destroy(go, 5f);
        m_timer.Reset(_fireRate);
    }
}

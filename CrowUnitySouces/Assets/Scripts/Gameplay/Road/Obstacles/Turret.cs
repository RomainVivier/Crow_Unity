using UnityEngine;
using System.Collections;

public class Turret : Obstacle
{
    public Animator _anim;
    public float _fireRate;
    public Transform _canon;
    public GameObject _projectile;
	public Renderer[] _WarmSignals;
    public Color _WarmColor;
    public Color _shootColor;
    public ParticleSystem _particle;

    private Timer m_timer;
	private int m_numberSignals;
	private bool m_engaged=false;
	private BoxCollider m_collider;
	
    public override void Start()
    {
        base.Start();
        m_timer = new Timer();
        m_collider=transform.parent.collider as BoxCollider;
    }

    public override void Update()
    {
        base.Update();

        if(m_state != State.Activated)
        {
            return;
        }

        if(m_timer.Current < _fireRate/3)
        {
            _particle.Stop();
        }

        if(m_timer.IsElapsedOnce)
        {
            Fire();
        }

		m_numberSignals = (int)((1 - m_timer.CurrentNormalized) / (_fireRate / _WarmSignals.Length));

		for(int i =0; i < _WarmSignals.Length; i++)
		{
			if(m_numberSignals < _WarmSignals.Length - 1)
			{
				_WarmSignals[i].material.color = i <= m_numberSignals ? _shootColor : _WarmColor;
			}
			else
			{
				_WarmSignals[i].material.color = Color.white;
			}
		}

		Vector3 center=m_collider.center;
		center.y=transform.localPosition.y+2.7f;
		m_collider.center=center;
    }

    public override void Activate()
    {
        base.Activate();
        _anim.SetTrigger("Engage");
        m_timer.Reset(_fireRate);
        m_engaged=true;
    }

    public override void Disactivate()
    {
        _anim.SetTrigger("Disengage");
        m_engaged=false;
    }

    void Fire()
    {
    	if(!m_engaged) return;
        GameObject go = GameObject.Instantiate(_projectile, _canon.position, _canon.rotation) as GameObject;
        Destroy(go, 5f);
        m_timer.Reset(_fireRate);
        _particle.Play();
    }
}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class Obstacle : MonoBehaviour
{
    #region members

    public float _maxDistVignette;
    public float _fadeDistVignette;
    public float _minDistVignette;
    public float _activateDistance;
    public float _disactivateDistance;
    public SpriteRenderer _vignette;

    protected Color m_vignetteColor;

    protected List<GadgetAbility> m_weaknesses;
    protected Rails m_rails;
    protected float m_railsIndex;
    protected float m_railsProgress;

    protected enum State
    {
        Waiting,
        Activated,
        Disactivated
    }

    protected State m_state;

    #endregion

    public Rails Rails
    {
        get { return m_rails; }
        set { m_rails = value; }
    }

    public float RailsIndex
    {
        get { return m_railsIndex; }
        set { m_railsIndex = value; }
    }

    public float RailsProgress
    {
        get { return m_railsProgress; }
        set { m_railsProgress = value; }
    }

    public virtual void Start()
    {
        m_weaknesses = new List<GadgetAbility>();
        if(_vignette != null)
        {
            m_vignetteColor = _vignette.color;
            m_vignetteColor.a = 0f;
            _vignette.color = m_vignetteColor;
        }
        
    }

    public virtual void Update()
    {
        float dist = Vector3.Distance(Score.Instance.Body.transform.position, transform.position);
        if (dist < _maxDistVignette && dist > _minDistVignette)
        {
            m_vignetteColor.a = Mathf.Lerp(0f, 1f, (dist - _minDistVignette) / (_fadeDistVignette - _minDistVignette) );
            _vignette.color = m_vignetteColor;
            _vignette.gameObject.transform.rotation = Quaternion.LookRotation(Vector3.Scale(Score.Instance.Body.transform.position - transform.position, Vector3.right));
        }

        if (dist <= _activateDistance && m_state == State.Waiting && Score.Instance.Body.transform.position.x < transform.position.x)
        {
            Activate();
        }

        if (dist <= _disactivateDistance && m_state == State.Activated)
        {
            Disactivate();
        }
    }

    public virtual void Activate()
    {
        m_state = State.Activated;
    }

    public virtual void Disactivate()
    {
        m_state = State.Disactivated;
    }

    public virtual void OnTriggerExit(Collider other)
    {
        if (other.collider.CompareTag("ChunkDelimiter"))
        {
            Destroy(this);
        }
    }
}

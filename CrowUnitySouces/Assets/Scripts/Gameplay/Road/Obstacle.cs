using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class Obstacle : MonoBehaviour
{
    #region members

    protected List<GadgetAbility> m_weaknesses;
    protected Rails m_rails;
    protected float m_railsIndex;
    protected float m_railsProgress;

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
    }

    public virtual void Behaviour()
    {
    }

    public virtual void OnTriggerExit(Collider other)
    {
        if (other.collider.CompareTag("ChunkDelimiter"))
        {
            Destroy(this);
        }
    }
}

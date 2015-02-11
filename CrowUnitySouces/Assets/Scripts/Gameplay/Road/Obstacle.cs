using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class Obstacle : MonoBehaviour
{
    #region members

    protected List<GadgetAbility> m_weaknesses;

    #endregion

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

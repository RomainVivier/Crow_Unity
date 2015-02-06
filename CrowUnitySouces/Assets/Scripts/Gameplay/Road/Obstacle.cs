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

    void OnTriggerExit(Collider other)
    {
        //Debug.Log("No longer in contact with " + other.transform.name);

        if (other.collider.CompareTag("ChunkDelimiter"))
        {
            Destroy(this);
        }
    }
}

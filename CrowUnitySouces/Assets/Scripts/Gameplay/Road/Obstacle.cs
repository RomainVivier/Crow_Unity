using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class Obstacle : MonoBehaviour
{

    #region members

    public GameObject _prefab;
    protected List<GadgetAbility> m_weaknesses;

    #endregion

    public virtual void Start()
    {
        m_weaknesses = new List<GadgetAbility>();
    }

    public virtual void Behaviour()
    {
    }
}

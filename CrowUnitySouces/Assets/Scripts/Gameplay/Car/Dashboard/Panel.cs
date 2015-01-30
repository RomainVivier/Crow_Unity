using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Panel : MonoBehaviour
{

    #region Members
    public Transform _spawnPoint;
    public float _distanceToUnlock;

    private GameObject m_panel;
    private GadgetButton[] m_buttons;
    private bool m_isVisible;

    #endregion

    #region Properties

    public bool IsVisible
    {
        get { return m_isVisible; }
        set
        {
            if(value)
            {
                //TODO lancé l'anim pour afficher le panel
            }

            m_isVisible = value;
        }
    }

    #endregion

    #region Panel functions

    public void Init()
    {
        // define instanciate path
        //m_panel = GameObject.Instantiate(Resources.Load(""), _spawnPoint.position, Quaternion.identity)  as GameObject;

        m_buttons = gameObject.GetComponentsInChildren<GadgetButton>();

        for (int i = 0; i < m_buttons.Length; i++)
        {
            m_buttons[i].Init();
        }
    }

    public List<GadgetAbility> AddAbilities()
    {
        //TODO
        List<GadgetAbility> tempAbilities = new List<GadgetAbility>();

        for (int i = 0; i < m_buttons.Length; i++)
        {
            tempAbilities.AddRange(m_buttons[i]._abilities);
        }

        return tempAbilities;
    }

    #endregion

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position, transform.localScale);
    }
}

﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Panel : MonoBehaviour
{

    #region Members
    public string _panelModelName;
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
        string panel = "PanelModel/"+ _panelModelName + "_" + Random.Range(1, 2);
        Debug.Log(panel);

        m_panel = GameObject.Instantiate(Resources.Load(panel), transform.position, Quaternion.AngleAxis(-90, Vector3.up)) as GameObject;
        m_panel.transform.parent = this.transform;
        m_panel.transform.localScale = Vector3.one;

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

}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Panel : MonoBehaviour
{

    #region Members
    public string _panelModelName;
    public float _distanceToUnlock;
    public Animator _anim;
	public bool _randomizeGadgets = true;
	public bool _randomizeLayout = true;
	public int _forcedIndex;


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
            if (value)
            {
                _anim.SetTrigger("Open");
            }
            else
            {
                _anim.SetTrigger("Close");
            }

            m_isVisible = value;
        }
    }

    #endregion

    #region Panel functions

    public void Init()
    {
		int panelIndex;
		if(_randomizeLayout)
			panelIndex = Random.Range(1, 2);
		else
			panelIndex = _forcedIndex;
		string panel = "PanelModel/"+ _panelModelName + "_" + panelIndex;

        m_panel = GameObject.Instantiate(Resources.Load(panel), transform.position, Quaternion.Euler(new Vector3(-10f, -90f, 0f))) as GameObject;
        m_panel.transform.parent = this.transform;
        m_panel.transform.localScale = Vector3.one;

        if (_distanceToUnlock > 0f)
        {
            return;
        }
        else
        {
            IsVisible = true;
        }

        Invoke("InitButtons", 1f);
    }

    public void InitButtons()
    {
        m_buttons = gameObject.GetComponentsInChildren<GadgetButton>();

        for (int i = 0; i < m_buttons.Length; i++)
        {
			if(_randomizeGadgets)
				m_buttons[i].AssignRandomGadget();
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

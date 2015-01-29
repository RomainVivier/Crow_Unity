using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Panel : MonoBehaviour
{

    #region Members

    public float _distanceToUnlock;
    private bool m_isVisible;
    private List<GadgetButton> m_buttons;

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


    #region Mono Functions

    void Start ()
    {
	
	}
	
	void Update ()
    {

    }

    #endregion 

    #region Panel



    #endregion 
}

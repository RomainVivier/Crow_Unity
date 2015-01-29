using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Dashboard : MonoBehaviour
{


    #region Members

    private List<GadgetAbility> m_possessedAbilities;
    private List<Panel> m_panels;

    #endregion

    #region Mono Function

    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

    }

    #endregion 

    #region Dashboard functions

    /// <summary>
    /// Here we generate the dashboard based on panels presets
    /// </summary>
    public void Generate()
    {
        //TODO get a random panels preset and generate each of them, add GadgetAbility depending on gadget link to the buttons
    }

    /// <summary>
    /// Update panel to make the change if any change is required
    /// </summary>
    public void UpdatePanels()
    {
        //TODO update each panel of the dashboard
    }

    #endregion
}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Dashboard : MonoBehaviour
{

    #region Members
    private List<GadgetAbility> m_abilities;
    private GameObject m_dashboard;
    private Panel[] m_panels;

    #endregion

    #region Mono Function

    void Start ()
    {
        string dashboard = "Dashboard_" + Random.Range(1, 2);

        m_dashboard = GameObject.Instantiate(Resources.Load(dashboard), transform.position, Quaternion.AngleAxis(-90, Vector3.up))  as GameObject;
        m_dashboard.transform.parent = this.transform;

        m_panels = gameObject.GetComponentsInChildren<Panel>();

        for (int i = 0; i < m_panels.Length; i++)
        {
            m_panels[i].Init();
            m_abilities.AddRange(m_panels[i].AddAbilities());
        }
	}
	
	// Update is called once per frame
	void Update ()
    {
        UpdatePanels();
    }

    #endregion 

    /// <summary>
    /// Update panel to make the change if any change is required
    /// </summary>
    private void UpdatePanels()
    {
        //TODO update each panel of the dashboard verify if the distance to unlock has been reached and add the abilities
    }
}

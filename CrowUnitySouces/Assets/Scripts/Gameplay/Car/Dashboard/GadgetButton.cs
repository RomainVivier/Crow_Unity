using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GadgetButton : MonoBehaviour
{

    #region Members

    public GadgetFamily _gadgetFamily;
    public string _gadgetID;
    public GadgetAbility[] _abilities;
    
    #endregion 
    
    #region GagdetButton Functions

    public void Init()
    {
        _gadgetID = GadgetManager.Instance.RandomUnassignGadget();
        _abilities = GadgetManager.Instance.GadgetAbilities(_gadgetID);
    }

    public void PlayGadget()
    {
        GadgetManager.Instance.PlayGadget(_gadgetID);
    }

    #endregion 

    void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(transform.position, transform.localScale);
    }
}

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

        BoxCollider bc = collider as BoxCollider;
        Gizmos.color = Color.cyan;
        Matrix4x4 temp = Gizmos.matrix;
        Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);
        Gizmos.DrawWireCube(Vector3.Scale(bc.center, transform.lossyScale), Vector3.Scale(transform.lossyScale, bc.size));
        Gizmos.matrix = temp;

    }
}

using UnityEngine;
using System.Collections;

public class GadgetButton : MonoBehaviour {

    public string _gadgetID;

    public void PlayGadget()
    {
        GadgetManager.Instance.PlayGadget(_gadgetID);
    }

}

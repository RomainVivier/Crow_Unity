using UnityEngine;
using System.Collections;

public class GadgetTrigger : MonoBehaviour {

    public string _gadgetID;

    public void PlayGadget()
    {
        GadgetManager.Instance.PlayGadget(_gadgetID);
    }

}

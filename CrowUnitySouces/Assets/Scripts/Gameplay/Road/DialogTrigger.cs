using UnityEngine;
using System.Collections;

public class DialogTrigger : MonoBehaviour
{
    #region attributes
    public string _name;
    #endregion

    #region methods

    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.transform.parent.GetComponent<Car>()!=null)
        {
            DialogsManager._instance.triggerEvent(DialogsManager.DialogInfos.EventType.TRIGGER, _name);
        }
    }
    #endregion
}

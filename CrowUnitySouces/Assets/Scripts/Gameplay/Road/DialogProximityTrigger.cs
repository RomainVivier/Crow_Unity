using UnityEngine;
using System.Collections;

public class DialogProximityTrigger : MonoBehaviour
{
    #region attributes
    public string _name;
    #endregion

    #region methods

    void Update()
    {
        DialogsManager._instance.triggerProximityEvent(_name, transform.position);
    }

    #endregion
}

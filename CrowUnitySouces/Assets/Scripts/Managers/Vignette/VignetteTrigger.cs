using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class VignetteTrigger : MonoBehaviour
{
    public List<int> _pos;
    public string _name;
    
    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.transform.root.gameObject.GetComponent<Car>()!=null)
        {
            VignetteManager.Instance.Pop(VignetteType.Front, _pos, _name);
        }
    }
}

using UnityEngine;
using System.Collections;

public class VignetteTrigger : MonoBehaviour
{
    public Vignette vignette;
    
    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.transform.root.gameObject.name.Equals("Car"))
        {
            //vignette.Pop();
        }
    }
}

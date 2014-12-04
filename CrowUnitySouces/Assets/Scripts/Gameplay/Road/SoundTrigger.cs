using UnityEngine;
using System.Collections;

public class SoundTrigger : MonoBehaviour {

    public string _soundName;

    void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            //Play Sound
        }
        
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position, transform.localScale);
    }
}

using UnityEngine;
using System.Collections;

public class SoundPlayer : MonoBehaviour
{

    #region Members

    public string _soundName;
    public bool _is3D=false;
    public bool _onlyOnce = true;

    private FMOD.Studio.EventInstance m_fmodEvent;
    private bool m_alreadyPlayed;
    #endregion

    #region MonoBehaviour

    void Start()
    {
        Debug.Log("coucou");
        m_fmodEvent=FMOD_StudioSystem.instance.GetEvent("event:/"+_soundName);
        m_alreadyPlayed = true;
    }
    #endregion

    #region Collider
    void OnTriggerEnter(Collider other)
    {
        Debug.Log("Trigger");
        if(other.gameObject.name=="Car" && (!_onlyOnce || m_alreadyPlayed))
        {
            m_alreadyPlayed=true;
            m_fmodEvent.start();
            if(!_is3D)
            {
                Vector3 dpos=transform.position-other.transform.position;
                dpos.Normalize();
                Vector3 right=other.transform.right;
                float diff=Vector3.Dot(dpos,right);
                FMOD.Studio.ParameterInstance param;
                m_fmodEvent.getParameter("Pan",out param);
                param.setValue(diff>0 ? 1 : 0); 
            }
        }
    }
    #endregion
}

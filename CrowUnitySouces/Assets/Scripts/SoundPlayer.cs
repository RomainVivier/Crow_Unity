using UnityEngine;
using System.Collections;

public class SoundPlayer : MonoBehaviour
{

    #region Members

    public string _soundName;
    public bool _is3D=false;
    public bool _onlyOnce = true;
    public float proba = 1;

    private FMOD.Studio.EventInstance m_fmodEvent;
    private bool m_alreadyPlayed;
    #endregion

    #region MonoBehaviour

    void Start()
    {
        //m_fmodEvent=FMOD_StudioSystem.instance.GetEvent("event:/"+_soundName);
        m_fmodEvent=FMOD_StudioSystem.instance.GetEvent("event:/SFX/Env Objects/envCarSwoosh");
        m_alreadyPlayed = true;
    }
    #endregion

    #region Collider
    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.name=="Car" && (!_onlyOnce || m_alreadyPlayed))
        {
            Debug.Log("coucou");
            m_alreadyPlayed=true;
            if(Random.Range(0f,1f)<proba)
            {
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
    }
    #endregion
}


///Le cube lilian le cube !!!
using UnityEngine;
using System.Collections;
using FMOD.Studio;

public class SoundPlayer : MonoBehaviour
{

    #region Members

    public string _soundName;
    public bool _is3D=false;
    public bool _onlyOnce = true;
    public float proba = 1;
    public float minSpeedKmh = 80;
    public int forcePan = 0;

    private FMOD.Studio.EventInstance m_fmodEvent;
    private bool m_alreadyPlayed;
    #endregion

    #region MonoBehaviour

    void Start()
    {
        m_fmodEvent=FMOD_StudioSystem.instance.GetEvent("event:/"+_soundName);
        m_alreadyPlayed = false;
    }
    #endregion

    #region Collider
    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.name=="Body" && other.gameObject.transform.parent.GetComponent<Car>()!=null
            && (!_onlyOnce || !m_alreadyPlayed))
        {
            m_alreadyPlayed=true;
            if(Random.Range(0f,1f)<proba)
            {
                float speedKmh = other.gameObject.transform.parent.GetComponent<Car>().getForwardVelocity() * 3.6f;
                if (speedKmh >= minSpeedKmh)
                {
                    Debug.Log("play " + _soundName);
                    m_fmodEvent.start();
                    if (!_is3D)
                    {
                        FMOD.Studio.ParameterInstance param;
                        m_fmodEvent.getParameter("Pan", out param);
                        if (forcePan == 0)
                        {
                            Vector3 dpos = transform.position - other.transform.position;
                            dpos.Normalize();
                            Vector3 right = other.transform.right;
                            float diff = Vector3.Dot(dpos, right);
                            param.setValue(diff > 0 ? 1 : 0);
                        }
                        else param.setValue(forcePan > 0 ? 1 : 0);
                    }
                    else
                    {
                        _3D_ATTRIBUTES threeDeeAttr = new _3D_ATTRIBUTES();
                        threeDeeAttr.position = UnityUtil.toFMODVector(transform.position);
                        threeDeeAttr.up = UnityUtil.toFMODVector(transform.up);
                        threeDeeAttr.forward = UnityUtil.toFMODVector(transform.forward);
                        m_fmodEvent.set3DAttributes(threeDeeAttr);
                    }
                }
            }
        }
    }
    #endregion
}


///Le cube lilian le cube !!!
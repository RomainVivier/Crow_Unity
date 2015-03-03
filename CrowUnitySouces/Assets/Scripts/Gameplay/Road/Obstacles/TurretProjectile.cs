using UnityEngine;
using System.Collections;
using FMOD.Studio;

public class TurretProjectile : MonoBehaviour {

    public float _speed;

    private FMOD.Studio.EventInstance m_event;
    private _3D_ATTRIBUTES m_threeDeeAttr;

    void Start()
    {
        m_event = FMOD_StudioSystem.instance.GetEvent("event:/SFX/Obstacles/Turrets/obsTurretMissile");
        m_event.start();
    }

	void Update ()
    {
        transform.position -= Vector3.right * Time.deltaTime * _speed;
        m_threeDeeAttr.position = UnityUtil.toFMODVector(transform.position);
        m_threeDeeAttr.up = UnityUtil.toFMODVector(transform.up);
        m_threeDeeAttr.forward = UnityUtil.toFMODVector(transform.forward);
        m_threeDeeAttr.velocity = UnityUtil.toFMODVector(Vector3.right*_speed-Car.Instance.transform.FindChild("Body").gameObject.rigidbody.velocity);
        m_event.set3DAttributes(m_threeDeeAttr);
	}

    void OnDestroy()
    {
        m_event.release();
    }
}

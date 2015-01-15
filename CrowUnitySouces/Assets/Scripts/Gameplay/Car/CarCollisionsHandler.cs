using UnityEngine;
using System.Collections;

public class CarCollisionsHandler : MonoBehaviour
{

    #region private members
    private FMOD.Studio.EventInstance m_impactVehicleSound;
    private FMOD.Studio.ParameterInstance m_impactVehicleSpeed;
    private FMOD.Studio.EventInstance m_impactConcreteSound;
    private FMOD.Studio.ParameterInstance m_impactConcreteSpeed;
    private Car m_car;
    private Timer cooldownTimer;

    #endregion

    #region MonoBehaviour
    void Start()
    {
        m_impactVehicleSound=FMOD_StudioSystem.instance.GetEvent("event:/SFX/Impacts/impactVehicle");
        m_impactVehicleSound.getParameter("Speed", out m_impactVehicleSpeed);
        m_impactConcreteSound=FMOD_StudioSystem.instance.GetEvent("event:/SFX/Impacts/impactConcrete");
        m_impactConcreteSound.getParameter("Speed", out m_impactConcreteSpeed);
        m_car = transform.parent.gameObject.GetComponent<Car>();
        cooldownTimer = new Timer();
        cooldownTimer.Reset(0.01f);
    }
    #endregion

    #region Collider
    void OnCollisionEnter(Collision collision)
    {
        GameObject oth = collision.gameObject;
        if (oth.name == "Body")
        {
            if (cooldownTimer.IsElapsedLoop)
            {
                playSound(collision, m_impactVehicleSound, m_impactVehicleSpeed);
                cooldownTimer.Reset(2f);
            }
        }
        else if (!collision.collider.isTrigger)
        {
            Vector3 contactPoint = collision.contacts[0].point;
            Vector3 center = transform.position;
            Vector3 diff = contactPoint - center;
            float relDy = Vector3.Dot(transform.up, diff);
            if (relDy > -0.7) playSound(collision, m_impactConcreteSound, m_impactConcreteSpeed);
        }
    }
    #endregion

    #region private methods
    void playSound(Collision collision, FMOD.Studio.EventInstance sound, FMOD.Studio.ParameterInstance param)
    {
        FMOD.Studio._3D_ATTRIBUTES threeDeeAttr = new FMOD.Studio._3D_ATTRIBUTES();
        threeDeeAttr.position = FMOD.Studio.UnityUtil.toFMODVector(collision.contacts[0].point);
        threeDeeAttr.up = FMOD.Studio.UnityUtil.toFMODVector(new Vector3(0,1,0));
        threeDeeAttr.forward = FMOD.Studio.UnityUtil.toFMODVector(collision.contacts[0].normal);
        threeDeeAttr.velocity = FMOD.Studio.UnityUtil.toFMODVector(Vector3.zero);
        sound.set3DAttributes(threeDeeAttr);
        float maxSpeed = m_car.maxSpeedKmh;
        RailsControl rc = m_car.gameObject.GetComponent<RailsControl>();
        if (rc != null) maxSpeed = rc.setSpeedKmh;
        Vector3 relativeVelocity = m_car.getVelocity();
        Rigidbody body=collision.gameObject.rigidbody;
        //if(body!=null) relativeVelocity-=body.GetRelativePointVelocity(new Vector3(0, 0, 0));
        param.setValue(Mathf.Min(relativeVelocity.magnitude*3.6f / maxSpeed,1));
        sound.start();
    }
    #endregion
}

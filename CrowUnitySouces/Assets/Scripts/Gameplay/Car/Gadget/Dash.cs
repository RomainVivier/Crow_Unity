using UnityEngine;
using System.Collections;

public class Dash : Gadget
{
    #region Members
    [System.Serializable]
    public class CameraParameters
    {
        public Camera camera;
        public float targetFOV;
        public Vector3 displacement;

        [HideInInspector]
        public float startFOV;
        [HideInInspector]
        public Vector3 startPos;
    }

    public CameraParameters[] _cameraParameters;
    public RailsControl _rc;
    public Car _car;
    public float _speedCoeff;
    public float _duration = 1f;
    public float _increaseFovInertia = 0.1f;
    public float _decreaseFovInertia = 0.01f;

    private Timer m_timer;
    private float m_cameraPos = 0;
    #endregion

    #region MonoBehaviour

    public override void Awake()
    {
        GadgetManager.Instance.Register("Dash", this);
        m_timer = new Timer();
        for (int i = 0; i < _cameraParameters.Length;i++)
        {
            CameraParameters cp=_cameraParameters[i];
            cp.startFOV = cp.camera.fieldOfView;
            cp.startPos = cp.camera.transform.localPosition;
        }
        base.Awake();
    }

    public override void Update()
    {
        if (m_timer.IsElapsedOnce)
        {
            Stop();
        }
        float pos = Mathf.Pow(IsReady ? _decreaseFovInertia : _increaseFovInertia, Time.deltaTime);
        m_cameraPos = Mathf.Lerp(IsReady ? 0 : 1, m_cameraPos, pos);
        for (int i = 0; i < _cameraParameters.Length;i++)
        {
            CameraParameters cp=_cameraParameters[i];
            cp.camera.fieldOfView = Mathf.Lerp(cp.startFOV, cp.targetFOV,m_cameraPos);
            cp.camera.transform.localPosition = cp.startPos + m_cameraPos * cp.displacement;
        }
        base.Update();
    }

    #endregion

    public override void Play()
    {
        base.Play();
		FMOD_StudioSystem.instance.PlayOneShot("event:/SFX/Gadgets/Boost/gadgetBoostExecute",transform.position);
        _rc.setSpeedKmh *= _speedCoeff;
        //_car.InstantSetSpeedKmh(_rc.setSpeedKmh);
        _car.gameObject.GetComponent<PolynomialEngine>().maxPowerKw *= 100;
        _car.gameObject.GetComponent<PolynomialEngine>().powerMinRpmKw*= 100;
        if(!_car.isSteering() && Vector3.Dot(_car.getForwardVector(),_car.getForwardTarget())>0.99) _car.transform.FindChild("Body").GetComponent<Rigidbody>().AddRelativeTorque(-10000, 0, 0, ForceMode.Impulse);
        _car.gameObject.GetComponent<Transmission>().lockGear(4);
        //_car.gameObject.transform.FindChild("Body/CenterOfMass").localPosition += new Vector3(0, -0.25f, 0);
        _car.maxSpeedKmh *= _speedCoeff;
        _car.updateValues();
        m_timer.Reset(_duration);
        IsReady = false;
    }

    public override void Stop()
    {
        base.Stop();

        _rc.setSpeedKmh /= _speedCoeff;
        _car.gameObject.GetComponent<PolynomialEngine>().maxPowerKw /= 100;
        _car.gameObject.GetComponent<PolynomialEngine>().powerMinRpmKw/= 100;
        _car.gameObject.GetComponent<Transmission>().lockGear(1);
        _car.gameObject.GetComponent<Transmission>().lockGear(-1);
        //_car.gameObject.transform.FindChild("Body/CenterOfMass").localPosition -= new Vector3(0, -0.25f, 0);
        _car.maxSpeedKmh /= _speedCoeff;
        _car.updateValues();
        if (_car.getForwardVelocityKmh() > _car.maxSpeedKmh) _car.InstantSetSpeedKmh(_car.maxSpeedKmh);
        IsReady = true;
	
    }
}

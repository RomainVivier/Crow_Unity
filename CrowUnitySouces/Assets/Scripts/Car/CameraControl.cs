using UnityEngine;
using System.Collections;

public class CameraControl : MonoBehaviour {

    public float _maxRotation;
    public float _rotateSpeed;
    public Camera _camera;

    //crappy
    public ProtoController _pc;
    private Timer m_timerMP;
    private Timer m_timerRocket;

    private float _initialAngle;
    private float _currentRotation;

    public GameObject _penis;
    private Timer m_timerPenis;

    //end of crap

	void Start ()
    {
        if(_camera == null)
        {
            Debug.LogError("Camera reference can't be null");
            return;
        }

        _currentRotation = _camera.transform.rotation.eulerAngles.y;
        _initialAngle = _currentRotation;

        KeyBinder.Instance.DefineActions("MouseLeftClick", new KeyActionConfig(KeyType.Action, 0, OnClick, null));


        //crappy
        m_timerMP = new Timer();
        m_timerRocket = new Timer();
        m_timerPenis = new Timer();
	}
	
	void Update ()
    {
        Vector3 currEuler = _camera.transform.localRotation.eulerAngles;

        if(Input.mousePosition.x <= 0)
        {
            currEuler.y -= (_rotateSpeed * Time.deltaTime);
        }
        else if (Input.mousePosition.x >= Screen.width)
        {
            currEuler.y += (_rotateSpeed * Time.deltaTime);
        }
        else
        {
            if (currEuler.y > 0 && currEuler.y < 180)
            {
                currEuler.y = Mathf.Clamp( currEuler.y - (_rotateSpeed * Time.deltaTime), 0, _maxRotation );
            }

            if (currEuler.y > 180 && currEuler.y < 360)
            {
                currEuler.y = Mathf.Clamp(currEuler.y + (_rotateSpeed * Time.deltaTime), (360 - _maxRotation), 360);
            }
        }

        if( currEuler.y > _maxRotation && currEuler.y < 180 )
        {
            currEuler.y = _maxRotation;
        }

        if( currEuler.y > 180 && currEuler.y < (360 - _maxRotation))
        {
            currEuler.y = (360 - _maxRotation);
        }
            
        _camera.transform.localRotation = Quaternion.Euler(currEuler);


        //crappy
        if(m_timerMP.IsElapsedOnce)
        {
            _pc.DeactivateMP();
        }

        if (m_timerRocket.IsElapsedOnce)
        {
            _pc.DeactivateRocket();
        }

        if (m_timerPenis.IsElapsedOnce)
        {
            _penis.SetActive(false);
        }
    }

    void OnClick()
    {
        Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, ~(1 << LayerMask.NameToLayer("Player"))))
        {

            if (hit.collider.CompareTag("Clickable"))
            {
                if (hit.collider.name == "button0")
                {
                    _pc.ActivateRocket();
                    _pc.PlayRocket();
                    m_timerRocket.Reset(2f);
                }

                if (hit.collider.name == "button1")
                {
                    _penis.SetActive(true);
                    m_timerPenis.Reset(10f);
                }

                if(hit.collider.name == "flip_flop")
                {
                    _pc.ActivateMP();
                    _pc.PlayMP();
                    m_timerMP.Reset(2f);
                }
            }
        }
    }

}

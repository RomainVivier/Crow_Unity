using UnityEngine;
using System.Collections;

public class CameraControl : MonoBehaviour {

    public float _maxRotation;
    public float _rotateSpeed;
    public Camera _camera;

	void Start ()
    {
        if(_camera == null)
        {
            Debug.LogError("Camera reference can't be null");
            return;
        }

        KeyBinder.Instance.DefineActions("MouseLeftClick", new KeyActionConfig(KeyType.Action, 0, OnClick, null));

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
					//rocket
                }

                if (hit.collider.name == "button1")
                {
					//penis
                }

                if(hit.collider.name == "flip_flop")
                {
                    Debug.Log("Spoonbill !");
                    GadgetManager.Instance.PlayGadget("Spoonbill");
                }
            }
        }
    }

}

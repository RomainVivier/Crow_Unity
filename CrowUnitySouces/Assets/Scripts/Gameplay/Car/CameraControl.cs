﻿using UnityEngine;
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


		if(Input.GetKeyDown(KeyCode.S))
			GadgetManager.Instance.PlayGadget("Spoonbill");
    }

    void OnClick()
    {
        Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, ~(1 << LayerMask.NameToLayer("Player"))))
        {
            //Debug.Log(hit.collider.name);

            if (hit.collider.CompareTag("Clickable"))
            {
                GadgetButton gt = hit.collider.GetComponent<GadgetButton>(); 

                if(gt)
                {
                    gt.PlayGadget();
                }
				else
				{
					Animator anim = hit.collider.GetComponent<Animator>();
					if(anim != null)
						StartCoroutine("AutoPress", anim);
				}
            }
        }
    }

	IEnumerator AutoPress(Animator anim)
	{
		anim.SetTrigger("Press");
		yield return new WaitForSeconds(1);
		anim.SetTrigger("Unpress");
	}

}

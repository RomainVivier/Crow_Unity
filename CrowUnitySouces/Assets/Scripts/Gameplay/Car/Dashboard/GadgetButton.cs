﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GadgetButton : MonoBehaviour
{

    #region Members

    public GadgetFamily _gadgetFamily;
    public string _gadgetID;
    public GadgetAbility[] _abilities;
    public Vector2 _swipeVector=Vector2.zero;
    public bool _activatedOnStart = false;

	private GameObject m_cooldownPanel;
    private Vector2 m_startPoint;
    private float m_tgtAngle;
    private Gadget m_gadget;

    private const int SWIPE_TOLERANCE_DEG = 45;
    private Animator m_anim;
    #endregion 
    
    #region GagdetButton Functions

	public void AssignRandomGadget()
	{
		_gadgetID = GadgetManager.Instance.RandomUnassignGadget();
	}

    public void Init()
    {
        var gadget = GadgetManager.Instance.getGadgetById(_gadgetID);
        if(gadget != null)
        {
            gadget._buttonAnim = GetComponent<Animator>();
            gadget._buttonAnim.SetTrigger("Activate");
        }
        _abilities = GadgetManager.Instance.GadgetAbilities(_gadgetID);
    }

    public void Start()
    {
        m_tgtAngle=Mathf.Atan2(_swipeVector.y,_swipeVector.x);
        m_gadget = GadgetManager.Instance.GetGadget(_gadgetID);
		m_cooldownPanel = Resources.Load ("CooldownMark") as GameObject;
		
		if(_activatedOnStart)
        {
            Init();
        }
    }

    public void OnLeftClickPress()
    {
        if(_gadgetID == null)
        {
            return;
        }

        if (_swipeVector == Vector2.zero) {
			GadgetManager.Instance.PlayGadget (_gadgetID);
			Gadget gad = GadgetManager.Instance.getGadgetById(_gadgetID);

			GameObject cooldownPanel = GameObject.Instantiate(m_cooldownPanel) as GameObject;
			cooldownPanel.GetComponent<FakeCooldown>()._lengthInSeconds = gad._cooldown;
			cooldownPanel.transform.parent = transform;
			//cooldownPanel.transform.localScale = Vector3.one;
			cooldownPanel.transform.localPosition = new Vector3(0, 0.25f, 0);
			cooldownPanel.transform.localEulerAngles = new Vector3(90f, 0, 0);
		}
		else m_startPoint = new Vector2(Input.mousePosition.x, Input.mousePosition.y) / Screen.height;
    }

    public void OnLeftClickRelease()
    {
        if(_gadgetID == null)
            return;

        if(_swipeVector!=Vector2.zero)
        {
            Vector2 vec = (new Vector2(Input.mousePosition.x, Input.mousePosition.y) / Screen.height)-m_startPoint;
            if(vec.sqrMagnitude>_swipeVector.sqrMagnitude)
            {
                if (m_gadget && m_gadget._invertGesture) vec = -vec;
                float angle = Mathf.Atan2(vec.y, vec.x);
			
                if( (angle>m_tgtAngle-SWIPE_TOLERANCE_DEG*Mathf.Deg2Rad && angle<m_tgtAngle+SWIPE_TOLERANCE_DEG*Mathf.Deg2Rad))
                    //|| (angle>m_tgtAngle-(SWIPE_TOLERANCE_DEG+360)*Mathf.Deg2Rad && angle<m_tgtAngle+(SWIPE_TOLERANCE_DEG+360)*Mathf.Deg2Rad)
                    //|| (angle>m_tgtAngle-(SWIPE_TOLERANCE_DEG-360)*Mathf.Deg2Rad && angle<m_tgtAngle+(SWIPE_TOLERANCE_DEG-360)*Mathf.Deg2Rad))
                    GadgetManager.Instance.PlayGadget(_gadgetID);
            }
        }
    }
    #endregion

    void OnDrawGizmos()
    {
        BoxCollider bc = collider as BoxCollider;
        Gizmos.color = Color.cyan;
        Matrix4x4 temp = Gizmos.matrix;
        Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);
        Gizmos.DrawWireCube(Vector3.Scale(bc.center, transform.lossyScale), Vector3.Scale(transform.lossyScale, bc.size));
        Gizmos.matrix = temp;
    }
}

using UnityEngine;
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

    public MeshRenderer _buttonRenderer;
    public Color _darkColor;
    public Color _brightColor;

    private Color m_tempColor;
    private Timer m_cooldownTimer;
    private bool m_isInCooldown;

    private Vector2 m_startPoint;
    private float m_tgtAngle;
    private Gadget m_gadget;

    private const int SWIPE_TOLERANCE_DEG = 45;
    private Animator m_anim;
    #endregion 
    
    
    public float Cooldown
    {
        set
        {
            m_cooldownTimer.Reset(value);
            m_isInCooldown = true;
        }
    }

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
            gadget._button = this;
        }
        _abilities = GadgetManager.Instance.GadgetAbilities(_gadgetID);
    }

    void Start()
    {
        m_tgtAngle=Mathf.Atan2(_swipeVector.y,_swipeVector.x);
        m_gadget = GadgetManager.Instance.GetGadget(_gadgetID);
        m_cooldownTimer = new Timer();
        m_isInCooldown = false;
        
        if(_activatedOnStart)
        {
            Init();
        }
    }

    void Update()
    {
        if (_buttonRenderer != null && !m_cooldownTimer.IsElapsedLoop && m_isInCooldown)
        {
            m_tempColor = Vector4.Lerp(_darkColor, _brightColor, (1 - m_cooldownTimer.CurrentNormalized));
            _buttonRenderer.renderer.material.SetColor("_Color", m_tempColor);
        }
        else
        {
            m_isInCooldown = false;
        }
    }

    public void OnLeftClickPress()
    {
        if(_gadgetID == null)
        {
            return;
        }

        if (_swipeVector == Vector2.zero) GadgetManager.Instance.PlayGadget(_gadgetID);
        else m_startPoint = new Vector2(Input.mousePosition.x, Input.mousePosition.y) / Screen.height;
    }

    public void OnLeftClickRelease()
    {
        
        if(_gadgetID == null)
        {
            return;
        }

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

using UnityEngine;
using System.Collections;

public class TouchManager : MonoBehaviour
{

    #region Delegates

    public delegate void SwipeDelegate();
    public delegate void TouchDelegate(SwipeInfos si);

    public SwipeDelegate _swipeLeft;
    public SwipeDelegate _swipeRight;

    public TouchDelegate _touchStart;
    public TouchDelegate _touchStay;
    public TouchDelegate _touchEnd;
    public TouchDelegate _touchEndZone;

    #endregion Delegates

    public struct SwipeInfos
    {
        public Vector2 swipeStart;
        public Vector2 swipeEnd;
        public bool inSwipe;
        public bool inZoneSwipe;
    };
    private struct TouchInfos
    {
        public enum State{UNHELD,BEGIN,HELD,END};
        public State state;
        public Vector2 pos;
    }
    private SwipeInfos[] m_swipeInfos;

    private static TouchManager m_instance;

    private Vector2 m_wheelCenter;
    private float m_wheelRadius;

    #region Singleton

    public static TouchManager Instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = GameObject.FindObjectOfType<TouchManager>();
                if (m_instance == null)
                {
                    GameObject singleton = new GameObject();
                    singleton.name = "TouchManager";
                    m_instance = singleton.AddComponent<TouchManager>();
                }
            }

            return m_instance;
        }
    }

    void Awake()
    {
        if (m_instance == null)
        {
            m_instance = this;
            m_instance.Init();
        }
        else
        {
            if (this != m_instance)
                Destroy(this.gameObject);
        }
    }

    private void Init()
    {
        m_swipeInfos=new SwipeInfos[11];
        for (int i = 0; i < 11; i++) m_swipeInfos[i].inSwipe = false;

        _touchEnd += Swipe;
        _touchEndZone += SwipeZone;
        m_wheelCenter.x = -0.44f * Screen.height + Screen.width / 2;
        m_wheelCenter.y = 0.062f;
        m_wheelRadius = 0.2f * Screen.height;//0.155f
    }

    #endregion

    void Update()
    {
        Touch();
    }

    #region TouchFunctions

    public void Touch()
    {
        TouchInfos t=new TouchInfos();
        t.state = TouchInfos.State.UNHELD; ;
        if (Input.GetMouseButtonDown(0)) t.state = TouchInfos.State.BEGIN;
        else if (Input.GetMouseButton(0)) t.state = TouchInfos.State.HELD;
        else if (Input.GetMouseButtonUp(0)) t.state = TouchInfos.State.END;
        t.pos = new Vector2(Input.mousePosition.x,Input.mousePosition.y);
        handleTouch(t,ref m_swipeInfos[0]);

        for (int i = 0; i < Input.touchCount; i++)
        {
            switch(Input.touches[i].phase)
            {
                case TouchPhase.Began:
                    t.state = TouchInfos.State.BEGIN;
                    break;
                case TouchPhase.Moved:
                    t.state = TouchInfos.State.HELD;
                    break;
                case TouchPhase.Ended:
                    t.state = TouchInfos.State.END;
                    break;
                default:
                    t.state = TouchInfos.State.UNHELD;
                    break;
            }
            handleTouch(t, ref m_swipeInfos[i + 1]);
        }
    }

    private void handleTouch(TouchInfos ti, ref SwipeInfos si)
    {
        if (ti.state==TouchInfos.State.BEGIN)
        {
            if ((ti.pos - m_wheelCenter).magnitude < m_wheelRadius) si.inZoneSwipe = true;
            si.inSwipe = true;
            si.swipeStart = ti.pos;
            if (_touchStart != null)
            {
                _touchStart(si); 
            }
    }
        bool endSwipe = false;
        bool endZoneSwipe = false;
        if (ti.state==TouchInfos.State.HELD)
        {
            if ((ti.pos - m_wheelCenter).magnitude > m_wheelRadius && si.inZoneSwipe) endZoneSwipe = true;
            if (_touchStay != null)
            {
                _touchStay(si);
            }
        }

        if (ti.state==TouchInfos.State.END && si.inSwipe ) endSwipe = true;
        if (ti.state==TouchInfos.State.END && si.inZoneSwipe ) endZoneSwipe = true;

        if(endSwipe)
        {
            si.inSwipe=false;
            si.swipeEnd = ti.pos;
            if(_touchEnd != null)
            {
                _touchEnd(si);
            }
        }

        if(endZoneSwipe)
        {
            si.inZoneSwipe = false;
            si.swipeEnd = ti.pos;
            if(_touchEndZone != null)
            {
                _touchEndZone(si);
            }
        }
    }

    public void Swipe(SwipeInfos si)
    {
        Vector2 swipeVector = si.swipeStart - si.swipeEnd;
        if(swipeVector.magnitude > (Screen.width / 6) )
        {
            if(swipeVector.x > 0)
            {
                if (_swipeRight != null)
                {
                    _swipeRight();
                }
            }
            else
            {
                if (_swipeLeft != null)
                {
                    _swipeLeft();
                }
            }
        }
    }

    public void SwipeZone(SwipeInfos si)
    {
        Vector2 swipeVector = si.swipeStart - si.swipeEnd;
        if(swipeVector.magnitude > m_wheelRadius/4)//(Screen.width / 6) )
        {
            if(swipeVector.x > 0)
            {
                if (_swipeRight != null)
                {
                    _swipeRight();
                }
            }
            else
            {
                if (_swipeLeft != null)
                {
                    _swipeLeft();
                }
            }
        }
    }

    #endregion TouchFunctions
}

﻿using UnityEngine;
using System.Collections;

public class TouchManager : MonoBehaviour
{

    #region Delegates

    public delegate void SwipeLeft();
    public delegate void SwipeRight();

    public delegate void TouchStart();
    public delegate void TouchStay();
    public delegate void TouchEnd();


    public SwipeLeft _swipeLeft;
    public SwipeLeft _swipeRight;

    public SwipeLeft _touchStart;
    public SwipeLeft _touchStay;
    public SwipeLeft _touchEnd;

    #endregion Delegates

    private static TouchManager m_instance;
    private Vector2 m_swipeStart;
    private Vector2 m_swipeEnd;


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
        _touchEnd += Swipe;
    }

    #endregion

    void Update()
    {
        Touch();
    }

    #region TouchFunctions

    public void Touch()
    {

#if UNITY_STANDALONE
        if (Input.GetMouseButtonDown(0))
        {
            m_swipeStart = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            if (_touchStart != null)
            {
                _touchStart(); 
            }
        }

        if (Input.GetMouseButton(0))
        {
            if (_touchStay != null)
            {
                _touchStay();
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            m_swipeEnd = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            if(_touchEnd != null)
            {
                _touchEnd();
            }
            
        }

#elif UNITY_ANDROID
		if(Input.touchCount > 0)
		{
			if(Input.GetTouch(0).phase == TouchPhase.Began)
			{
				m_swipeStart = new Vector2(Input.touches[0].position.x, Input.touches[0].position.y);
                if (_touchStart != null)
                {
                    _touchStart(); 
                }
			}
			if(Input.GetTouch(0).phase == TouchPhase.Moved)
			{
                if (_touchStay != null)
                {
                    _touchStay();
                }
			}
			if(Input.GetTouch(0).phase == TouchPhase.Ended)
			{
                if(Input.touchCount > 0)
                {
                    m_swipeEnd = new Vector2(Input.touches[0].position.x, Input.touches[0].position.y);
                }
                else
                {
                    m_swipeEnd = new Vector2(Mathf.Infinity, Mathf.Infinity);
                }

                if(_touchEnd != null)
                {
                    _touchEnd();
                }
			}
		}
#endif

    }

    public void Swipe()
    {
        Vector2 swipeVector = m_swipeStart - m_swipeEnd;

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

    #endregion TouchFunctions
}
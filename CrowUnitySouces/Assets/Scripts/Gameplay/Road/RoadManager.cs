using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RoadManager : MonoBehaviour
{

    #region Enumeration

    public enum RoadState
    {
        Intro,
        Test,
        Game
    }

    #endregion

    #region Members

    public Transform _startPoint;
    public int _numberOfChunk;
    public bool _usePattern;
    public string _pattern;

    private RoadState m_state;
    private string m_introChunk;
    private string m_testChunk;
    private List<RoadChunk> m_chunks = new List<RoadChunk>();
    private RoadChunk m_lastChunk;

    private RoadManager m_instance;

    #endregion

    #region Singleton

    public RoadManager Instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = GameObject.FindObjectOfType<RoadManager>();
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
        if (_numberOfChunk <= 1)
        {
            Debug.LogError("You need to have more than one chunk to generate an infinite road.");
            return;
        }

        for (int i = 0; i < _numberOfChunk; i++)
        {
            m_chunks.Add(new RoadChunk());
        }

        m_lastChunk = m_chunks[_numberOfChunk - 1];

        Generate();
    }

    #endregion


    #region Properties

    public RoadState State
    {
        get { return m_state; }
        set { m_state = value; }
    }

    #endregion

    #region MonoBehaviour

    //void Start()
    //{
    //    if(_numberOfChunk <= 1)
    //    {
    //        Debug.LogError("You need to have more than one chunk to generate an infinite road.");
    //        return;
    //    }

    //    Generate();
    //}
	
	void Update()
    {
        for(int i = 0; i < _numberOfChunk; i++)
        {
            if(m_chunks[i].IsUnused)
            {
                PlaceChunk(i);
                m_chunks[i].IsUnused = false;
            }
        }

    }

    #endregion


    #region Functions

    void Generate()
    {
        if(_startPoint == null)
        {
            Debug.LogError("Start point can't be null.");
        }
        
        for (int i = 0; i < _numberOfChunk; i++)
        {

            if (i == 0)
            {
                
            }
            else
            {
                
            }

            
        }

        m_lastChunk = m_chunks[_numberOfChunk - 1];


    }


    void PlaceChunk( int order )
    {
        if(order >= _numberOfChunk)
        {
            Debug.LogError("The order of the moved chunk can't be greater or equal to the number of chunks.");
            return;
        }

        RoadChunk rc = m_chunks[order];

        Debug.Log("position = " + m_lastChunk.transform.position);
        rc.transform.position = m_lastChunk._endPoint.position + rc.StartToCenter;

        m_lastChunk = rc;
    }

    #endregion 
}
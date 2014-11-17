﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RoadManager : MonoBehaviour
{

    #region Members

    public Transform _startPoint;
    public int _numberOfChunk;

    /// <summary>
    /// key (int) orders of the chunk,
    /// Value (RoadChunk) RoadChunk component.
    /// </summary>
    private List<RoadChunk> m_chunks = new List<RoadChunk>();
    private RoadChunk m_lastChunk; 

    #endregion

    #region MonoBehaviour

    void Start()
    {
        if(_numberOfChunk <= 1)
        {
            Debug.LogError("You need to have more than one chunk to generate an infinite road.");
            return;
        }

        Generate();
	}
	
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

        for (int i =0; i < _numberOfChunk; i++)
        {
            GameObject chunk = GameObject.Instantiate(Resources.Load("RoadChunk")) as GameObject;
            RoadChunk rc = chunk.GetComponent<RoadChunk>();
            if(rc == null)
            {
                Debug.LogError("Fail during the road generation one chunk does not possess \"RoadChunk\" script.");
                return;
            }

            if(m_chunks.Count > 0)
            {
                rc.transform.position = m_chunks[m_chunks.Count -1]._endPoint.position + rc.StartToCenter;
            }else{
                rc.transform.position = _startPoint.position;
            }

            m_chunks.Add(rc);
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

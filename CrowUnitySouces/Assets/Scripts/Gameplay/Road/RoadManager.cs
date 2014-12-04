using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RoadManager : MonoBehaviour
{

    #region Members

    public Transform _startPoint;
    public int _numberOfChunk;
    public bool _useChunkPool = false;
    public ChunkPool _pool;

    public List<string> _introChunks = new List<string>();
    public List<string> _testChunks = new List<string>();
    public List<string> _gameChunks = new List<string>();

    private Queue<string> m_introChunks;
    private Queue<string> m_testChunks;
    private Queue<string> m_gameChunks;

    private List<RoadChunk> m_chunks = new List<RoadChunk>();
    private RoadChunk m_lastChunk;
    private GameObject m_car;
    
    private State m_currentState;
    private Theme m_currentTheme;

    #endregion

    #region Enum

    public enum State
    {
        Intro,
        Test,
        Game
    }

    #endregion

    #region Properties

    public State CurrentState
    {
        get { return m_currentState; }
        set { m_currentState = value; }
    }

    public Theme CurrentTheme
    {
        get { return m_currentTheme; }
        set { m_currentTheme = value; }
    }

    #endregion

    #region MonoBehaviour

    void Start()
    {
        if(_numberOfChunk <= 1)
        {
            Debug.LogError("You need to have more than one chunk to generate an infinite road.");
            return;
        }

        m_introChunks = new Queue<string>(_introChunks);
        m_testChunks = new Queue<string>(_testChunks);
        m_gameChunks = new Queue<string>(_gameChunks);

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
           
            GameObject chunk = PullChunk();
            if(chunk == null)
            {
                Debug.LogError("Fail during the road generation the chunk generated is null.");
                return;
            }
            
            RoadChunk rc = chunk.GetComponent<RoadChunk>();
            if(rc == null)
            {
                Debug.LogError("Fail during the road generation one chunk does not possess \"RoadChunk\" script.");
                return;
            }

            if(m_chunks.Count > 0)
            {
                rc.transform.position = m_chunks[m_chunks.Count -1]._endPoint.position + rc.StartToCenter;
				m_chunks[m_chunks.Count-1].NextChunk=rc;
            }else{
                rc.transform.position = _startPoint.position;
                m_car = GameObject.Instantiate(Resources.Load("Car"), (rc._startPoint.position + Vector3.up + Vector3.right), Quaternion.Euler(new Vector3(0,90,0))) as GameObject;
                m_car.GetComponent<RailsControl>().chunk = rc;
            }

            m_chunks.Add(rc);
        }

        m_lastChunk = m_chunks[_numberOfChunk - 1];
    }

    void PlaceChunk( int order )
    {

        /*
         * TODO
         * 
         * Mise en place de bon chunk
         * via la méthode PullChunk
         * 
        */

        if(order >= _numberOfChunk)
        {
            Debug.LogError("The order of the moved chunk can't be greater or equal to the number of chunks.");
            return;
        }

        RoadChunk rc = m_chunks[order];
		
        // has to be suppressed ?
        

        m_lastChunk.NextChunk=rc;
        rc.transform.position = m_lastChunk._endPoint.position + rc.StartToCenter;

        m_lastChunk = rc;
    }

    GameObject PullChunk()
    {
        GameObject chunk;
        string path = "";

        if (CurrentState == State.Intro)
        {
            if (m_introChunks.Count > 0)
            {
                path = m_introChunks.Dequeue();
            }
            else
            {
                CurrentState = State.Test;
            }    
        }
        
        if(CurrentState == State.Test)
        {
            if (m_testChunks.Count > 0)
            {
                path = m_testChunks.Dequeue();
            }
            else
            {
                CurrentState = State.Game;
            }
        }
        
        if (CurrentState == State.Game)
        {
            if(!_useChunkPool)
            {
                if (m_gameChunks.Count > 0)
                {
                    path = m_gameChunks.Dequeue();
                }
            }
            else
            {
                return _pool.GetUnusedChunk();
            }
        }

        chunk = GameObject.Instantiate(Resources.Load("Chunks/"+path)) as GameObject;
        chunk.name = path;

        return chunk;
    }

    #endregion

}

public enum Theme
{
    Country,
    City,
    Tunnel
}
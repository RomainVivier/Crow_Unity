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
    public float _lightRange;
    public float _lightIntensity;

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
        for (int i = (m_chunks.Count - 1); i >= 0; i--)
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


            Light light = chunk.transform.FindChild("Point light").GetComponent<Light>();
            if(light!=null)
            {
                light.range = _lightRange;
                light.intensity = _lightIntensity;
            }

            if(m_chunks.Count > 0)
            {
                rc.transform.position = m_chunks[m_chunks.Count -1]._endPoint.position + rc.StartToCenter;
				m_chunks[m_chunks.Count-1].NextChunk=rc;
            }else{
                rc.transform.position = _startPoint.position;
                m_car = GameObject.Instantiate(Resources.Load("CarV2"), (rc._startPoint.position + Vector3.up + Vector3.right*2 ), Quaternion.Euler(new Vector3(0,90,0))) as GameObject;
                m_car.name = "CarV2";
                m_car.GetComponent<RailsControl>().chunk = rc;
            }
            rc.Generate();


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

        // hide or remove chunk
        RoadChunk rc = m_chunks[order];
        rc.HideChunk();
        m_chunks.RemoveAt(order);

        // get next chunk and verify if it is null
        GameObject chunk = PullChunk();
        if (chunk == null)
        {
            Debug.LogError("Fail during the road generation the chunk generated is null.");
            return;
        }

        //add new chunk
        rc = chunk.GetComponent<RoadChunk>();
        rc.Generate();
        m_chunks.Add(rc);


        // position it and assign it as last chunk
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
                var tempgo = _pool.GetUnusedChunk();
                tempgo.SetActive(true);
                return tempgo;
            }
        }

        if (path != "")
        {
            chunk = GameObject.Instantiate(Resources.Load("Chunks/" + path)) as GameObject;
            chunk.name = path;
			chunk.GetComponent<RoadChunk>().Generate();
            return chunk;
        }
        else
        {
            return null;
        }
    }

    #endregion

}

public enum Theme
{
    Country,
    City,
    Tunnel
}
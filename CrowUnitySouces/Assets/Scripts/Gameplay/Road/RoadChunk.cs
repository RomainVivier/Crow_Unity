using UnityEngine;
using System.Collections;

public class RoadChunk : MonoBehaviour {

    #region Public Members
    public Transform _startPoint;
    public Transform _endPoint;

    public GameObject[] _environmentPoint;
    public Rails[] _rails;
    public Obstacle[] _obstacles;

    private bool m_isUnused = false;
    
    #endregion

    #region Properties

    public bool IsUnused
    { 
        get{ return m_isUnused; }

        set { m_isUnused = value; }
    }

    public Vector3 StartToCenter
    {
        get { return (transform.position - _startPoint.position); }
    }

    #endregion

    #region MonoBehaviours

    void OnTriggerExit(Collider other)
    {
        Debug.Log("No longer in contact with " + other.transform.name);

        if (other.collider.CompareTag("ChunkDelimiter"))
        {
            m_isUnused = true;
        }
    }

    #endregion

    #region Functions

    void Generate()
    {
        // instanciate piece of chunk

        // get environments / obstacles

        // generate environments

        // generate obstacles

    }

    void Generate(string pattern)
    {

    }

    #endregion
}

﻿using UnityEngine;
using System.Collections;

public class SwattingBuilding : MonoBehaviour
{

    #region attributes
    // Used building generators
    public BuildingGenerator _leftBuildingGenerator;
    public BuildingGenerator _rightBuildingGenerator;
    
    // Timing parameters
    public float _upTime = 3;
    public float _fallTime = 0.25f;
    public float _downTime = 4;
    public float _riseTime = 0.25f;

    private enum State
    {
        UP,
        FALLING,
        DOWN,
        RISING
    };

    private State m_state;
    private Timer m_timer;
    private BuildingGenerator m_usedBuildingGenerator;
    private GameObject m_buildingGameObject;
    #endregion

    #region methods
    void Start ()
    {
        // Set used building generator
        m_usedBuildingGenerator = Random.Range(0, 2) == 0 ? _leftBuildingGenerator : _rightBuildingGenerator;
        if (_leftBuildingGenerator == null) m_usedBuildingGenerator = _leftBuildingGenerator;
        if (_rightBuildingGenerator == null) m_usedBuildingGenerator = _rightBuildingGenerator;
        
        // Destroy the other if it exists
        if (m_usedBuildingGenerator == _leftBuildingGenerator && _rightBuildingGenerator != null)
        {
            GameObject.Destroy(_rightBuildingGenerator.gameObject);
            _rightBuildingGenerator = null;
        }
        if (m_usedBuildingGenerator == _rightBuildingGenerator && _leftBuildingGenerator != null)
        {
            GameObject.Destroy(_leftBuildingGenerator.gameObject);
            _leftBuildingGenerator = null;
        }

        // Regenerate the building with height=4
        m_usedBuildingGenerator._minHeight = 4;
        m_usedBuildingGenerator._maxHeight = 4;
        m_usedBuildingGenerator.regenerate();

        // Add a collider to the building
        BoxCollider bc=m_usedBuildingGenerator.gameObject.AddComponent<BoxCollider>();
        bc.center = new Vector3(0, 90, 0);
        bc.size = new Vector3(32, 180, 45);

        // Change layer and tag
        m_usedBuildingGenerator.gameObject.tag = "Obstacle";
        m_usedBuildingGenerator.gameObject.layer = LayerMask.NameToLayer("Obstacle");

        // Attach collision handler
        SwattingBuildingCollisionHandler sbch= m_usedBuildingGenerator.gameObject.AddComponent<SwattingBuildingCollisionHandler>();
        sbch._swattingBuilding = this;

        // Create a new gameobject to have the right pivot
        m_buildingGameObject = new GameObject();
        m_buildingGameObject.transform.position = m_usedBuildingGenerator.transform.position;
        m_buildingGameObject.transform.rotation = m_usedBuildingGenerator.transform.rotation;
        m_buildingGameObject.transform.position += m_usedBuildingGenerator.transform.right * 2.5f;
        m_usedBuildingGenerator.transform.SetParent(m_buildingGameObject.transform);
        m_buildingGameObject.transform.SetParent(transform);

        // Init state
        m_state = State.UP;
        m_timer = new Timer();
        m_timer.Reset(_upTime);

    }
	
	void Update ()
    {
        switch(m_state)
        {
            case State.UP:
                if(m_timer.IsElapsedOnce)
                {
                    // Start next state
                    m_state = State.FALLING;
                    m_timer.Reset(_fallTime);
                }
                break;
            case State.FALLING:
                if(m_timer.IsElapsedOnce)
                {
                    // Rotate the building in it's final position
                    Vector3 rot = m_buildingGameObject.transform.localRotation.eulerAngles;
                    rot.z = -90;
                    m_buildingGameObject.transform.localRotation=Quaternion.Euler(rot);

                    // Start next state
                    m_state = State.DOWN;
                    m_timer.Reset(_downTime);
                }
                else
                {
                    // Rotate the building
                    Vector3 rot = m_buildingGameObject.transform.localRotation.eulerAngles;
                    rot.z = -90+(m_timer.Current/_fallTime)*90;
                    m_buildingGameObject.transform.localRotation=Quaternion.Euler(rot);
                }
                break;
            case State.DOWN:
                if(m_timer.IsElapsedOnce)
                {
                    // Start next state
                    m_state = State.RISING;
                    m_timer.Reset(_riseTime);
                }
                break;
            case State.RISING:
                if(m_timer.IsElapsedOnce)
                {
                    // Rotate the building in it's final position
                    Vector3 rot = m_buildingGameObject.transform.localRotation.eulerAngles;
                    rot.z = 0;
                    m_buildingGameObject.transform.localRotation=Quaternion.Euler(rot);

                    // Start next state
                    m_state = State.UP;
                    m_timer.Reset(_upTime);
                }
                else
                {
                    // Rotate the building
                    Vector3 rot = m_buildingGameObject.transform.localRotation.eulerAngles;
                    rot.z = -90 * m_timer.Current / _riseTime;
                    m_buildingGameObject.transform.localRotation=Quaternion.Euler(rot);
                }
                break;
        }
    }

    public void handleCollision(Collision collision)
    {
        if (collision.gameObject.transform.parent.gameObject.GetComponent<Car>() != null)
        {
            Debug.Log("car");
            // Instant kill if it swats the player
            /*if(m_state==State.FALLING)
            {

            }

            // Rise the building
            m_state = State.RISING;
            m_timer.Reset(_riseTime);*/
        }
    }
    #endregion
}

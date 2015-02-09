using UnityEngine;
using System.Collections;

public class ObstacleDestroyer : MonoBehaviour
{
    #region members
    private static float AUTO_DESTRUCTION_TIME = 1f;
    private Timer m_timer;
    #endregion

    #region mono
    void Start()
    {
        m_timer = new Timer();
        m_timer.Reset(AUTO_DESTRUCTION_TIME);
    }
	
	void Update ()
    {
        if (m_timer.IsElapsedOnce) GameObject.Destroy(gameObject);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "InvisibleWall") GameObject.Destroy(gameObject);
    }
    #endregion
}

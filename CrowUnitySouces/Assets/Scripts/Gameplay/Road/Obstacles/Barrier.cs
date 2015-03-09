using UnityEngine;
using System.Collections;


public class Barrier : Obstacle
{

    #region attributes
    const float ACTIVATION_DIST = 100;
    const float DISPLACEMENT = 3;
    const float DISPLACEMENT_TIME = 1;

    private Timer m_timer = null;
    #endregion

    #region methods
    void Start ()
    {
	
	}
	
	void Update ()
    {
        if(m_timer==null)
        {
            if((Car.Instance.transform.Find("Body").position-transform.position).magnitude<ACTIVATION_DIST)
            {
                m_timer = new Timer();
                m_timer.Reset(DISPLACEMENT_TIME);
            }
        }
        else if(!m_timer.IsElapsedLoop)
        {
            transform.position += Time.deltaTime* transform.up * DISPLACEMENT / DISPLACEMENT_TIME;
        }
    }
    #endregion
}

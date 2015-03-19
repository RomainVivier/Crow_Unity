using UnityEngine;
using System.Collections;

public class FragmentDestroyer : MonoBehaviour
{

    #region members_constants
    private const float SPEED = 5;
    private const float ROT_SPEED = 5;
    private const float DISAPPEAR_TIME = 10;

    private Vector3 m_speed;
    private Vector3 m_rotAxis;
    private float m_rotAngle;
    private Timer m_timer;
    #endregion

    #region methods
    public FragmentDestroyer()
    {
		m_speed = new Vector3(Random.Range(-1, 1), Random.Range(-1, 1), Random.Range(-1, 1));
		if (m_speed == Vector3.zero) m_speed = Vector3.one;
		m_speed *= SPEED / m_speed.magnitude;
		if (m_speed.y < 0) m_speed.y = -m_speed.y;
		Quaternion rotSpeed = Quaternion.Euler(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360));
		rotSpeed.ToAngleAxis(out m_rotAngle, out m_rotAxis);
		m_rotAngle = ROT_SPEED;
		m_timer=new Timer();
    }
    
    void Start()
    {
		m_timer.Reset(DISAPPEAR_TIME);	
    }	
    
	public void AddSpeed(Vector3 speed)
	{
		m_speed+=speed;
	}
	
	void FixedUpdate ()
    {
        m_speed.y-=9.81f*Time.fixedDeltaTime;
        transform.position += Time.fixedDeltaTime*m_speed;
        transform.Rotate(m_rotAxis, m_rotAngle * Time.fixedDeltaTime);
        if (m_timer.IsElapsedOnce) GameObject.Destroy(gameObject);
    }
    #endregion
}

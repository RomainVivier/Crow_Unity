using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Score : MonoBehaviour
{
#region members

    public Text _text;

    private float _distanceTraveled = 0f;
    private float _score;
    private float _speed;
    private int _combo = 0;
    private static Score m_instance;

#endregion 

#region Properties

    public float DistanceTravaled
    {
        get { return _distanceTraveled; }
        set { _distanceTraveled = value; }
    }

    public float Speed
    {
        get { return _speed; }
        set { _speed = value; }
    }

    public int Combo
    {
        get { return _combo; }
        set { _combo = value; }
    }

#endregion 

#region Singleton

    public static Score Instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = GameObject.FindObjectOfType<Score>();
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
    }

#endregion

    void Update ()
    {
	    
	}
}

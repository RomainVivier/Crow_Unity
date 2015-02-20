using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Score : MonoBehaviour
{
    #region members

    public Text _text;
    public float _timeToReset = 5f;

    private float m_distanceTraveled = 0f;
    private float m_speed;
    private float m_score;
    private int m_combo = 0;
    private Timer m_comboTimer;
    private bool m_hideScore = false;


    private static Score m_instance;
    private RailsControl m_rc;

    #endregion

    #region Properties

    public float DistanceTravaled
    {
        get
        {
            if (m_rc == null)
            {
                m_rc = GameObject.FindObjectOfType<RailsControl>();
                return m_distanceTraveled;
            }
            else
            {
                return m_distanceTraveled + m_rc.chunk._rails.Dist * m_rc.Progress;
            }
        }
        set { m_distanceTraveled = value; }
    }

    public float Speed
    {
        get { return m_speed; }
        set { m_speed = value; }
    }

    public int Combo
    {
        get { return m_combo; }
        set { m_combo = value; }
    }

    public bool HideScore
    {
        get { return m_hideScore; }
        set
        {
            if (!value)
            {
                _text.text = "";
            }
            m_hideScore = value;
        }
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
        m_comboTimer = new Timer();
    }

    #endregion

    void Update()
    {
        if (!HideScore)
        {
            _text.text = ((int)(DistanceTravaled + m_score)).ToString();
        }
    }

    public void AddScore(int value,int combo=1)
    {
        m_score += m_combo * value;
        m_combo+=combo;
    }

    public void ResetCombo()
    {
        m_combo = 0;
    }

}

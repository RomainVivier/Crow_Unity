using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Score : MonoBehaviour
{
    #region members
    private const float DISPLAY_LAG = 1;
    private const int NB_DIGITS = 7;

    public Text _text;
    public float _timeToReset = 5f;

    private float m_distanceTraveled = 0f;
    private float m_speed;
    private float m_score;
    private float m_displayScore;
    private int m_combo = 0;
    private Timer m_comboTimer;
    private bool m_hideScore = false;
    private float m_augmentSpeed=0;

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
        set { m_distanceTraveled = value-m_rc.chunk._rails.Dist * m_rc.Progress; }
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
        m_displayScore = 0;
    }

    #endregion

    void Update()
    {
        m_displayScore += m_augmentSpeed * Time.deltaTime;
        if (m_displayScore > m_score) m_displayScore = m_score;
        if (!HideScore)
        {
            _text.text = ((int)(DistanceTravaled + m_displayScore)).ToString();
            /*float displayScore=DistanceTravaled+m_displayScore;
            float[] reelAngles=new float[NB_REELS];
            reelAngles[0] = (displayScore % 10)*2*Mathf.PI;
            float pow10 = 1;
            for(int i=1;i<reelAngles;i++)
            {
                pow10 *= 10;

            }*/
        }
    }

    public void AddScore(int value,int combo=1)
    {
        m_score += m_combo * value;
        m_combo+=combo;
        float diff = m_score - m_displayScore;
        m_augmentSpeed = diff / DISPLAY_LAG;
    }

    public void ResetCombo()
    {
        m_combo = 0;
    }

}

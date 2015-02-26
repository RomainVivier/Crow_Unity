using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Score : MonoBehaviour
{
    #region members
    private const float SCORE_DISPLAY_LAG = 1;
    private const float DIST_DISPLAY_LAG = 0.2f;

    private const int NB_DIGITS = 6;

    public Text _text;
    public float _timeToReset = 5f;
    public float _incrementDist = 1;
    public int _incrementValue = 1;
    public int _incrementDistMult = 1;

    private float m_distanceTraveled = 0f;
    private float m_speed;
    private float m_score;
    private float m_distMod;
    private float m_oldDist = 0;
    private float m_displayScore;
    private float m_displayScoreDist;
    private int m_combo = 1;
    private Timer m_comboTimer;
    private bool m_hideScore = false;
    private float m_augmentSpeed=0;
    private float m_addBonus;
    private static Score m_instance;
    private RailsControl m_rc;
    private GameObject[] m_digits;

    private GameObject m_body;
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

    public GameObject Body
    {
        get
        {
            if(m_body == null)
            {
                m_body = GameObject.Find("Body");
            }

            return m_body;
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

    void Start()
    {
        m_digits = new GameObject[NB_DIGITS];
        for(int i=0;i<NB_DIGITS;i++)
        {
            m_digits[i] = GameObject.Find("ScoreDigit"+ i);
            m_digits[i].GetComponent<MeshRenderer>().material.mainTextureScale = new Vector2(1, 0.1f);
        }
    }

    private void Init()
    {
        m_comboTimer = new Timer();
        m_displayScore = 0;
        m_digits = new GameObject[NB_DIGITS];
        m_body = GameObject.Find("Body");
    }

    #endregion

    void Update()
    {
        float diffDist = DistanceTravaled - m_oldDist;

        /*m_score += diffDist * Combo;
        m_displayScore += diffDist * Combo;*/
        m_oldDist = DistanceTravaled;
        m_distMod += diffDist / (_incrementDist * _incrementDistMult);
        if(m_distMod>1)
        {
            int bonus = Mathf.FloorToInt(m_distMod) * _incrementValue * _incrementDistMult*m_combo;
            m_score += bonus;
            m_addBonus += bonus;
            float newAugmentSpeed = m_addBonus / DIST_DISPLAY_LAG;
            if (newAugmentSpeed > m_augmentSpeed) m_augmentSpeed = newAugmentSpeed;
            m_distMod -= Mathf.Floor(m_distMod);
        }

        m_displayScore += m_augmentSpeed * Time.deltaTime;
        m_addBonus -= m_augmentSpeed * Time.deltaTime;
        if (m_displayScore > m_score)
        {
            m_displayScore = m_score;
            m_augmentSpeed = 0;
        }
        if (m_addBonus < 0) m_addBonus = 0;

        if (!HideScore)
        {
            _text.text = "";// ((int)(m_displayScore)).ToString();
            //float[] digitPos=new float[NB_DIGITS];
            //digitPos[0] = (displayScore % 10)*0.1f;
            m_digits[0].GetComponent<MeshRenderer>().material.mainTextureOffset= new Vector2(0, 1-((m_displayScore+1) % 10)*0.1f);
            float pow10 = 1;
            for(int i=1;i<NB_DIGITS;i++)
            {
                pow10 *= 10;
                float digitPos = Mathf.Floor((((m_displayScore) / pow10)+1) % 10);
                if (m_displayScore % pow10 > pow10 - 1) digitPos += m_displayScore % 1;
                //float digitPos = (m_displayScore / pow10) + 0.5f;
                m_digits[i].GetComponent<MeshRenderer>().material.mainTextureOffset = new Vector2(0, 1-digitPos*0.1f);
            }
        }
    }

    public void AddScore(int value,int combo=1)
    {
        m_score += m_combo * value;
        m_combo+=combo;
        float diff = m_score - m_displayScore;
        m_augmentSpeed = diff / SCORE_DISPLAY_LAG;
    }

    public void ResetCombo()
    {
        m_combo = 1;
    }

}

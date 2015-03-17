using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Score : MonoBehaviour
{
    #region members
	public enum ScoreType
	{
		EVENT,MINOR_OBSTACLE,STUFF
	}
    
    private const float SCORE_DISPLAY_LAG = 1;
    private const float DIST_DISPLAY_LAG = 0.2f;
	private const float COMBO_SIZE_FEEDBACK_SPEED=2;
	private const float COMBO_TEN_FEEDBACK_SPEED=1; 
	private const float COMBO_BASE_SCALE=0.65f;
	private const float COMBO_FINAL_SCALE=1.3f;
		
    private const int NB_DIGITS = 6;
	private const int NB_COMBO_DIGITS = 2;
	
    public float _timeToReset = 5f;
    public float _incrementDist = 1;
    public int _incrementValue = 1;
    public int _incrementDistMult = 1;
	public Color[] _comboColors;
	
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

	private float m_comboSizeFeedback=1;
	private float m_comboTenFeedback=1;
	
    private GameObject m_body;

    private GameObject m_comboObject;
    private GameObject m_comboTenFeedbackObject=null;
    
    private GameObject[] m_comboDigits;
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
            m_digits[i] = GameObject.Find("Score_"+ i);
            m_digits[i].GetComponent<MeshRenderer>().material.mainTextureScale = new Vector2(1, 1f);
        }
        m_comboObject=GameObject.Find ("ComboBase");
        m_comboDigits=new GameObject[NB_COMBO_DIGITS];
        for(int i=0;i<NB_COMBO_DIGITS;i++)
        	m_comboDigits[i]=m_comboObject.transform.Find("Digit"+i).gameObject;
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

		m_comboSizeFeedback+=Time.deltaTime*COMBO_SIZE_FEEDBACK_SPEED;
		if(m_comboSizeFeedback>1) m_comboSizeFeedback=1;
		
		if(m_comboTenFeedbackObject!=null) updateTenFeedback();
		
        if (!HideScore)
        {
            //float[] digitPos=new float[NB_DIGITS];
            //digitPos[0] = (displayScore % 10)*0.1f;
            m_digits[0].GetComponent<MeshRenderer>().material.mainTextureOffset= new Vector2(0, 1-((m_displayScore) % 10)*0.1f);
            float pow10 = 1;
            for(int i=1;i<NB_DIGITS;i++)
            {
                pow10 *= 10;
                float digitPos = Mathf.Floor((((m_displayScore) / pow10)) % 10);
                if (m_displayScore % pow10 > pow10 - 1) digitPos += m_displayScore % 1;
                //float digitPos = (m_displayScore / pow10) + 0.5f;
                m_digits[i].GetComponent<MeshRenderer>().material.mainTextureOffset = new Vector2(0, 1-digitPos*0.1f);
            }
            m_comboObject.SetActive(m_combo>1);
            float xScale=Mathf.PingPong(m_comboSizeFeedback*2,1);
            xScale=1+xScale*0.2f;
            m_comboObject.transform.localScale=new Vector3(COMBO_BASE_SCALE*xScale,COMBO_BASE_SCALE,COMBO_BASE_SCALE);
            int combo=m_combo;
            for(int i=0;i<NB_COMBO_DIGITS;i++)
            {
				m_comboDigits[i].GetComponent<MeshRenderer>().material.mainTextureOffset = new Vector2(0, 0.9f-(combo%10)*0.1f);
				m_comboDigits[i].GetComponent<MeshRenderer>().material.color=_comboColors[(m_combo/10)%_comboColors.Length];
            	combo/=10;
            }
        }
        GameInfos.Instance.score=(int)m_score;
        GameInfos.Instance.dist=(int)DistanceTravaled;
        if(m_combo>GameInfos.Instance.maxCombo) GameInfos.Instance.maxCombo=m_combo;
    }

    public void AddScore(ScoreType type, int value,int combo=1)
    {
        m_score += m_combo * value;
        int oldCombo=m_combo;
        m_combo+=combo;
        float diff = m_score - m_displayScore;
        m_augmentSpeed = diff / SCORE_DISPLAY_LAG;
        GameInfos gi=GameInfos.Instance;
        if(combo!=0 && m_comboSizeFeedback>=1) m_comboSizeFeedback=0;
        if(oldCombo/10!=m_combo/10) startTenFeedback(m_combo/10);
        switch(type)
        {
        	case ScoreType.EVENT:
				gi.nbEvents++;
				gi.eventsCombo+=combo;
			break;
			case ScoreType.MINOR_OBSTACLE:
				gi.nbMinorObstacles++;
				gi.minorObstaclesCombo+=combo;
			break;
			case ScoreType.STUFF:
				gi.nbStuff++;
				gi.stuffPoints+=value;
			break;
        }
    }

    public void AddScore(ScoreType type, int value, Vector3 pos, int combo=1)
    {
        AddScore(type, value, combo);
    }

    public void ResetCombo()
    {
        m_combo = 1;
    }

	private void startTenFeedback(int tens)
	{
		// Duplicate combo object
		if(m_comboTenFeedbackObject!=null) GameObject.Destroy(m_comboTenFeedbackObject);
		for(int i=0;i<NB_COMBO_DIGITS;i++)
			m_comboDigits[i].GetComponent<MeshRenderer>().material.color=_comboColors[(m_combo/10)%_comboColors.Length];
		
		m_comboTenFeedbackObject=GameObject.Instantiate(m_comboObject) as GameObject;
		m_comboTenFeedbackObject.transform.SetParent(m_comboObject.transform.parent);
		m_comboTenFeedbackObject.transform.localPosition=m_comboObject.transform.localPosition;
		m_comboTenFeedbackObject.transform.localRotation=m_comboObject.transform.localRotation;

		// Set texture offset to show ten value
		m_comboTenFeedbackObject.transform.Find ("Digit1").GetComponent<MeshRenderer>()
			.material.mainTextureOffset=new Vector2(0, 0.9f-tens*0.1f);	
		m_comboTenFeedbackObject.transform.Find ("Digit0").GetComponent<MeshRenderer>()
			.material.mainTextureOffset=new Vector2(0, 0.9f);
		
		// Init feedback pos
		m_comboTenFeedback=0;	
		
	}
	
	private void updateTenFeedback()
	{		
		// Update feedback value
		m_comboTenFeedback+=Time.deltaTime*COMBO_TEN_FEEDBACK_SPEED;
		
		// Destroy object
		if(m_comboTenFeedback>1)
		{
			m_comboTenFeedback=1;
			GameObject.Destroy(m_comboTenFeedbackObject);
			m_comboTenFeedbackObject=null;
		}
		else // Update alphas
		{
			int nbChildren=m_comboTenFeedbackObject.transform.childCount;
			float scale=Mathf.Lerp (COMBO_BASE_SCALE,COMBO_FINAL_SCALE,m_comboTenFeedback);
			m_comboTenFeedbackObject.transform.localScale=new Vector3(scale,scale,scale);
			for(int i=0;i<nbChildren;i++)
			{
				MeshRenderer mr=m_comboTenFeedbackObject.transform.GetChild(i).GetComponent<MeshRenderer>();
				Color c=mr.material.color;
				c.a=1-m_comboTenFeedback;
				mr.material.color=c;
			}
		}
	}
}

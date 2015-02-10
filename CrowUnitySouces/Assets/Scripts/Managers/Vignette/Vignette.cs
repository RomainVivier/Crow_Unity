using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Vignette : MonoBehaviour
{

    public float _defaultTime = 5;
    public Image _frame;
    public List<int> _railsTaken;
    public Animator _anim;
    

    private VignetteType _type;    
    private Timer m_timer;
    private RectTransform m_rect;
    private CanvasScaler m_cs;

    #region Properties

    public bool isFinished { get; set; }

    #endregion 
    
    void Start()
    {
        m_timer = new Timer();
        isFinished = true;
        m_rect = gameObject.GetComponent<RectTransform>();
        m_cs = GameObject.FindObjectOfType<CanvasScaler>();
        gameObject.SetActive(false);
    }

    void Update()
    {
        if (m_timer.IsElapsedLoop)
        {
            Close();
        }
    }

    public void Pop(VignetteType type, List<int> rails)
    {
        Close();
        isFinished = false;
        gameObject.SetActive(true);
        m_timer.Reset(_defaultTime);
        //TODO set vignette's frame depending of type
        _railsTaken = rails;

        Vector2 rectPos = m_rect.anchoredPosition;
        Vector2 size = m_rect.sizeDelta;

        if (size.x < 0) size.x = m_cs.referenceResolution.x + size.x;
        if (size.y < 0) size.y = m_cs.referenceResolution.y + size.y;

        //TODO better way to set the Size
        switch(rails.Count)
        {
            case 1 :
                m_rect.localScale = Vector3.one;

                if(rails.Contains(0))
                {
                    rectPos.x = size.x / 2 - m_cs.referenceResolution.x / 2;
                }

                if (rails.Contains(1))
                {
                    rectPos.x = 0;
                }

                if (rails.Contains(2))
                {
                    rectPos.x = m_cs.referenceResolution.x / 2 - size.x / 2;
                }
                break;

            case 2 :
                m_rect.localScale = new Vector3(2f,1f,1f);

                if (rails.Contains(0) && rails.Contains(1))
                {
                    rectPos.x = size.x - m_cs.referenceResolution.x / 2;
                }

                if (rails.Contains(1) && rails.Contains(2))
                {
                    rectPos.x = m_cs.referenceResolution.x / 2 - size.x;
                }

                break;
            case 3 :
                m_rect.localScale = new Vector3(3f, 1f, 1f);
                rectPos.x = 0;
                
                break;
        }

        m_rect.anchoredPosition = rectPos;
        //TODO set of animation
    }

    public bool OverlapWith(List<int> otherRails)
    {
        foreach (int i in otherRails)
        {
            if(_railsTaken.Contains(i))
            {
                return true;
            }
        }

        return false;
    }

    public void Close()
    {
        //TODO arrêté l'anim de la vignette plus l'affichage
        gameObject.SetActive(false);
        isFinished = true;
    }


}

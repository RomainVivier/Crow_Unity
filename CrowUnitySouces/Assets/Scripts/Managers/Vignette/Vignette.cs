using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Vignette : MonoBehaviour
{

    public float _defaultTime = 5;
    public Image _frame;
    public List<int> _railsTaken;
    

    private VignetteType _type;    
    private Timer m_timer;
    private RectTransform m_rect;

    #region Properties

    public bool isFinished { get; set; }

    #endregion 
    
    void Start()
    {
        m_timer = new Timer();
        isFinished = true;
        m_rect = gameObject.GetComponent<RectTransform>();
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

        Vector3 rectPos = m_rect.position;

        //TODO better way to set the Size
        switch(rails.Count)
        {
            case 1 :
                m_rect.localScale = Vector3.one;

                if(rails.Contains(0))
                {
                    rectPos.x =  m_rect.rect.width / 2;
                }

                if (rails.Contains(1))
                {
                    rectPos.x = Screen.width / 2;
                }

                if (rails.Contains(2))
                {
                    rectPos.x = Screen.width - m_rect.rect.width / 2;
                }
                break;

            case 2 :
                m_rect.localScale = new Vector3(2f,1f,1f);

                if (rails.Contains(0) && rails.Contains(1))
                {
                    rectPos.x = Screen.width / 3 - m_rect.rect.width / 2;
                }

                if (rails.Contains(1) && rails.Contains(2))
                {
                    rectPos.x = 2 * Screen.width / 3 + m_rect.rect.width / 2;
                }

                break;
            case 3 :
                m_rect.localScale = new Vector3(3f, 1f, 1f);
                rectPos.x = Screen.width / 2;
                
                break;
        }

        m_rect.position = rectPos;
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

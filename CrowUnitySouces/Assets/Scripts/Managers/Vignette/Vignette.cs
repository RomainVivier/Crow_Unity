using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class Vignette : MonoBehaviour
{

    public float _defaultTime = 5;
    public Sprite _frame;
    public List<int> _railsTaken;
    

    private VignetteType _type;    
    private Timer m_timer;

    #region Properties

    public bool isFinished { get; set; }

    #endregion 
    
    void Start()
    {
        m_timer = new Timer();
        isFinished = false;
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
        m_timer.Reset(_defaultTime);
        //set vignette's frame depending of type
    }

    public bool OverlapWith(List<int> otherRails)
    {
        //TODO
        return false;
    }

    public void Close()
    {
        //arrêté l'anim de la vignette plus l'affichage
        isFinished = true;
    }


}

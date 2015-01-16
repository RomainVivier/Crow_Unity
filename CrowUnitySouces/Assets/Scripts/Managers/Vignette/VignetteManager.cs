using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class VignetteManager : Vignette
{
    private Queue<Vignette> m_vignetteUsed;
    private Vignette[] m_vignettes;
    private static VignetteManager m_instance;

    #region Singleton

    public static VignetteManager Instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = GameObject.FindObjectOfType<VignetteManager>();
                if (m_instance == null)
                {
                    GameObject singleton = new GameObject();
                    singleton.name = "VignetteManager";
                    m_instance = singleton.AddComponent<VignetteManager>();
                }
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
        m_vignettes = new Vignette[2];
        m_vignetteUsed = new Queue<Vignette>();
    }

    #endregion

    #region Vignette Functions

    public void Pop(bool isEvent, VignetteType type, List<int> rails)
    {
        Vignette vignetteToPop;

        if(m_vignetteUsed.Count == 2)
        {
            vignetteToPop = m_vignetteUsed.Dequeue();
        }

        //recupérer une vignette non utilisée

        if (m_vignetteUsed.Count == 1)
        {
            Vignette tempVignette = m_vignetteUsed.Dequeue();
            //if(vignetteToPop.OverlapWith(tempVignette._railsTaken))
            //{

            //}
            //else
            //{

            //}

        }
        
    }

    #endregion



}

public enum VignetteType
{
    Front,
    Back,
    Side
}
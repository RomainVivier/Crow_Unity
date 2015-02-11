using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class VignetteManager : MonoBehaviour
{
    public Vignette[] _vignettes;

    private Queue<Vignette> m_vignetteUsed;
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
        //ajouter les deux vignettes
        m_vignetteUsed = new Queue<Vignette>();
    }

    #endregion


    void Update()
    {
        if(Input.GetKeyUp(KeyCode.Alpha1))
        {
            Pop(VignetteType.Front, new List<int>() {0}, "");
        }

        if (Input.GetKeyUp(KeyCode.Alpha2))
        {
            Pop(VignetteType.Front, new List<int>() { 1 }, "");
        }

        if (Input.GetKeyUp(KeyCode.Alpha3))
        {
            Pop(VignetteType.Front, new List<int>() { 2 }, "");
        }

        if (Input.GetKeyUp(KeyCode.Alpha4))
        {
            Pop(VignetteType.Front, new List<int>() { 0, 1 }, "");
        }

        if (Input.GetKeyUp(KeyCode.Alpha5))
        {
            Pop(VignetteType.Front, new List<int>() { 1, 2 }, "");
        }

        if (Input.GetKeyUp(KeyCode.Alpha6))
        {
            Pop(VignetteType.Front, new List<int>() { 0, 1, 2 }, "Cops");

        }
    }


    #region Vignette Functions

    public void Pop(VignetteType type, List<int> rails, string obs)
    {
        Vignette vignetteToPop;

        if (m_vignetteUsed.Count == 2)
        {
            vignetteToPop = m_vignetteUsed.Dequeue();
        }
        else
        {
            vignetteToPop = _vignettes.Where( v => v.isFinished == true ).FirstOrDefault();
        }

        if (m_vignetteUsed.Count == 1)
        {
            Vignette tempVignette = m_vignetteUsed.Dequeue();
            if (tempVignette.OverlapWith(rails))
            {
                tempVignette.Close();
            }
            else
            {
                m_vignetteUsed.Enqueue(tempVignette);
            }
        }

        vignetteToPop.Pop(type, rails);
        if(obs != "") 
            vignetteToPop._anim.SetTrigger(obs);
        m_vignetteUsed.Enqueue(vignetteToPop);

    }

    #endregion



}

public enum VignetteType
{
    Front,
    Back,
    Side
}
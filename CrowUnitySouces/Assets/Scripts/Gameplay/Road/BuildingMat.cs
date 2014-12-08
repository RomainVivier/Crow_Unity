using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BuildingMat
{
    private string m_buildingName;
    public ProceduralMaterial m_baseMat;
    public ProceduralMaterial m_middleMat;
    public ProceduralMaterial m_topMat;
    public Dictionary<float, Texture> m_base = new Dictionary<float, Texture>();
    public Dictionary<float, Texture> m_middle = new Dictionary<float, Texture>();
    public Dictionary<float, Texture> m_top = new Dictionary<float, Texture>();

    public string Name
    {
        get{ return m_buildingName; }
        set{ m_buildingName = value; }
    }

    public ProceduralMaterial BaseMat
    {
        get { return m_baseMat; }
        set { m_baseMat = value; }
    }

    public ProceduralMaterial MiddleMat
    {
        get { return m_middleMat; }
        set { m_middleMat = value; }
    }

    public ProceduralMaterial TopMat
    {
        get { return m_topMat; }
        set { m_topMat = value; }
    }

    public Texture Base (float color)
    {
        if (m_base.ContainsKey(color))
            return m_base[color];
        else
            return null;
    }

    public Texture Middle (float color)
    {
        if (m_middle.ContainsKey(color))
            return m_middle[color];
        else
            return null;
    }

    public Texture Top (float color)
    {
        if (m_top.ContainsKey(color))
            return m_top[color];
        else
            return null;
    }

    public void GenerateTextures(float color)
    {
        if(m_baseMat != null)
        {
            m_baseMat.SetProceduralFloat("Wall_Color", color);
            m_baseMat.RebuildTextures();
            m_base.Add(color, m_baseMat.GetGeneratedTextures()[0]);
        }
        
        if(m_middleMat != null)
        {
            m_middleMat.SetProceduralFloat("Wall_Color", color);
            m_middleMat.RebuildTextures();
            m_middle.Add(color, m_middleMat.GetGeneratedTextures()[0]);
        }
        
        if(m_topMat != null)
        {
            m_topMat.SetProceduralFloat("Wall_Color", color);
            m_topMat.RebuildTextures();
            m_top.Add(color, m_topMat.GetGeneratedTextures()[0]);
        }
    }

}

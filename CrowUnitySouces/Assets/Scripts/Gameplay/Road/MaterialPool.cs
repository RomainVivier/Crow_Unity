using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class MaterialPool : MonoBehaviour {
    #region members

    //public int _colorNb;
    /// <summary>
    /// Key = id
    /// Value = Materials to generate
    /// </summary>
    public List<ProceduralMaterial> _materials = new List<ProceduralMaterial>();
    
    #endregion

    #region MonoBehaviour

    void Awake()
    {
        //AllocateObjects();
        foreach(ProceduralMaterial pm in _materials)
        {
            pm.RebuildTextures();
        }
    }

    #endregion

    #region PoolFunction

    string GetBuildingMatName(string matName)
    {
        string name = "";

        if(matName.Contains("_Base"))
        {
            name = matName.Substring(0, matName.IndexOf("_Base", 0));
        }
    
        if(matName.Contains("_Middle"))
        {
            name = matName.Substring(0, matName.IndexOf("_Middle", 0));
        }

        if(matName.Contains("_Top"))
        {
            name = matName.Substring(0, matName.IndexOf("_Top", 0));
        }

        return name;
    }

    //BuildingMat GetBuildingMat(string name)
    //{
    //    if (name == "")
    //    {
    //        Debug.LogError("Name is null");
    //        return null;
    //    }

    //    BuildingMat mat = m_buildmats.Where(bm => bm.Name == name).FirstOrDefault();
    //    if (mat == null)
    //    {
    //        mat = new BuildingMat();
    //        mat.Name = name;
    //        m_buildmats.Add(mat);
    //    }

    //    return mat;
    //}

    //void AllocateObjects()
    //{

    //    if (_materials.Count == 0)
    //    {
    //        Debug.LogError("there is no materials to load !");
    //        return;
    //    }

    //    foreach (ProceduralMaterial mat in _materials)
    //    {
    //        var buildMat = GetBuildingMat(GetBuildingMatName(mat.name));
            
    //        if (mat.name.Contains("_Base"))
    //        {
    //            buildMat.BaseMat = mat;
    //        }

    //        if (mat.name.Contains("_Middle"))
    //        {
    //            buildMat.MiddleMat = mat;
    //        }

    //        if (mat.name.Contains("_Top"))
    //        {
    //            buildMat.TopMat = mat;
    //        }
    //    }

    //    for (int i = 0; i < 11; i++)
    //    {
    //        foreach (BuildingMat bm in m_buildmats)
    //        {
    //            bm.GenerateTextures((float)(i) / 10f);
    //        }
    //    }

    //    foreach (BuildingMat bm in m_buildmats)
    //    {
    //        Debug.Log(bm.Name + " base count : " + (bm.m_base.Count) + " middle count : " + (bm.m_middle.Count) + " top count : " + (bm.m_top.Count));
    //    }
    //}

    public Material GetRandomMaterial(string matName, int color)
    {
        string name = GetBuildingMatName(matName);
        Debug.Log("color = " +color );

        if (matName.Contains("_Base"))
        {
            foreach(ProceduralMaterial pm in _materials)
            {
                if (pm.name == name + "_Base" + "_" + color.ToString())
                {
                    return (Material)pm;
                }
            }
        }

        if (matName.Contains("_Middle"))
        {
            foreach (ProceduralMaterial pm in _materials)
            {
                if (pm.name == name + "_Middle" + "_" + color.ToString())
                {
                    return (Material)pm;
                }
            }
        }

        if (matName.Contains("_Top"))
        {
            foreach (ProceduralMaterial pm in _materials)
            {
                if (pm.name == name + "_Top" + "_" + color.ToString())
                {
                    return (Material)pm;
                }
            }
        }


        return null;
    }

    #endregion
}

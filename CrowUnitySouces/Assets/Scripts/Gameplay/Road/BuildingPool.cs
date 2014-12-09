using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class BuildingPool : MonoBehaviour {

    #region members

    public int _nbOfGO;
    public Object[] m_buildings;

    private Dictionary<string, List<PoolableObject>> m_pool = new Dictionary<string, List<PoolableObject>>();
    private static BuildingPool m_instance;
    #endregion

    #region Singleton

    public static BuildingPool Instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = GameObject.FindObjectOfType<BuildingPool>();
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
        AllocateObjects();
    }

    #endregion

    #region PoolFunction

    void AllocateObjects()
    {
        m_buildings = Resources.LoadAll("Buildings");

        if (_nbOfGO == 0)
        {
            Debug.LogError("there is no purpose in loading 0 building !");
            return;
        }

        foreach (Object go in m_buildings)
        {
            List<PoolableObject> tempList = new List<PoolableObject>();
            for (int i = 0; i < _nbOfGO; i++)
            {
                GameObject tempObject = GameObject.Instantiate(go) as GameObject;
                tempObject.gameObject.SetActive(false);
                tempObject.name = go.name;
                PoolableObject tempPO = tempObject.GetComponent<PoolableObject>(); 
                tempPO.IsPoolable = true;
                tempList.Add(tempPO);
            }

            m_pool.Add(go.name, tempList);
        }
    }

    void AllocateObjectNamed(string name)
    {
        GameObject poolObject = m_buildings.Where(po => po.name == name).FirstOrDefault() as GameObject;

        if (poolObject)
        {
            for (int i = 0; i < _nbOfGO; i++)
            {
                GameObject tempObject = GameObject.Instantiate(poolObject) as GameObject;
                tempObject.gameObject.SetActive(false);
                tempObject.name = poolObject.name;
                PoolableObject tempPO = tempObject.GetComponent<PoolableObject>();
                tempPO.IsPoolable = true;
                m_pool[name].Add(tempPO);
            }
        }
    }

    public GameObject GetUnusedBuilding(string name)
    {
        PoolableObject tempObject = m_pool[name].Where(po => po.IsPoolable == true).FirstOrDefault();

        if (tempObject)
        {
            tempObject.IsPoolable = false;
            return tempObject.gameObject;
        }
        else
        {
            AllocateObjectNamed(name);
            tempObject = m_pool[name].Where(po => po.IsPoolable == true).FirstOrDefault();
            tempObject.IsPoolable = false;
            return tempObject.gameObject;
        }

    }

    #endregion
}

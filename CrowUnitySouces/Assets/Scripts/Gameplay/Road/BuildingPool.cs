using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class BuildingPool : MonoBehaviour {

    #region members

    public int _nbOfGO;
    public List<GameObject> m_buidings = new List<GameObject>();

    private Dictionary<string, List<PoolableObject>> m_pool = new Dictionary<string, List<PoolableObject>>();
    #endregion

    #region MonoBehaviour

    void Awake()
    {
        AllocateObjects();
    }

    #endregion

    #region PoolFunction

    void AllocateObjects()
    {
        if (_nbOfGO == 0)
        {
            Debug.LogError("there is no purpose in loading 0 building !");
            return;
        }

        Debug.Log(m_buidings.Count);
        foreach (GameObject po in m_buidings)
        {
            List<PoolableObject> tempList = new List<PoolableObject>();
            for (int i = 0; i < _nbOfGO; i++)
            {
                GameObject tempObject = GameObject.Instantiate(po) as GameObject;
                tempObject.gameObject.SetActive(false);
                tempObject.name = po.name;
                PoolableObject tempPO = tempObject.GetComponent<PoolableObject>(); 
                tempPO.IsPoolable = true;
                tempList.Add(tempPO);
            }

            m_pool.Add(po.name, tempList);
        }
    }

    void AllocateObjectNamed(string name)
    {
        GameObject poolObject = m_buidings.Where(po => po.gameObject.name == name).FirstOrDefault();

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

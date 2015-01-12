using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class PoolManager : MonoBehaviour {

    [System.Serializable]
    public class StackObject : Stack<PoolableObject>
    { }

    [System.Serializable]
    public class PoolKeyMap : Utils.DictionaryWrapper<PoolKey,StackObject>
    {
    }

    #region members

    public PoolKeyMap _pool = new PoolKeyMap();

    private static PoolManager m_instance;

    #endregion


    #region Singleton

    public static PoolManager Instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = GameObject.FindObjectOfType<PoolManager>();
                if(m_instance == null)
                {
                    GameObject singleton = new GameObject();
                    singleton.name = "GenericPool";
                    m_instance = singleton.AddComponent<PoolManager>();
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
        AllocateObjects();
    }

    #endregion

    #region PoolFunction

    void AllocateObjects()
    {
        if (_pool.Dictionary.Count == 0)
        {
            Debug.LogError("there is no objects to load !");
            return;
        }

        foreach (PoolKey pk in _pool.Dictionary.Keys)
        {
            pk.Id = pk.Object.name;

            //for (int i = 0; i < pk.Number; i++)
            //{
            //    GameObject tempObject = GameObject.Instantiate(pk.Object) as GameObject;
            //    tempObject.SetActive(false);
            //    tempObject.name = pk.Id;
            //    PoolableObject tempPO = tempObject.GetComponent<PoolableObject>();
            //    if (tempPO == null)
            //    {
            //        Debug.LogError("GameObject named : " + pk.Id + ", has no PoolableObject component.");
            //        return;
            //    }
            //    tempPO.IsPoolable = true;
            //    _pool.Dictionary[pk].Add(tempPO);
            //}
        }
    }

    PoolableObject AllocateObjectsByID(PoolKey pk)
    {

        PoolKey poolKey = _pool.Dictionary.Where(pt => pt.Key == pk).FirstOrDefault().Key;

        if (poolKey == null)
        {
            Debug.LogError(poolKey.Id + "not found.");
            return null;
        }

        GameObject tempObject = GameObject.Instantiate(poolKey.Object) as GameObject;
        tempObject.SetActive(false);
        tempObject.name = poolKey.Id;
        PoolableObject tempPO = tempObject.GetComponent<PoolableObject>();
        return tempPO;


        //for (int i = 0; i < poolType.Key.Number; i++)
        //{
        //    GameObject tempObject = GameObject.Instantiate(poolType.Key.Object) as GameObject;
        //    tempObject.SetActive(false);
        //    tempObject.name = poolType.Key.Id;
        //    PoolableObject tempPO = tempObject.GetComponent<PoolableObject>();
        //    if (tempPO == null)
        //    {
        //        Debug.LogError("GameObject named : " + poolType.Key.Id + ", has no PoolableObject component.");
        //        return;
        //    }
        //    tempPO.IsPoolable = true;
        //    _pool.Dictionary[poolType.Key].Add(tempPO);
        //}
    }

    public GameObject GetUnusedObject(string id)
    {
        var poolType = _pool.Dictionary.Where(pt => pt.Key.Id == id).FirstOrDefault();

        if (poolType.Key == null)
        {
            Debug.LogError(poolType.Key.Id + "not found.");
            return null;
        }

        if (_pool.Dictionary[poolType.Key].Count > 0)
        {
            PoolableObject tempPO = _pool.Dictionary[poolType.Key].Pop();

            tempPO.Reset();

            return tempPO.gameObject;
        }
        else
        {
            return AllocateObjectsByID(poolType.Key).gameObject;

        }


        //PoolableObject tempPO = poolType.Value.Where(po => po.IsPoolable == true).FirstOrDefault();

        //if (tempPO)
        //{
        //    tempPO.IsPoolable = false;
        //    return tempPO.gameObject;
        //}
        //else
        //{
        //    AllocateObjectsByID(poolType.Key.Id);
        //    tempPO = poolType.Value.Where(po => po.IsPoolable == true).FirstOrDefault();
        //    tempPO.IsPoolable = false;
        //    return tempPO.gameObject;
        //}

    }

    #endregion
}

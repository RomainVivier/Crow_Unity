using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class ChunkPool : MonoBehaviour {

    #region members

    private List<RoadChunk> m_chunks = new List<RoadChunk>();
    //	private int m_index = 0;

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
        Object[] objects = Resources.LoadAll("Chunks");

        if (objects.Length == 0)
        {
            Debug.LogError("there is no chunk to load !");
            return;
        }

        for (int i = 0; i < objects.Length; i++)
        {
            GameObject tempObject = GameObject.Instantiate(objects[i]) as GameObject;
            tempObject.SetActive(false);
            tempObject.name = objects[i].name;
            RoadChunk tempChunk = tempObject.GetComponent<RoadChunk>();
            tempChunk.IsUnused = true;
            m_chunks.Add(tempChunk);
        }
    }

    public GameObject GetUnusedChunk()
    {
        RoadChunk tempChunk = m_chunks.Where(po => po.IsUnused == true).FirstOrDefault();

        if (tempChunk)
        {
            tempChunk.IsUnused = false;
            return tempChunk.gameObject;
        }
        else
        {
            AllocateObjects();
            tempChunk = m_chunks.Where(po => po.IsUnused == true).FirstOrDefault();
            tempChunk.IsUnused = false;
            return tempChunk.gameObject;
        }

    }

    #endregion
}

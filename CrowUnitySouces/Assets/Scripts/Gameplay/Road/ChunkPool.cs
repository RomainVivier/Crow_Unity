using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class ChunkPool : MonoBehaviour {

    #region members

    private List<RoadChunk> m_chunks = new List<RoadChunk>();
    private int[] m_history;

    public List<string> _usedChunks = new List<string>();
    public bool _historyRandomizer=false;
    public int _historySize = 6;
    public int _nbRetries = 6;

    private const int POOL_SIZE = 50;

    #endregion

    #region MonoBehaviour

    void Awake()
    {
        m_history = new int[_historySize];
        for (int i = 0; i < _historySize; i++) m_history[i] = Random.Range(0, _usedChunks.Count - 1);
        allocateObjects();
    }

    private void allocateObjects()
    {
        if (_usedChunks.Count == 0) AllocateDefaultObjects(); else allocateSetChunks(_usedChunks);
    }
    #endregion

    #region PoolFunction

    void AllocateDefaultObjects()
    {
        allocateObjects(Resources.LoadAll("Chunks"));
    }
    
    private void allocateObjects(Object[] objects)
    {
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

    private void unloadChunks()
    {
        foreach(RoadChunk rc in m_chunks)
        {
            GameObject tempObject = rc.gameObject;
            GameObject.Destroy(tempObject);
        }
        m_chunks.Clear();
        Resources.UnloadUnusedAssets();
    }

    private void allocateSetChunks(List<string> newChunks)
    {
        List<Object> objects=new List<Object>();
        if(_historyRandomizer)
        {
            for(int i=0;i<POOL_SIZE;i++)
            {
                int nbTries = 0;
                int nextChunk=0;
                do
                {
                    nextChunk = Random.Range(0, newChunks.Count);
                } while (nbTries < _nbRetries && m_history.Contains(nextChunk));
                for (int j = 0; j < _historySize - 1; j++) m_history[j] = m_history[j + 1];
                m_history[_historySize - 1] = nextChunk;
                objects.Add(Resources.Load("Chunks/"+newChunks[nextChunk]));
            }
        }
        else foreach(string s in  newChunks)
        {
            objects.Add(Resources.Load("Chunks/"+s));
        }
        allocateObjects(objects.ToArray());
    }
    public void SetChunckList(List<string> newChunks)
    {
        unloadChunks();
        allocateSetChunks(newChunks);
    }

    public GameObject GetUnusedChunk()
    {
        RoadChunk tempChunk;
        if(Application.loadedLevelName=="Endless") tempChunk = m_chunks.Where(po => po.IsUnused == true).OrderBy(a => Random.Range(0f,1f)).FirstOrDefault();
		else  m_chunks.Where(po => po.IsUnused == true).FirstOrDefault();

        if (tempChunk)
        {
            tempChunk.IsUnused = false;
            return tempChunk.gameObject;
        }
        else
        {
            allocateObjects();
            tempChunk = m_chunks.Where(po => po.IsUnused == true).FirstOrDefault();
            tempChunk.IsUnused = false;
            return tempChunk.gameObject;
        }
    }

    #endregion
}

using UnityEngine;
using System.Collections;

public class PrefabSpawner : MonoBehaviour
{
    #region members
    public GameObject prefab;
    private Vector3 m_pos;
    private GameObject m_spawnedObject = null;
    #endregion

    #region methods
    void Start ()
    {
        m_pos = new Vector3(float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity);
	}
	
	void Update ()
    {
        Vector3 pos = transform.position;
        if(pos!=m_pos)
        {
            m_pos = pos;
            spawnPrefab();
        }
    }

    private void spawnPrefab()
    {
        if (m_spawnedObject != null) GameObject.Destroy(m_spawnedObject);
        m_spawnedObject=(GameObject) GameObject.Instantiate(prefab, transform.position, transform.rotation);
    }
    
    void OnDrawGizmos()
    {

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, 1);
    }
    #endregion
}

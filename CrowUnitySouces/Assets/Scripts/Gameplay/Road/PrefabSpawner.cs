using UnityEngine;
using System.Collections;

public class PrefabSpawner : MonoBehaviour
{
    #region members
    public GameObject prefab;
    public Rails _rail;
    public float _railIndex;
    public float _railProggress;


    private Vector3 m_pos;
    private GameObject m_spawnedObject = null;
    #endregion

    #region methods

    public void spawnPrefab()
    {
        if (m_spawnedObject != null) 
			GameObject.Destroy(m_spawnedObject);
		m_spawnedObject=(GameObject) GameObject.Instantiate(prefab, transform.position, transform.rotation);
		m_spawnedObject.transform.parent = transform;
		Debug.Log("Spawned Object at " + m_spawnedObject.transform.position.ToString());
    }
    
    void OnDrawGizmos()
    {

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, 1);
    }

    public virtual void OnValidate()
    {
        if (_rail != null)
        {
            transform.position = _rail.getPoint(_railIndex, _railProggress);
        }
    }

    #endregion
}

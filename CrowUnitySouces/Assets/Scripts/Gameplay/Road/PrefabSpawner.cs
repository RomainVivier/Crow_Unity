using UnityEngine;
using System.Collections;

public class PrefabSpawner : MonoBehaviour
{
    #region members
    public GameObject prefab;
    public Vector3 _offset = new Vector3(0, 1, 0);
    public Vector3 _scale = new Vector3(1, 1, 1);
    public Rails _rail;
    public float _railIndex;
    public float _railProgress;


    private Vector3 m_pos;
    private GameObject m_spawnedObject = null;
    #endregion

    #region methods

    public void spawnPrefab()
    {
        if (m_spawnedObject != null) GameObject.Destroy(m_spawnedObject);
        m_spawnedObject=(GameObject) GameObject.Instantiate(prefab, transform.position + _offset, transform.rotation);
        Obstacle obstacle = m_spawnedObject.GetComponent<Obstacle>();

        if(obstacle != null)
        {
            obstacle.Rails = _rail;
            obstacle.RailsIndex = _railIndex;
            obstacle.RailsProgress = _railProgress;

			if(_rail != null)
				m_spawnedObject.transform.position = _rail.getPoint(_railIndex, _railProgress)+ _offset;

            m_spawnedObject.transform.localScale = _scale;
		}
		
        m_spawnedObject.transform.parent = transform;
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
            transform.position = _rail.getPoint(_railIndex, _railProgress);
        }
    }

    #endregion
}

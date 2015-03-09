using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class FakeCooldown : MonoBehaviour {

	public float _lengthInSeconds = 10;
	float m_timer;
	Transform m_child;
	Vector3 m_childScale = Vector3.zero;
	
	void Start () 
	{
		Destroy (gameObject, _lengthInSeconds);
		m_child = transform.GetChild (0);
	}

	void Update () 
	{
		m_timer += Time.deltaTime;
		m_childScale.x = m_childScale.y = m_childScale.z = m_timer / _lengthInSeconds;
		m_child.transform.localScale = m_childScale;
	}
}

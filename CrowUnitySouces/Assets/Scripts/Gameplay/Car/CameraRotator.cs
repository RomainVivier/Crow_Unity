using UnityEngine;
using System.Collections;

public class CameraRotator : MonoBehaviour
{
	#region attributes
	public bool _isLeft=true;
	
	private Collider m_carCollider=null;
	private GameObject m_carCamera=null;
	#endregion

	#region methods
	void Start ()
	{
	
	}
	
	void Update ()
	{
	}
	
	void OnTriggerEnter(Collider other)
	{
		if(m_carCollider==null)
		{
			GameObject oth = other.gameObject;
			Car car = oth.transform.root.GetComponent<Car>();
			if(car!=null)
			{
				m_carCollider=other;
				m_carCamera=car.transform.FindChild("Body/CameraRoot/CameraDashboard/CameraEnvironment").gameObject;
				m_carCamera.GetComponent<Animator>().SetBool(_isLeft ? "RightSidewalk" : "LeftSidewalk",true);
			}
		}
	}
	
	void OnTriggerExit(Collider other)
	{
		if(m_carCollider!=null && other==m_carCollider)
		{
			m_carCollider=null;
			m_carCamera.GetComponent<Animator>().SetBool(_isLeft ? "RightSidewalk" : "LeftSidewalk",false);
		}
	}

	#endregion
}

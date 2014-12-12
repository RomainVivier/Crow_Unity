using UnityEngine;
using System.Collections;

[System.Serializable]
public abstract class PoolableObject : MonoBehaviour 
{

	#region members

	private bool m_isPoolable = true;

	#endregion

	#region Properties

	public bool IsPoolable
	{
		get { return m_isPoolable; }
		set
		{ 
			if(m_isPoolable == value)
				return;

			m_isPoolable = value;

			if(m_isPoolable) 
				OnPoolClear();
			else 
				OnPoolInit();
		}
	}

	#endregion



	#region Override Interface

	/// <summary>
	/// Called whenever the object is picked up in the pool.
	/// This should be used for activating stuff.
	/// </summary>
	public virtual void OnPoolInit(){}

	/// <summary>
	/// Called whenever the object is 
	/// This should be used for deactivating stuff.
	/// </summary>
	public virtual void OnPoolClear(){}


	#endregion
}

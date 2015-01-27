using UnityEngine;
using System.Collections;

[System.Serializable]
public abstract class PoolableObject : MonoBehaviour 
{

	#region Override Interface

	/// <summary>
	/// Called whenever the object is picked up in the pool.
	/// This should be used for activating stuff.
	/// </summary>
	public virtual void Init(){}

	/// <summary>
	/// Called whenever the object is reset
	/// This should be used for deactivating stuff.
	/// </summary>
	public virtual void Reset()
    {
        //TODO register to the PoolManager stack again
    }



	#endregion
}

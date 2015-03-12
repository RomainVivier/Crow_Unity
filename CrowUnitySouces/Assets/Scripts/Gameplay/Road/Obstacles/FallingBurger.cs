using UnityEngine;
using System.Collections;

public class FallingBurger : Obstacle
{
    public Transform _burgerRoot;
    public GameObject _prefabBurger;
    public Animator _anim;
	public bool _instantiateBurger = true;

    public override void Activate()
    {
        base.Activate();
		if(_instantiateBurger)
		{
	        GameObject burger = GameObject.Instantiate(_prefabBurger, _burgerRoot.position, Quaternion.Euler(Vector3.right * (-90))) as GameObject;
	        burger.transform.parent = _burgerRoot;
	        burger.transform.localScale = new Vector3(0.566f, 0.7533f, 0.028f);
		}
		_anim.SetTrigger("Engage");
        m_state = State.Activated;
    }

}

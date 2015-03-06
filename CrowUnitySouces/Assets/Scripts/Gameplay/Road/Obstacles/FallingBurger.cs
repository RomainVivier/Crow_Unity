using UnityEngine;
using System.Collections;

public class FallingBurger : Obstacle
{
    public Transform _burgerRoot;
    public GameObject _prefabBurger;
    public Animator _anim;

    //public override void Update()
    //{
    //    if (Score.Instance.Body.transform.position.x > transform.position.x)
    //    {
    //        m_activated = false;
    //    }

    //    base.Update();
    //}

    public override void Activate()
    {
        base.Activate();
        //if (_anim.GetCurrentAnimationClipState(0)[0].clip.name != "Idle")
        //{
        //    _anim.SetTrigger("Reset"); 
        //}
        GameObject burger = GameObject.Instantiate(_prefabBurger, _burgerRoot.position, Quaternion.Euler(Vector3.right * (-90))) as GameObject;
        burger.transform.parent = _burgerRoot;
        burger.transform.localScale = new Vector3(0.566f, 0.7533f, 0.028f);
        _anim.SetTrigger("Engage");
        m_state = State.Activated;
    }

}

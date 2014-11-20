using UnityEngine;
using System.Collections;

public class ProtoController : MonoBehaviour {

    public GameObject _motherPastule;
    public Animator _mpAnimator;

    public GameObject _rocket;
    public Animator _rocketAnimator;

    public void ActivateMP()
    {
        if(_motherPastule == null)
        {
            Debug.Log("MP can't be null");
            return;
        }

        _motherPastule.SetActive(true);
    }

    public void DeactivateMP()
    {
        if (_motherPastule == null)
        {
            Debug.Log("MP can't be null");
            return;
        }

        _motherPastule.SetActive(false);
    }

    public void ActivateRocket()
    {
        if (_rocket == null)
        {
            Debug.Log("rocket can't be null");
            return;
        }

        _rocket.SetActive(true);
    }

    public void DeactivateRocket()
    {
        if (_rocket == null)
        {
            Debug.Log("rocket can't be null");
            return;
        }

        _rocket.SetActive(false);
    }

    public void PlayMP()
    {

        if (_mpAnimator == null)
        {
            Debug.Log("MP animator can't be null");
            return;
        }

        _mpAnimator.SetTrigger("Play");
    }

    public void PlayRocket()
    {

        if (_rocketAnimator == null)
        {
            Debug.Log("MP animator can't be null");
            return;
        }

        _rocketAnimator.SetTrigger("Play");
    }
}

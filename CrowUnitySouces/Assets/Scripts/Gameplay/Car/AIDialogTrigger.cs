using UnityEngine;
using System.Collections;

public class AIDialogTrigger : MonoBehaviour
{
    #region members
    public bool _onlyOnce = true;
    public string _sound = "";
    private bool m_alreadyPlayed=false;
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
        if(other.gameObject.name=="Body" && other.gameObject.transform.parent.GetComponent<Car>()!=null
            && (!_onlyOnce || !m_alreadyPlayed))
        {
            m_alreadyPlayed = true;
            (GameObject.FindObjectOfType<AI>() as AI).playDialog(_sound);
        }
    }

    #endregion
}

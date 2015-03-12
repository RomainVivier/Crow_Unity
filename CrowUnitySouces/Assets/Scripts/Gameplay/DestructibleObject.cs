using UnityEngine;
using System.Collections;

public class DestructibleObject : MonoBehaviour
{
    public int _bonus=100;
    public int _comboBonus=1;

    #region methods
    void Start ()
    {
	
	}
	
	void Update ()
    {

    }
    
    void OnTriggerEnter(Collider other)
    {
        GameObject oth = other.gameObject;
        Car car = oth.transform.root.GetComponent<Car>();
        if(car!=null)
        {
            int nbChild=transform.childCount;
            for(int i=0;i<nbChild;i++)
            {
                GameObject go = transform.GetChild(i).gameObject;
                go.AddComponent<FragmentDestroyer>();
            }
            Score.Instance.AddScore(Score.ScoreType.STUFF,_bonus, transform.position, _comboBonus);
        }
    }
    #endregion

}

using UnityEngine;
using System.Collections;

public class DestructibleObject : MonoBehaviour
{
    public int _bonus=100;
    public int _comboBonus=1;
	public ParticleSystem _particles;

	private bool m_hasExploded=false;
    #region methods
    void Start ()
    {
	
	}
	
	void Update ()
    {

    }
    
    void OnTriggerEnter(Collider other)
    {
    	if(m_hasExploded) return;
        GameObject oth = other.gameObject;
        Car car = oth.transform.root.GetComponent<Car>();
        if(car!=null)
        {
            int nbChild=transform.childCount;
            for(int i=0;i<nbChild;i++)
            {
                GameObject go = transform.GetChild(i).gameObject;
                FragmentDestroyer fd=go.AddComponent<FragmentDestroyer>();
                fd.AddSpeed(car.getForwardVector()*car.getForwardVelocity()*1f);
            }
            _particles.gameObject.SetActive(true);
            _particles.Play();
            m_hasExploded=true;
            Score.Instance.AddScore(Score.ScoreType.STUFF,_bonus, transform.position, _comboBonus);
        }
    }
    #endregion

}

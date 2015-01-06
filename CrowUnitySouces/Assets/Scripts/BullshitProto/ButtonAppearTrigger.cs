using UnityEngine;
using System.Collections;

public class ButtonAppearTrigger : MonoBehaviour {

	public Animator[] m_buttonsToHide;
	public float m_delayPerButton = 0.1f;
	bool m_initFlag = false;

	// Use this for initialization
	void Start () {
		for(int i = 0; i < m_buttonsToHide.Length; ++i)
			m_buttonsToHide[i].gameObject.SetActive(false);
	}

	void OnTriggerEnter(Collider other)
	{
		if(other.name != "Body" || m_initFlag)
			return;
		for(int i = 0; i < m_buttonsToHide.Length; ++i)
			StartCoroutine(DelayShow(m_buttonsToHide[i], i* m_delayPerButton));
		m_initFlag = true;
	}

	IEnumerator DelayShow(Animator button, float delayInSeconds)
	{
		yield return new WaitForSeconds(delayInSeconds);
		button.gameObject.SetActive(true);
		button.SetTrigger("Appear");
	}
}

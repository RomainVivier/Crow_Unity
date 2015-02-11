using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class FinalScreenController : MonoBehaviour {

	static FinalScreenController _instance;

	public static FinalScreenController Instance
	{
		get{
			if(_instance == null)
				_instance = GameObject.FindObjectOfType<FinalScreenController>();
			return _instance;
		}
	}

	float _blackoutDelayInSeconds = 1.5f;
	float _logoDelayInSeconds = 3.5f;

	public void Show()
	{
		Invoke ("ShowPanel", _blackoutDelayInSeconds);
		Invoke ("ShowLogo", _logoDelayInSeconds);
	}
	
	void ShowPanel()
	{
		GetComponent<Image>().enabled = true;
	}

	void ShowLogo()
	{
		GetComponentsInChildren<Image>()[1].enabled = true;
	}
}

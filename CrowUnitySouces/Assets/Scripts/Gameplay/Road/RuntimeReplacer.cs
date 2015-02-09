using UnityEngine;
using System.Collections;

public class RuntimeReplacer : MonoBehaviour {

	public GameObject _objectToReplace;
	public GameObject _replacement;

	// Use this for initialization
	void Start () {

		if(_objectToReplace == null)
			_objectToReplace = gameObject;

		GameObject instancedReplacement = Instantiate(_replacement) as GameObject;
		instancedReplacement.transform.parent = _objectToReplace.transform.parent;
		instancedReplacement.transform.localPosition = _objectToReplace.transform.localPosition;
		instancedReplacement.transform.localRotation = _objectToReplace.transform.localRotation;
		instancedReplacement.transform.localScale = _objectToReplace.transform.localScale;
		Destroy(_objectToReplace);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}

using UnityEngine;
using System.Collections;

public class BuildingGenerator : MonoBehaviour {

    public int _minHeight;
    public int _maxHeight;

    public string _enviro;

	void Start()
    {
	    if(_minHeight < 1 || (_maxHeight < 1 || _maxHeight < _minHeight) || _enviro=="")
        {
            Debug.Log("Some parameters are wrong. Correct it and try again.");
            return;
        }
        int height = Random.Range(_minHeight, _maxHeight);
        BuildingGeneratorParameters.RandomBuilding rb = GameObject.Find(_enviro)//(Resources.Load("Enviros/"+_enviro) as GameObject)
                    .GetComponent<BuildingGeneratorParameters>()
                    .getRandomBuilding(height);

        GameObject building;

        for (int i = 0; i < height ; i++)
        {
            if(i==0) building = PoolManager.Instance.GetUnusedObject(rb.baseObject.name);
            else if(i==height-1) building=PoolManager.Instance.GetUnusedObject(rb.topObject.name);
            else building=PoolManager.Instance.GetUnusedObject(rb.middleObject.name);
            building.GetComponent<MeshRenderer>().material = rb.material;
            building.SetActive(true);
            building.transform.position = transform.position + Vector3.up * 30 * i * transform.localScale.y;
            building.transform.rotation = Quaternion.Euler(new Vector3(-90, 90 + transform.rotation.eulerAngles.y, 0));
            building.transform.localScale = Vector3.Scale(building.transform.localScale, transform.localScale);
            building.transform.parent = transform;
        }
	}


    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(transform.position + Vector3.up * (_maxHeight * transform.localScale.y * 30 / 2), Vector3.Scale(new Vector3(50, 30 * _maxHeight, 50), transform.localScale));
    }
}

using UnityEngine;
using System.Collections;

public class BuildingGenerator : MonoBehaviour {

    public int _minHeight;
    public int _maxHeight;
    public string _theme;

	void Start()
    {
	    if(_minHeight < 1 || (_maxHeight < 1 || _maxHeight < _minHeight) || _theme == "")
        {
            Debug.Log("Some parameters are wrong. Correct it and try again.");
            return;
        }


        int randColor = Random.Range(0, 3);
        int height = Random.Range(_minHeight, _maxHeight);
        GameObject building;
        bool useWholePiece=false;

        if(height==1)
        {
            building = PoolManager.Instance.GetUnusedObject(_theme + "_Whole_" + 0);//randColor);
            if(building)
            {
                useWholePiece = true;
                building.SetActive(true);
                building.transform.position = transform.position;
                building.transform.rotation = Quaternion.Euler(new Vector3(-90, 90 + transform.rotation.eulerAngles.y, 0));
                building.transform.localScale = Vector3.Scale(building.transform.localScale, transform.localScale);

                building.transform.parent = transform;
            }
        }
        if(!useWholePiece)
        {
            if (height != 1)
            {
                building = PoolManager.Instance.GetUnusedObject(_theme + "_Base_" + randColor);
                building.SetActive(true);
                building.transform.position = transform.position;
                building.transform.rotation = Quaternion.Euler(new Vector3(-90, 90 + transform.rotation.eulerAngles.y, 0));
                //building = GameObject.Instantiate(Resources.Load("Buildings/" + _theme + "_Base"), transform.position, Quaternion.Euler(new Vector3(-90, 90 + transform.rotation.eulerAngles.y, 0))) as GameObject;
                //building.renderer.materials[1] = _matPool.GetRandomMaterial(building.renderer.materials[1].name, randColor);
                building.transform.localScale = Vector3.Scale(building.transform.localScale, transform.localScale);

                building.transform.parent = transform;
            }

            for (int i = 1; i < (height - 1); i++)
            {
                building = PoolManager.Instance.GetUnusedObject(_theme + "_Middle_" + randColor);
                building.SetActive(true);
                building.transform.position = transform.position + Vector3.up * 30 * i * transform.localScale.y;
                building.transform.rotation = Quaternion.Euler(new Vector3(-90, 90 + transform.rotation.eulerAngles.y, 0));
                //building = GameObject.Instantiate(Resources.Load("Buildings/" + _theme + "_Middle"), transform.position + Vector3.up * 30 * i * transform.localScale.y, Quaternion.Euler(new Vector3(-90, 90 + transform.rotation.eulerAngles.y, 0))) as GameObject;
                //building.renderer.materials[1] = _matPool.GetRandomMaterial(building.renderer.materials[1].name, randColor);
                building.transform.localScale = Vector3.Scale(building.transform.localScale, transform.localScale);
                building.transform.parent = transform;
            }

            if (height >= 1)
            {
                building = PoolManager.Instance.GetUnusedObject(_theme + "_Top_" + randColor);
                building.SetActive(true);
                building.transform.position = transform.position + Vector3.up * 30 * (height - 1) * transform.localScale.y;
                building.transform.rotation = Quaternion.Euler(new Vector3(-90, 90 + transform.rotation.eulerAngles.y, 0));
                //building = GameObject.Instantiate(Resources.Load("Buildings/" + _theme + "_Top"), transform.position + Vector3.up * 30 * (height - 1) * transform.localScale.y, Quaternion.Euler(new Vector3(-90, 90 + transform.rotation.eulerAngles.y, 0))) as GameObject;
                //building.renderer.materials[1] = _matPool.GetRandomMaterial(building.renderer.materials[1].name, randColor);
                building.transform.localScale = Vector3.Scale(building.transform.localScale, transform.localScale);
                building.transform.parent = transform;
            }
        }
	}


    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(transform.position + Vector3.up * (_maxHeight * transform.localScale.y * 30 / 2), Vector3.Scale(new Vector3(50, 30 * _maxHeight, 50), transform.localScale));
    }
}

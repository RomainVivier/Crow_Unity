using UnityEngine;
using System.Collections;

public class BuildingGenerator : MonoBehaviour {

    public int _minHeight = 1;
    public int _maxHeight = 1;
    public string _theme;

	void Start()
    {
	    if(_minHeight < 1 || (_maxHeight < 1 || _maxHeight < _minHeight) || _theme == "")
        {
            Debug.Log("Some parameters are wrong. Correct it and try again.");
            return;
        }

        float randColor = Random.Range(0f, 1f);
        ProceduralMaterial mat;
        int height = Random.Range(_minHeight, _maxHeight);
        GameObject building;

        if (height != 1)
        {
            building = GameObject.Instantiate(Resources.Load("Buildings/" + _theme + "_Base"), transform.position, Quaternion.Euler(new Vector3(-90, 90 + transform.rotation.eulerAngles.y, 0))) as GameObject;
            mat = building.renderer.materials[1] as ProceduralMaterial;
            mat.SetProceduralFloat("Wall_Color", randColor);
            building.renderer.materials[1] = mat;
            building.transform.localScale = Vector3.Scale(building.transform.localScale, transform.localScale);

            building.transform.parent = transform;
        }

        for (int i = 1; i < (height - 1); i++)
        {
            building = GameObject.Instantiate(Resources.Load("Buildings/" + _theme + "_Middle"), transform.position + Vector3.up * 30 * i * transform.localScale.y, Quaternion.Euler(new Vector3(-90, 90 + transform.rotation.eulerAngles.y, 0))) as GameObject;
            mat = building.renderer.materials[1] as ProceduralMaterial;
            mat.SetProceduralFloat("Wall_Color", randColor);
            building.renderer.materials[1] = mat;
            building.transform.localScale = Vector3.Scale(building.transform.localScale, transform.localScale);
            building.transform.parent = transform;
        }

        if (height >= 1)
        {
            building = GameObject.Instantiate(Resources.Load("Buildings/" + _theme + "_Top"), transform.position + Vector3.up * 30 * (height - 1) * transform.localScale.y, Quaternion.Euler(new Vector3(-90, 90 + transform.rotation.eulerAngles.y, 0))) as GameObject;
            mat = building.renderer.materials[1] as ProceduralMaterial;
            mat.SetProceduralFloat("Wall_Color", randColor);
            building.renderer.materials[1] = mat;
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

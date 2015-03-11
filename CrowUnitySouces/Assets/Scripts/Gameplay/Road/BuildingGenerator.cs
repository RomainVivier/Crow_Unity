using UnityEngine;
using System.Collections;

public class BuildingGenerator : MonoBehaviour {

    public int _minHeight;
    public int _maxHeight;

    public string _enviro;
	public BuildingGenerator _nextBuildingGenerator=null;
	
	private int m_maxLength=-1;
	private bool m_dontGenerate=false;
	
	void Start()
    {
        if(!m_dontGenerate) generate();
	}


    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(transform.position + Vector3.up * (_maxHeight * transform.localScale.y * 30 / 2), Vector3.Scale(new Vector3(50, 30 * _maxHeight, 50), transform.localScale));
    }

    public void regenerate()
    {
        for(int i=0;i<transform.childCount;i++) GameObject.Destroy(transform.GetChild(i).gameObject);
        generate();
    }

    private void generate()
    {
		getMaxLength();    
	    if(_minHeight < 1 || (_maxHeight < 1 || _maxHeight < _minHeight) || _enviro=="")
        {
            Debug.Log("Some parameters are wrong. Correct it and try again.");
            return;
        }
        int height = Random.Range(_minHeight, _maxHeight);
        BuildingGeneratorParameters.RandomBuilding rb = GameObject.Find(_enviro)//(Resources.Load("Enviros/"+_enviro) as GameObject)
                    .GetComponent<BuildingGeneratorParameters>()
                    .getRandomBuilding(height,m_maxLength);

        GameObject building;
		
		if(rb.length>1) _nextBuildingGenerator.noBuilding(rb.length-2);
		
        for (int i = 0; i < height ; i++)
        {
            if(i==0) building = PoolManager.Instance.GetUnusedObject(rb.baseObject.name);
            else if(i==height-1) building=PoolManager.Instance.GetUnusedObject(rb.topObject.name);
            else building=PoolManager.Instance.GetUnusedObject(rb.middleObject.name);
            if(rb.material!=null) building.GetComponent<MeshRenderer>().material = rb.material;
            building.SetActive(true);
            building.transform.position = transform.position + Vector3.up * 30 * i * transform.localScale.y + (rb.length-1)*new Vector3(6,0,0);
            building.transform.rotation = Quaternion.Euler(new Vector3(-90, 90 + transform.rotation.eulerAngles.y, 0));
            building.transform.localScale = Vector3.Scale(building.transform.localScale, transform.localScale);
            building.transform.parent = transform;
            if(i==0)
            {
                GameObject go = new GameObject();
                go.transform.parent = building.transform;
                go.transform.localPosition = Vector3.zero;
                go.transform.localEulerAngles = Vector3.zero;
				go.layer = 2; //Ignore Raycast
                BoxCollider bc=go.AddComponent<BoxCollider>();
                bc.isTrigger = true;
                bc.center = new Vector3(0, -10, 2);
                bc.size = new Vector3(11, 20, 5);
                SoundPlayer sp = go.AddComponent<SoundPlayer>();
                sp._is3D = false;
                sp._onlyOnce = false;
                sp._soundName = "SFX/Env Objects/envSwooshBuildingEnterLeft";
                sp._soundNameRight="SFX/Env Objects/envSwooshBuildingEnterRight";
                sp._soundNameExit="SFX/Env Objects/envSwooshBuildingExitLeft";
                sp._soundNameExitRight="SFX/Env Objects/envSwooshBuildingExitRight";
            }
        }
    }
    
    private int getMaxLength()
    {
    	if(m_maxLength==-1)
    	{
    		if(_nextBuildingGenerator==null) m_maxLength=1;
    		else m_maxLength=_nextBuildingGenerator.getMaxLength()+1;
    	}
    	return m_maxLength;
    }
    
    private void noBuilding(int length)
    {
    	m_dontGenerate=true;
		for(int i=0;i<transform.childCount;i++) GameObject.Destroy(transform.GetChild(i).gameObject);
		if(length>0) _nextBuildingGenerator.noBuilding(length-1);
    }
}

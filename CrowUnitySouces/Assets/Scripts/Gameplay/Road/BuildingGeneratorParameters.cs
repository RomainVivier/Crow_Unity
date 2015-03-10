using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class BuildingGeneratorParameters : MonoBehaviour{

    [System.Serializable]
    public class BuildingInfos
    {
        public int maxHeight=0;
        public GameObject baseObject;
        public GameObject middleObject;
        public GameObject topObject;
        public float weight=1;
        public int length=1;
        public Material[] materials;
    }

    public class RandomBuilding
    {
        public GameObject baseObject;
        public GameObject middleObject;
        public GameObject topObject;
        public Material material;
        public int length;
        
        public RandomBuilding(BuildingInfos bi)
        {
            baseObject = bi.baseObject;
            middleObject = bi.middleObject;
            topObject = bi.topObject;
            material = bi.materials.Length != 0 ? bi.materials[Random.Range(0, bi.materials.Length)] : null;
            length=bi.length;
        }
    }
    public BuildingInfos[] _buildingInfos;

    public RandomBuilding getRandomBuilding(int height=0, int maxLength=1)
    {
        List<BuildingInfos> okInfos = _buildingInfos.Where(b => (height == 0 || b.maxHeight == 0 || b.maxHeight >= height) && b.length<=maxLength).ToList();
        float totalWeight = okInfos.Aggregate(0f, (val, b) => val + b.weight);
        float rndVal = Random.Range(0, totalWeight);

        while(okInfos.First().weight<=rndVal)
        {
            rndVal -= okInfos.First().weight;
            okInfos.RemoveAt(0);
        }
        return new RandomBuilding(okInfos.First());
    }
}

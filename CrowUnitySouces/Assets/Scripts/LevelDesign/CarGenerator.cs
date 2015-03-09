using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CarGenerator : MonoBehaviour {

	//PrefabSpawner[] _prefabSpawners;
	public GameObject _carPrefab;
	public float _difficultyMultiplier = 1.0f;
	PrefabSpawner[] m_spawnPoints;
	List<GameObject> m_cars = new List<GameObject>();

	const bool NOROADBLOCKS = false;
	
	void OnEnable()
	{
		int amountOfCarsToGenerate = (int)(_difficultyMultiplier * DifficultyManager.instance.GetRandomizedDifficulty ());
		Generate (amountOfCarsToGenerate);
		DifficultyManager.instance.IncreaseDifficulty ();
		Debug.Log ("Difficulty: " + DifficultyManager.instance._difficulty + "  Cars Generated: " + amountOfCarsToGenerate);
	}

	void Generate(int amountOfCarsToGenerate)
	{
		for (int i = 0; i < m_cars.Count; ++i) {
			GameObject.Destroy(m_cars[i]);
		}
		m_cars = new List<GameObject> ();

		List<PrefabSpawner> spawnPoints = new List<PrefabSpawner> ();

		for (int i = 1; i < m_spawnPoints.Length; ++i) {
			spawnPoints.Add (m_spawnPoints [i]);
		}

		if(NOROADBLOCKS)
		{
			for(int i = spawnPoints.Count - 1; i > -1; i-=3)
			{
				int removedIndex = i - Random.Range(0, 3);
				//Debug.Log("i = " + i + " / Removed: " + removedIndex);
				spawnPoints.RemoveAt(removedIndex);
			}
		}
		amountOfCarsToGenerate = Mathf.Min(amountOfCarsToGenerate, spawnPoints.Count);

		int randomIndex;
		PrefabSpawner selectedSpawnPoint;
		//GameObject newCar;
		for (int q = amountOfCarsToGenerate; q > 0; --q) {

			randomIndex = (int)(Random.value * spawnPoints.Count);
			selectedSpawnPoint = spawnPoints[randomIndex];
			selectedSpawnPoint.spawnPrefab();
			/*
			newCar = GameObject.Instantiate(_carPrefab) as GameObject;
			m_cars.Add(newCar);
			newCar.transform.parent = selectedSpawnPoint;
			newCar.transform.localPosition = Vector3.zero;
			newCar.rigidbody.velocity = Vector3.zero;
			newCar.rigidbody.angularVelocity = Vector3.zero;
			*/
			spawnPoints.Remove (selectedSpawnPoint);
		}
	}

	// Use this for initialization
	void Awake () {
		m_spawnPoints = GetComponentsInChildren<PrefabSpawner> ();
	}
}

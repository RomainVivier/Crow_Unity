using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CarGenerator : MonoBehaviour {

	public GameObject _carPrefab;
	Transform[] m_spawnPoints;
	List<GameObject> m_cars = new List<GameObject>();
	
	void OnEnable()
	{
		int amountOfCarsToGenerate = DifficultyManager.instance.GetRandomizedDifficulty ();
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

		List<Transform> spawnPoints = new List<Transform> ();

		for (int i = 0; i < m_spawnPoints.Length; ++i) {
			spawnPoints.Add (m_spawnPoints [i]);
		}

		int randomIndex;
		Transform selectedSpawnPoint;
		GameObject newCar;
		for (int q = amountOfCarsToGenerate; q > 0; --q) {
			randomIndex = (int)(Random.value * spawnPoints.Count + 1);
			selectedSpawnPoint = spawnPoints[randomIndex];
			newCar = GameObject.Instantiate(_carPrefab) as GameObject;
			m_cars.Add(newCar);
			newCar.transform.parent = selectedSpawnPoint;
			newCar.transform.localPosition = Vector3.zero;
			newCar.rigidbody.velocity = Vector3.zero;
			newCar.rigidbody.angularVelocity = Vector3.zero;
			spawnPoints.Remove (selectedSpawnPoint);
		}
	}

	// Use this for initialization
	void Awake () {
		m_spawnPoints = GetComponentsInChildren<Transform> ();
	}
}

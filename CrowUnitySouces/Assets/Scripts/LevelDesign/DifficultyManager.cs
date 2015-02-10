using UnityEngine;
using System.Collections;

public class DifficultyManager : MonoBehaviour 
{
	private static DifficultyManager _instance;
	
	public static DifficultyManager instance
	{
		get
		{
			if(_instance == null)
				_instance = GameObject.FindObjectOfType<DifficultyManager>();
			return _instance;
		}
	}

	public float _difficultyIncrement = 0.1f;
	public float _difficulty = 1.0f;
	public float _randomRange = 2.0f;
	public int _seed = 0;

	public void IncreaseDifficulty()
	{
		_difficulty += _difficultyIncrement;
	}
	public int GetRandomizedDifficulty()
	{
		//Random.seed = _seed++; 
		float result = _difficulty - _randomRange * 0.5f + Random.value * _randomRange + 1;
		return (int)result;
	}
}
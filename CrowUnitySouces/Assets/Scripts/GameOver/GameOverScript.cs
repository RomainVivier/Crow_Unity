using UnityEngine;
using System.Collections;

public class GameOverScript : MonoBehaviour
{
	public GameOverScreen _gameOverScreen;
	public HighScoresScreen _highScoresScreen;
	public CreditsScreen _creditsScreen;
	
	public float _scrollSpeed=0;
	
	private float m_scrollPos=0;
	
	private FMOD.Studio.EventInstance m_gameOverEvent;
	private FMOD.Studio.ParameterInstance m_gameOverParam;
	
	void Start()
	{
		//m_gameOverEvent=FMOD_StudioSystem.instance.GetEvent("event:/Meta/gameOver");
		//m_gameOverEvent.getParameter("gameOver", out m_gameOverParam);
		//m_gameOverParam.setValue(0);
		//m_gameOverEvent.start();
	}
	
	public void startGameOver()
	{
		if(_gameOverScreen.gameObject.activeSelf || _highScoresScreen.gameObject.activeSelf) return;
		KeyBinder.Instance.enabled=false;
		_gameOverScreen.gameObject.SetActive(true);
		_gameOverScreen.init();
		m_scrollPos=0;
		
		GameObject.Find ("CarV2").GetComponent<RailsControl>().setSpeedKmh=0;
		GameObject.Find ("CarV2").GetComponent<RailsControl>().minSpeedKmh=0;
		GameObject.Find ("CarV2").GetComponent<PolynomialEngine>().maxPowerKw=0;	
		GameObject.Find ("CarV2").GetComponent<PolynomialEngine>().powerMinRpmKw=0;
		GameObject.Find ("CarV2").GetComponent<Car>().updateValues();
		
		//m_gameOverParam.setValue(1);	
		//m_gameOverEvent.start();
		FMOD_StudioSystem.instance.PlayOneShot("event:/Meta/gameOver",transform.position);
	}
	
	public void startHighScores()
	{
		_gameOverScreen.gameObject.SetActive(false);
		_highScoresScreen.gameObject.SetActive(true);
		_highScoresScreen.init();
	}
	
	public void showCredits()
	{
		_highScoresScreen.gameObject.SetActive(false);
		_creditsScreen.gameObject.SetActive(true);
	}
	
	public void restartGame()
	{
		KeyBinder.Instance.enabled=true;
		//m_gameOverParam.setValue(0);
		FMOD_StudioSystem.Destroy(FMOD_StudioSystem.instance);
		Application.LoadLevel(1);
	}
	
	void Update()
	{
		m_scrollPos+=_scrollSpeed*Time.deltaTime;
		if(_gameOverScreen.gameObject.activeSelf) _gameOverScreen.setScrollPos(m_scrollPos);
		if(_highScoresScreen.gameObject.activeSelf) _highScoresScreen.setScrollPos(m_scrollPos);
		if(Input.GetKeyDown(KeyCode.G)) startGameOver();
	}
}

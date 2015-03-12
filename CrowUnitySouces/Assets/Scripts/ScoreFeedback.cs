using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ScoreFeedback : MonoBehaviour
{

    #region attributes
    const float DISPLAY_TIME = 2; 
    private Timer m_timer;
    private RectTransform m_rectTransform;
    private Transform m_cameraTransform;
    #endregion

    #region methods
    public void init(Vector3 pos, int score, int combo)
    {
        // Get references
        m_rectTransform = GetComponent<RectTransform>();
        m_rectTransform.position = pos;
        m_cameraTransform = Car.Instance.gameObject.transform.FindChild("Body/CameraRoot/CameraDashboard/CameraEnvironment").transform;

        // Set text
        GetComponent<Text>().text = "";//score + " x " + combo;

        // Start timer
        m_timer = new Timer(DISPLAY_TIME);
    }

    void Update ()
    {
        m_rectTransform.rotation = m_cameraTransform.rotation;
        if (m_timer.IsElapsedOnce) GameObject.Destroy(this.gameObject);
	}
    #endregion
}

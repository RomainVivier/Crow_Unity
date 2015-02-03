using UnityEngine;
using System.Collections;

public class AmbientLightChanger : MonoBehaviour
{
    #region attributes

    // Change this in-game
    public Color AmbientLight
    {
        get { return RenderSettings.ambientLight; }
        set { RenderSettings.ambientLight = value; }
    }

    // Change this in the editor
    public Color c;

    #endregion

    #region methods
    void Start ()
    {
	
	}
	
	void Update ()
    {

    }
    #endregion
}

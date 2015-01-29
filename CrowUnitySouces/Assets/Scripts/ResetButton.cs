using UnityEngine;
using System.Collections;

public class ResetButton : MonoBehaviour {
    public void click()
    {
        Application.LoadLevel(Application.loadedLevelName);
    }
}

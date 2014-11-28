using UnityEngine;
using System.Collections;

public class JB : MonoBehaviour
{
    public float maxFlareBrightness=25;
    public float flareCurvePower = 2;
    public float startFlareTime = 3;
    public float endFlareTime = 10;
    public float startFadeoutTime = 10;
    public float endFadeoutTime = 13;

    private bool active;
    private float runningTime;
    private LensFlare flareL;
    private LensFlare flareR;

    // Use this for initialization
	void Start ()
    {
        active = false;
        guiTexture.pixelInset=new Rect(0f, 0f, Screen.width, Screen.height);
	}
	
	// Update is called once per frame
	void FixedUpdate()
    {
        if(active)
        {
            runningTime += Time.fixedDeltaTime;
            float flareTime = (runningTime - startFlareTime) / (endFlareTime - startFlareTime);
            if (flareTime > 1) flareTime = 1;
            if (flareTime < 0) flareTime = 0;
            float brightness = Mathf.Pow(flareTime, flareCurvePower)*maxFlareBrightness;
            flareL.brightness = brightness;
            flareR.brightness = brightness;
            float alpha=(runningTime-startFadeoutTime)/(endFadeoutTime-startFadeoutTime);
            if(alpha<0) alpha=0;
            if(alpha>1) alpha=1;
            guiTexture.color = new Color(0,0,0,alpha);
            guiTexture.enabled = true;
        }
	}

    public bool activate()
    {
        bool ret = active;
        if(ret==false)
        {
            active = true;
            runningTime = 0;
        }
        return ret;
    }

    public void OnValidate()
    {
        flareL = GameObject.Find("Car").transform.FindChild("Body")
                                       .transform.FindChild("FlareL").GetComponent<LensFlare>();
        flareR = GameObject.Find("Car").transform.FindChild("Body")
                                       .transform.FindChild("FlareR").GetComponent<LensFlare>();
    }

    public void OnGUI()
    {
        if(runningTime>endFadeoutTime)
        {
            GUIStyle style = new GUIStyle();
            style.fontSize = 36;
            style.richText = true;
            style.alignment = TextAnchor.MiddleCenter;

            GUI.Label(
                        new Rect(Screen.width*0.4f, Screen.height*0.4f,
                                 Screen.width*0.2f, Screen.height*0.2f),
                      "<color=#ffffffff>To be continued.\nSee you in March.</color>",style);
        }
    }
}

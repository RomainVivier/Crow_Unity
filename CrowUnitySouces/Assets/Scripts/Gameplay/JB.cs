using UnityEngine;
using System.Collections;

public class JB : MonoBehaviour
{
    public float firstFlareCurvePower = 2;
    public float secondFlareCurvePower = 2;
    public float startFlareTime = 3;
    public float midFlareTime = 5;
    public float midFlareBrightness = 5;
    public float endFlareTime = 10;
    public float endFlareBrightness=10;
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
            float firstFlareTime = (runningTime - startFlareTime) / (midFlareTime - startFlareTime);
            float brightness = 0;
            if (firstFlareTime > 1)
            {
                float secondFlareTime=(runningTime - midFlareTime) / (endFlareTime - startFlareTime);
                if (secondFlareTime > 1) secondFlareTime = 1;
                brightness = Mathf.Lerp(midFlareBrightness, endFlareBrightness, Mathf.Pow(secondFlareTime, secondFlareCurvePower));
            }
            else
            {
                if (firstFlareTime < 0) firstFlareTime = 0;
                brightness = Mathf.Pow(firstFlareTime, firstFlareCurvePower)*midFlareBrightness;
            }
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
			GameObject car = GameObject.Find("Car(Clone)");
			if(car == null)
				return ret;
			flareL = car.transform.FindChild("Body")
				.transform.FindChild("FlareL").GetComponent<LensFlare>();
			flareR = car.transform.FindChild("Body")
				.transform.FindChild("FlareR").GetComponent<LensFlare>();

            active = true;
            runningTime = 0;
            GameObject.Find("Car(Clone)/AI").GetComponent<AI>().playDialog("test");
        }
        return ret;
    }

    public void OnValidate()
    {
		GameObject car = GameObject.Find("Car(Clone)");
		if(car == null)
			return;
		flareL = car.transform.FindChild("Body")
                                       .transform.FindChild("FlareL").GetComponent<LensFlare>();
		flareR = car.transform.FindChild("Body")
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

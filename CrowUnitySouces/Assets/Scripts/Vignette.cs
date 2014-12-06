using UnityEngine;
using System.Collections;

public class Vignette : MonoBehaviour
{

    public float defaultTime = 5;
    private float remainingTime;

	void Start ()
    {
	
	}
	
    void Update()
    {
        remainingTime -= Time.deltaTime;
        if (remainingTime <= 0) camera.enabled = false;
        GameObject child = transform.GetChild(0).gameObject;
        child.transform.position = new Vector3(0.5f, 0.5f, 1.5f);
        child.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
    }

    public void pop(float time=-1)
    {
        pop(camera.rect,time);
    }

    public void pop(Rect rect, float time=-1)
    {
        if (time == -1) time = defaultTime;
        remainingTime = time;
        camera.enabled = true;
        camera.rect=rect;
        float mainVfov = GameObject.Find("Car/Body/Camera").camera.fieldOfView;
        camera.fieldOfView=Mathf.Atan(Mathf.Tan(Mathf.Deg2Rad*mainVfov)*rect.height)*Mathf.Rad2Deg;
    }

    public void stop()
    {
        remainingTime = 0;
    }
}

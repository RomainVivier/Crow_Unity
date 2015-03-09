using UnityEngine;
using System.Collections;

public class RainbowPointLight : MonoBehaviour {

    public float _gradiantSpeed=1;
    public ParticleSystem _particlesSystem;

    private float m_gradiantPos;
    private Light m_light;

	void Start ()
    {
        m_light = GetComponent<Light>();
	}
	
	void Update ()
    {
	    if(_particlesSystem.isPlaying)
        {
            m_light.enabled = true;
            m_gradiantPos += _gradiantSpeed * Time.deltaTime;
            Color color=Color.black;
            float inPos = (m_gradiantPos * 6) % 1;
            int outPos = (Mathf.FloorToInt(m_gradiantPos * 6))%6;
            switch(outPos)
            {
                case 0:color = new Color(1, 0, inPos);break;
                case 1:color = new Color(1-inPos, 0, 1);break;
                case 2:color = new Color(0, inPos, 1); break;
                case 3:color = new Color(0, 1, 1-inPos); break;
                case 4:color = new Color(inPos, 1, 0); break;
                case 5:color = new Color(1, 1-inPos, 0); break;
            }
            m_light.color = color;
        }
        else
        {
            m_gradiantPos = 0;
            m_light.enabled = false;
        }
	}
}

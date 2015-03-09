using UnityEngine;
using System.Collections;

public class RainbowPointLight : MonoBehaviour {

    public ParticleSystem _particlesSystem;
	public Color[] _colors;
	public float _time;
	public bool loop = false;

    private float m_gradiantPos;
    private Light m_light;
	private int m_nbColors;
	
	void Start ()
    {
        m_light = GetComponent<Light>();
        m_nbColors=_colors.Length;
	}
	
	void Update ()
    {
	    if(_particlesSystem==null || _particlesSystem.isPlaying)
        {
            m_light.enabled = true;
            m_gradiantPos += Time.deltaTime/_time;
            //Color color=Color.black;
			if(!loop && m_gradiantPos>(m_nbColors-2f)/(m_nbColors-1f)) m_gradiantPos=(m_nbColors-2f)/(m_nbColors-1f);
				
            float inPos = (m_gradiantPos * m_nbColors) % 1;
            int outPos = (Mathf.FloorToInt(m_gradiantPos * m_nbColors))%m_nbColors;
            Color color, prevColor,nextColor;
            prevColor=_colors[outPos];
            nextColor=_colors[(outPos+1)%m_nbColors];
            
            color.r=Mathf.Lerp(prevColor.r,nextColor.r,inPos);
			color.g=Mathf.Lerp(prevColor.g,nextColor.g,inPos);
			color.b=Mathf.Lerp(prevColor.b,nextColor.b,inPos);
			color.a=Mathf.Lerp(prevColor.a,nextColor.a,inPos);

            m_light.color = color;
        }
        else
        {
            m_gradiantPos = 0;
            m_light.enabled = false;
        }
	}
	
	public void Reset()
	{
		m_gradiantPos=0;
	}
}

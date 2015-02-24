using UnityEngine;
using System.Collections;

public class FakeRPM : MonoBehaviour
{

    #region parameters
    [System.Serializable]
    public struct GearParameters
    {
        public float startRPM;
        public float endRPM;
        public float gearTime;
        public float power;
        public float instability;
    }
    public GearParameters[] _gears;

    public float _declutchTime;
    public float _clutchTime;
    public float _shiftTime;
    public float _disengagedRPM;

    #endregion

    #region private attributes
    private int m_currentGear = 0;
    private float m_currentGearPos = 0;
    private bool m_disengaged = false;
    private float m_timeToReengage = 0;
    #endregion

    #region methods
    void Start ()
    {
	    
	}
	
	public void update (float acceleration, float brake)
    {
        if(m_disengaged)
        {
            m_timeToReengage -= Time.fixedDeltaTime;
            if (m_timeToReengage <= 0)
            {
                m_disengaged = true;
                m_timeToReengage = 0;
            }
        }
        else
        {
            m_currentGearPos += acceleration * Time.fixedDeltaTime / _gears[m_currentGear].gearTime;
            if(m_currentGearPos>1)
            {
                if (m_currentGear == _gears.Length - 1) m_currentGearPos = 1;
                else
                {
                    m_currentGear++;
                    m_currentGearPos = 0;
                    m_disengaged = true;
                    m_timeToReengage = _shiftTime;
                    FMOD_StudioSystem.instance.PlayOneShot("event:/SFX/Car Mechanics/carGearUp", transform.position);
                }
            }
            if(m_currentGearPos<0)
            {
                if(m_currentGear==0) m_currentGearPos=0;
                else
                {
                    m_disengaged = true;
                    m_currentGearPos=1;
                    m_currentGear--;
                    m_timeToReengage = _shiftTime;
                    FMOD_StudioSystem.instance.PlayOneShot("event:/SFX/Car Mechanics/carGearDown", transform.position);
                }
            }
        }
    }

    public float getRPM()
    {
        float pos = Mathf.Pow(m_currentGearPos, _gears[m_currentGear].power);
        float rpm = Mathf.Lerp(_gears[m_currentGear].startRPM, _gears[m_currentGear].endRPM, pos);
        rpm += (Mathf.PerlinNoise(m_currentGearPos*100, 0)-0.5f)*_gears[m_currentGear].instability;
        if(m_disengaged)
        {
            float engaged=0;
            if(m_timeToReengage < _clutchTime) engaged=(m_timeToReengage / _clutchTime);
            if (_shiftTime - m_timeToReengage < _declutchTime) engaged=(_shiftTime - m_timeToReengage) / _clutchTime;
            rpm = Mathf.Lerp(_disengagedRPM, rpm,engaged);
        }
        return rpm;
    }
    #endregion
}

using UnityEngine;

[System.Serializable]
public class Timer
{

    bool _once = true;
    float _duration = 1.0f;
    float _startTime = Time.time;

    public Timer() : this(0f, false)
    {
        _once = false;
    }

    public Timer(float duration) : this(duration, false)
    {
    }

    public Timer(float duration, bool forceStartTime)
    {
        _duration = duration;
        if (forceStartTime)
            _startTime = Time.time;
    }

    /*
     * Returns current time left
     *  Duration           0
     *  t= 0     ---->   t=end
     */
    public float Current
    {
        get { return _duration - (Time.time - _startTime); }
    }

    /*
     * Returns current time left normalized
     * It's a more convenient way to send value to shaders
     * for example
     *  1                  0
     *  t= 0     ---->   t=end
     */
    public float CurrentNormalized
    {
        get { return Mathf.Max(Mathf.Min(Current / _duration, 1.0f), 0.0f); }
    }


    private bool _isElapsedLoop = false;
    /*
     * Return true each frame the timer is done
     * (current value < 0)
     */
    public bool IsElapsedLoop
    {
        get
        {
            if (_isElapsedLoop)
            {
                return true;
            }
            else
            {
                _isElapsedLoop = (Time.time - _startTime > _duration);
                return _isElapsedLoop;
            }
        }
    }

    /*
     * Return true only once, after the timer is done
     * (current value < 0)
     */
    public bool IsElapsedOnce
    {
        get
        {
            if (IsElapsedLoop)
            {
                if (_once)
                {
                    _once = false;
                    return true;
                }
                else return false;
            }
            return false;
        }
    }

    /*
     * Make the Timer restart
     * Giving a new duration > 0 will make this
     * duration the new one
     */
    public void Reset(float newDuration)
    {
        if (newDuration > 0.0f)
            _duration = newDuration;
        _startTime = Time.time;
        _isElapsedLoop = false;
        _once = true;
    }
}
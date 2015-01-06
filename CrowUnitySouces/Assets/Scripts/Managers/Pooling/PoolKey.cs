using UnityEngine;
using System.Collections;

[System.Serializable]
public class PoolKey {

    [UnityEngine.SerializeField]
    private string m_id;
    [UnityEngine.SerializeField]
    private Object m_object;
    [UnityEngine.SerializeField]
    private int m_number;

    public string Id
    {
        get { return m_id; }
        set { m_id = value; }
    }
    public Object Object
    {
        get { return m_object; }
        set { m_object = value; }
    }

    public int Number
    {
        get { return m_number; }
        set { m_number = value; }
    }
}

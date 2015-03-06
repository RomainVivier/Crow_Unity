using UnityEngine;
using System.Collections;

public class Burger : MonoBehaviour
{
    public Vector3 _direction;
    public float _speed;

    void Update()
    {
        transform.position += _direction * Time.deltaTime * _speed;
    }

}

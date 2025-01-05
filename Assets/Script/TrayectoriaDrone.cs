using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrayectoriaDrone : MonoBehaviour
{
    public Transform pointA;
    public Transform controlPoint; 
    public Transform pointB;
    public float speed = 1.0f; 

    private float t = 0.0f;

    void Update()
    {
        t += speed * Time.deltaTime;

        if (t > 1.0f)
            t = 0.0f; // Reinicia el movimiento

        // Calcula la posición en la curva de Bezier
        transform.position = Mathf.Pow(1 - t, 2) * pointA.position +
                             2 * (1 - t) * t * controlPoint.position +
                             Mathf.Pow(t, 2) * pointB.position;
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotacionPlaneta : MonoBehaviour
{
    public float velocidadRotacion = 50f;

    void Update()
    {
        transform.Rotate(Vector3.up * velocidadRotacion * Time.deltaTime);
    }
}

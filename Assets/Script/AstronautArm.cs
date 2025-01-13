using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

public class AstronautArm : MonoBehaviour
{
    public FABRIK fabrikScript;

    [Header("Joints")]
    public List<Transform> joints;
    public List<Vector3> links;

    public List<LineRenderer> LineRenderers = new List<LineRenderer>();
    public Material lineMaterial;

    [Header("Target")]
    public Transform target;
    public Transform target_2;

    [Header("CCD Parameters")]
    public float tolerance = 1.0f;
    public float maxIterations = 1e5f;

    public float smoothFactor = 0.1f;


    // Temporal Variables
    private int iterationCount = 0;
    private float rotation;
    private Vector3 axis;
    private int index = 0;
    private int state;


    private void Start()
    {
        SetLinks();

        foreach (Transform joint in joints)
        {
            LineRenderer lineRenderer = joint.gameObject.AddComponent<LineRenderer>();
            InitializeLineRenderer(lineRenderer);
            LineRenderers.Add(lineRenderer);
        }

        state = 0;
    }

    private void Update()
    {

        IncreaseJointCCD();
        UpdateVisualLinks();
    }

    //CDD algorithm
    private void IncreaseJointCCD()
    {
        //si no hemos llegado al objetivo
        if (iterationCount < maxIterations && Vector3.Distance(joints.Last().position, target.position) > tolerance)
        {

            if (index >= joints.Count - 1) 
                index = 0;
            else 
                index++;

            Vector3 currentJoint = joints[index].position;
            Vector3[] referenceVectors = GetReferenceVectors(currentJoint);

            rotation = GetRotationAngle(referenceVectors);
            axis = GetRotationAxis(referenceVectors);

            UpdateChildJointsPositions();

            iterationCount++;
        }
        else
        {
            catchDrone();
        }
    }

    //actualizamos la posición de todos los joints hijos usando quaternions
    private void UpdateChildJointsPositions()
    {
        Quaternion temporalQuaternion = Quaternion.AngleAxis(rotation * 180 / Mathf.PI, axis);
        if (index <= joints.Count - 2)
        {
            for (int i = index; i <= joints.Count - 2; i++)
            {
                //joints[i + 1].position = joints[i].position + temporalQuaternion * links[i];
                Vector3 targetPosition = joints[i].position + temporalQuaternion * links[i];
                Vector3 interpolatedPosition = Vector3.Lerp(joints[i + 1].position, targetPosition, smoothFactor);
                joints[i + 1].position = joints[i].position + (interpolatedPosition - joints[i].position).normalized * links[i].magnitude;

            }
        }

        //actualizamos los vectores entre cada joint
        UpdateJoints();
    }

    #region Support Functions

    private void SetLinks()
    {
        links.Clear();
        for (int i = 1; i < joints.Count; i++)
        {
            links.Add(joints[i].position - joints[i - 1].position);
        }
    }

    //encontramos el vector desde el joint actual al endFactor
    //y el vector entre el vector actual y el target
    private Vector3[] GetReferenceVectors(Vector3 currentJointPosition)
    {
        Vector3[] referenceVectors = new Vector3[2];

        referenceVectors[0] = Vector3.Normalize(joints.Last().position - currentJointPosition);
        referenceVectors[1] = Vector3.Normalize(target.position - currentJointPosition);

        return referenceVectors;
    }

    //Calculamos las rotaciones necesarias para alcanzar nuestro objetivo usando producto escalar
    private float GetRotationAngle(Vector3[] referenceVectors)
    {
        float theta = Mathf.Acos(
            Mathf.Clamp(
                Vector3.Dot(referenceVectors[0], referenceVectors[1]),
                -1.0f,
                1.0f));

        return theta;
    }

    //Calculamos el eje de rotacion
    private Vector3 GetRotationAxis(Vector3[] referenceVectors)
    {
        Vector3 rotationAxis = Vector3.Cross(referenceVectors[0], referenceVectors[1]);
        return rotationAxis;
    }

    void InitializeLineRenderer(LineRenderer lineRenderer)
    {
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
        lineRenderer.positionCount = 2;
        lineRenderer.material = lineMaterial;
        lineRenderer.startColor = Color.white;
        lineRenderer.endColor = Color.white;
    }

    void UpdateVisualLinks()
    {
        for (int i = 0; i < joints.Count - 1; i++)
        {
            LineRenderers[i].SetPosition(0, joints[i].position);
            LineRenderers[i].SetPosition(1, joints[i + 1].position);
        }
    }

    private void UpdateJoints()
    {
        SetLinks();
    }

    void catchDrone()
    {
        if(state == 0)
        {
            if (fabrikScript != null)
            {
                fabrikScript.dropDrone();
                fabrikScript.retirada();
            }

            target.SetParent(joints[joints.Count - 1]);

            target = target_2;

            state = 1;
        }
        
    }

    #endregion

}
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

public class AstronautArm : MonoBehaviour
{
    [Header("Joints")]
    public List<Transform> joints;
    public List<Vector3> links;

    [Header("Target")]
    public Transform target;

    [Header("CCD Parameters")]
    public float tolerance = 1.0f;
    public float maxIterations = 1e5f;

    // Private Parameters

    // Temporal Variables
    private int iterationCount = 0;
    private float rotation;
    private Vector3 axis;
    private int index = 0;

    private void Start()
    {
        SetLinks();
    }

    private void Update()
    {
        IncreaseJointCCD();
    }

    #region CCD

    private void IncreaseJointCCD()
    {
        if (iterationCount < maxIterations && Vector3.Distance(joints.Last().position, target.position) > tolerance)
        {

            if (index >= 4) index = 0;
            else index++;

            Vector3 currentJoint = joints[index].position;
            Vector3[] referenceVectors = GetReferenceVectors(currentJoint);

            rotation = GetRotationAngle(referenceVectors);
            axis = GetRotationAxis(referenceVectors);

            UpdateChildJointsPositions();

            iterationCount++;
        }
    }

    private void UpdateChildJointsPositions()
    {
        Quaternion temporalQuaternion = Quaternion.AngleAxis(rotation * 180 / Mathf.PI, axis);
        if (index <= joints.Count - 2)
        {
            for (int i = index; i <= joints.Count - 2; i++)
            {
                joints[i + 1].position = joints[i].position + temporalQuaternion * links[i];
            }
        }

        UpdateJoints();
    }

    #endregion

    #region Support Functions

    private void SetLinks()
    {
        links.Clear();
        for (int i = 1; i < joints.Count; i++)
        {
            links.Add(joints[i].position - joints[i - 1].position);
        }
    }

    private Vector3[] GetReferenceVectors(Vector3 currentJointPosition)
    {
        Vector3[] referenceVectors = new Vector3[2];

        referenceVectors[0] = Vector3.Normalize(joints.Last().position - currentJointPosition);
        referenceVectors[1] = Vector3.Normalize(target.position - currentJointPosition);

        return referenceVectors;
    }

    private float GetRotationAngle(Vector3[] referenceVectors)
    {
        float theta = Mathf.Acos(
            Mathf.Clamp(
                Vector3.Dot(referenceVectors[0], referenceVectors[1]),
                -1.0f,
                1.0f));

        return theta;
    }

    private Vector3 GetRotationAxis(Vector3[] referenceVectors)
    {
        Vector3 rotationAxis = Vector3.Cross(referenceVectors[0], referenceVectors[1]);
        return rotationAxis;
    }

    private void UpdateJoints()
    {
        SetLinks();
    }

    #endregion

}

using UnityEngine;

public class Camera : MonoBehaviour
{
    public Camera camera1;
    public Camera camera2;

    private void Start()
    {
        SetActiveCamera(camera1);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SetActiveCamera(camera1);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SetActiveCamera(camera2);
        }
    }

    private void SetActiveCamera(Camera activeCamera)
    {
        camera1.gameObject.SetActive(activeCamera == camera1);
        camera2.gameObject.SetActive(activeCamera == camera2);
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CamGyro : MonoBehaviour
{
    private bool gyroEnabled;
    private Gyroscope gyro;

    private GameObject cameraContainer;
    private Quaternion rot;


    public GameObject cameraPosition1;
    public GameObject cameraPosition2;

    private void Start()
    {
        cameraContainer = new GameObject("Camera Container");
        cameraContainer.transform.position = transform.position;
        transform.SetParent(cameraContainer.transform);

        gyroEnabled = EnableGyro();

    }

    private bool EnableGyro()
    {
        if (SystemInfo.supportsGyroscope)
        {
            gyro = Input.gyro;
            gyro.enabled = true;

            cameraContainer.transform.rotation = Quaternion.Euler(90f, 90f, 0f);
            rot = new Quaternion(0, 0, 1, 0);

            return true;
        }
        return false;
    }
    private void Update()
    {
        if (gyroEnabled)
        {
            transform.localRotation = gyro.attitude * rot;
        }
    }

    public void ChangePosition()
    {
        Vector3 position1 = cameraPosition1.transform.position;
        Vector3 position2 = cameraPosition2.transform.position;

        if (cameraContainer.transform.position == position1)
        {
            cameraContainer.transform.position = position2;
        }

        else if (cameraContainer.transform.position == position2)
        {
            cameraContainer.transform.position = position1;
        }

    }

}

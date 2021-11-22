using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CamGyro : MonoBehaviour
{
    public Camera arCamera;
    public GameObject cameraPositions;
    public GameObject startPosition;
    public GameObject ceilingLights;
    public GameObject roof;
    public GameObject camera2DPosition;

    private bool gyroEnabled;
    private Gyroscope gyro;

    private GameObject cameraContainer;
    private Quaternion rot;

    private GameObject hitObject;

    private GameObject currentCameraPosition;
    private GameObject previousCameraPosition;

    private bool camera2D;


   // public GameObject[] cameraPositions;

    private void Start()
    {
        cameraContainer = new GameObject("Camera Container");
        cameraContainer.transform.position = transform.position;
        transform.SetParent(cameraContainer.transform);

        gyroEnabled = EnableGyro();
       
        currentCameraPosition = startPosition;
        startPosition.SetActive(false);

        roof.SetActive(true);

        camera2D = false;

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

        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                RaycastChangePosition(touch);
            }

            if (camera2D == true)
            {
                if (touch.phase == TouchPhase.Moved)
                {
                    MoveCamera2D(touch);
                }

                Touch touch2 = Input.GetTouch(1);

            }
        }
    }


    /*
    public void ChangePosition()
    {
        Vector3 position1 = cameraPositions[0].transform.position;
        Vector3 position2 = cameraPositions[1].transform.position;

        if (cameraContainer.transform.position == position1)
        {
            cameraContainer.transform.position = position2;
        }

        else if (cameraContainer.transform.position == position2)
        {
            cameraContainer.transform.position = position1;
        }

    }
    */

    public void RaycastChangePosition(Touch touch)
    {
        Ray ray = arCamera.ScreenPointToRay(touch.position);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            hitObject = hit.transform.gameObject;

            Debug.Log(hitObject);

            if (hitObject.tag == "Camera Position")
            {

                if (camera2D == true)
                {
                    arCamera.orthographic = false;
                    gyroEnabled = true;
                    camera2D = false;
                }

                previousCameraPosition = currentCameraPosition;
                currentCameraPosition = hitObject;               

                cameraContainer.transform.position = hitObject.transform.position;

                previousCameraPosition.SetActive(true);
                currentCameraPosition.SetActive(false);

            }
        }
    }

    public void ShowCameraPositions()
    {
        if (cameraPositions.activeSelf)
        {
            cameraPositions.SetActive(false);
        }
        else if (!cameraPositions.activeSelf)
        {
            cameraPositions.SetActive(true);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Roof")
        {
            roof.SetActive(false);
            ceilingLights.SetActive(false);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Roof")
        {
            roof.SetActive(true);
            ceilingLights.SetActive(true);
        }
    }

    public void Camera2D()
    {
        if (cameraContainer.transform.position != camera2DPosition.transform.position)
        {
            camera2D = true;

            previousCameraPosition = currentCameraPosition;
            currentCameraPosition = camera2DPosition;

            cameraContainer.transform.position = camera2DPosition.transform.position;

            previousCameraPosition.SetActive(true);
            currentCameraPosition.SetActive(false);

            arCamera.orthographic = true;
            gyroEnabled = false;

            transform.localRotation = Quaternion.Euler(0f, 0f, 0f);

        }
    }

    public void MoveCamera2D(Touch touch)
    {

    }
}

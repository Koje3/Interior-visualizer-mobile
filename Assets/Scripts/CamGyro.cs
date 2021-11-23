using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class CamGyro : MonoBehaviour
{
    public Camera arCamera;
    public GameObject cameraPositions;
    public GameObject startPosition;
    public GameObject cameraTopViewPosition;
    [Space(10)]
    public GameObject ceilingLights;
    public GameObject roof;   
    public GameObject button2D;
    [Space(10)]
    public float panSpeed = 0.1f;
    public float zoomSpeed = 0.1f;
    public float zoomSpeed3D = 1f;
    [Space(10)]
    public float zoomOutMin = 1f;
    public float zoomOutMax = 10f;

    private bool gyroEnabled;
    private Gyroscope gyro;

    private GameObject cameraContainer;
    private Quaternion rot;

    private GameObject hitObject;

    private GameObject currentCameraPosition;
    private GameObject previousCameraPosition;

    private bool camera2D;
    private bool topView;

    private float startFieldOfView;
    private float tempFieldOfView;
    private float startOrthographicSize;
    private float tempOrthographicSize;


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
        topView = false;

        startFieldOfView = arCamera.fieldOfView;
        tempFieldOfView = arCamera.fieldOfView;

        startOrthographicSize = 5;
        tempOrthographicSize = startOrthographicSize;
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
            Touch touch1 = Input.GetTouch(0);

            if (Input.touchCount < 2)
            {
                if (touch1.phase == TouchPhase.Began)
                {
                    RaycastChangePosition(touch1);
                }
            }


            if (topView == true)
            {
                if (Input.touchCount < 2)
                {
                    if (touch1.phase == TouchPhase.Moved)
                    {
                        PanCamera(touch1);
                    }
                }


                if (Input.touchCount == 2)
                {
                    Touch touch2 = Input.GetTouch(1);

                    ZoomCamera(touch1, touch2);
                }
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

                if (topView == true)
                {
                    transform.localRotation = Quaternion.Euler(90f, 90f, 0f);

                    arCamera.orthographic = false;
                    arCamera.fieldOfView = startFieldOfView; 
                    gyroEnabled = true;
                    camera2D = false;
                    topView = false;

                    button2D.GetComponentInChildren<Text>().text = "2D VIEW";
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
        if (!camera2D)
        {
            camera2D = true;
            topView = true;

            previousCameraPosition = currentCameraPosition;
            currentCameraPosition = cameraTopViewPosition;

            cameraContainer.transform.position = cameraTopViewPosition.transform.position;

            previousCameraPosition.SetActive(true);
            currentCameraPosition.SetActive(false);

            arCamera.orthographic = true;
            gyroEnabled = false;

            transform.localRotation = Quaternion.Euler(0f, 0f, 0f);

            button2D.GetComponentInChildren<Text>().text = "3D VIEW";

            arCamera.orthographicSize = tempOrthographicSize;

        }
        else
        {
            camera2D = false;
            arCamera.orthographic = false;

            button2D.GetComponentInChildren<Text>().text = "2D VIEW";

            arCamera.fieldOfView = tempFieldOfView;
        }
    }

    public void TopView()
    {
        if (!topView)
        {
            topView = true;

            previousCameraPosition = currentCameraPosition;
            currentCameraPosition = cameraTopViewPosition;

            cameraContainer.transform.position = cameraTopViewPosition.transform.position;

            previousCameraPosition.SetActive(true);
            currentCameraPosition.SetActive(false);

            arCamera.orthographic = true;
            gyroEnabled = false;

            transform.localRotation = Quaternion.Euler(0f, 0f, 0f);

        }
    }

    public void PanCamera(Touch touch)
    {
        Vector2 touchDeltaPosition = touch.deltaPosition;
        cameraContainer.transform.Translate(-touchDeltaPosition.x * panSpeed * Time.deltaTime, -touchDeltaPosition.y * panSpeed * Time.deltaTime, 0);
    }

    public void ZoomCamera(Touch touch1, Touch touch2)
    {
       
        Vector2 touchZeroPrevPos = touch1.position - touch1.deltaPosition;
        Vector2 touchOnePrevPos = touch2.position - touch2.deltaPosition;

        float prevMagnitude = (touchZeroPrevPos - touchOnePrevPos).magnitude;
        float currentMagnitude = (touch1.position - touch2.position).magnitude;

        float difference = currentMagnitude - prevMagnitude;

        if (camera2D)
        {
            arCamera.orthographicSize = Mathf.Clamp(arCamera.orthographicSize - difference * zoomSpeed * Time.deltaTime, zoomOutMin, zoomOutMax);
            tempFieldOfView = Mathf.Clamp(tempFieldOfView - difference * zoomSpeed3D * Time.deltaTime, 30, 110);
        }
        else
        {
            arCamera.fieldOfView = Mathf.Clamp(arCamera.fieldOfView - difference * zoomSpeed3D * Time.deltaTime, 30, 110);
            tempOrthographicSize = Mathf.Clamp(tempOrthographicSize - difference * zoomSpeed * Time.deltaTime, zoomOutMin, zoomOutMax);
        }

    }

    public void ZoomCamera3D(Touch touch1, Touch touch2)
    {

    }

}

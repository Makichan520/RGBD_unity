using System.Collections;
using System.Collections.Generic;
using Perc6d;
using UnityEngine;
using UnityEngine.Perception.GroundTruth;

public class CameraCoordinator : MonoBehaviour
{
    public float               _screenCaptureInterval = 0.166f; 
    float                      _elapsedTime;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        _elapsedTime += Time.deltaTime;
        if (_elapsedTime > _screenCaptureInterval)
        {
            _elapsedTime -= _screenCaptureInterval;
            PerceptionCamera[] percCameras = GetComponentsInChildren<PerceptionCamera>();
            foreach (PerceptionCamera cam in percCameras)
            {
                cam.RequestCapture();
            }

            DepthCamera[] depthCameras = GetComponentsInChildren<DepthCamera>();
            foreach (DepthCamera cam in depthCameras)
            {
                cam.RequestCapture();
            }
        }
    }
}

<<<<<<< HEAD
using System;
using UnityEngine;
using UnityEngine.Perception.GroundTruth;
using UnityEngine.Rendering;
using Unity.Mathematics;

[Serializable]
public class IntrinsicMatrixLabeler : CameraLabeler
{
    public struct CameraIntrinsicsSpec
    {
        public Vector3 cam_position;
        public Quaternion cam_rotation;
        public float3x3 cam_intristicMatrix;
    }

    Camera m_Camera;
    AnnotationDefinition m_AnnotationDefinition;
    
    public string annotationId = "94179c03-6258-4cfe-8449-f337fcd24301";
    
    public override string description
    {
        get => "Outputs the camera sensor's position(world coordinate), rotation and intrinsic matrix for each captured frame.";
        protected set { }
    }
    protected override bool supportsVisualization => false;
    
    protected override void Setup()
    {
        m_Camera = perceptionCamera.GetComponent<Camera>();
        
        m_AnnotationDefinition = DatasetCapture.RegisterAnnotationDefinition(
            "Camera intrinsics",
            "Counts of objects for each label in the sensor's view",
            id: new Guid(annotationId));
        
    }

    float3x3 GetIntrinsic(Camera cam)
    {
        float pixel_aspect_ratio = (float)cam.pixelWidth / (float)cam.pixelHeight;

        float alpha_u = cam.focalLength * ((float)cam.pixelWidth / cam.sensorSize.x);
        float alpha_v = cam.focalLength * pixel_aspect_ratio * ((float)cam.pixelHeight / cam.sensorSize.y);

        float u_0 = (float)cam.pixelWidth / 2;
        float v_0 = (float)cam.pixelHeight / 2;

        //IntrinsicMatrix in row major
        float3x3 camIntriMatrix = new float3x3(alpha_u, 0f, u_0,
                                            0f, alpha_v, v_0,
                                            0f, 0f, 1f);
        return camIntriMatrix;
    }

    protected override void OnBeginRendering(ScriptableRenderContext scriptableRenderContext)
    {
        sensorHandle.ReportAnnotationValues(m_AnnotationDefinition, new [] { new CameraIntrinsicsSpec
        {
            cam_position = m_Camera.transform.position,
            cam_rotation = m_Camera.transform.rotation,
            cam_intristicMatrix = GetIntrinsic(m_Camera)
        }}
        );
    }


}
=======
using System;
using UnityEngine;
using UnityEngine.Perception.GroundTruth;
using UnityEngine.Rendering;
using Unity.Mathematics;

[Serializable]
public class IntrinsicMatrixLabeler : CameraLabeler
{
    public struct CameraIntrinsicsSpec
    {
        public Vector3 cam_position;
        public Quaternion cam_rotation;
        public float3x3 cam_intristicMatrix;
    }

    Camera m_Camera;
    AnnotationDefinition m_AnnotationDefinition;
    
    public string annotationId = "94179c03-6258-4cfe-8449-f337fcd24301";
    
    public override string description
    {
        get => "Outputs the camera sensor's position(world coordinate), rotation and intrinsic matrix for each captured frame.";
        protected set { }
    }
    protected override bool supportsVisualization => false;
    
    protected override void Setup()
    {
        m_Camera = perceptionCamera.GetComponent<Camera>();
        
        m_AnnotationDefinition = DatasetCapture.RegisterAnnotationDefinition(
            "Camera intrinsics",
            "Counts of objects for each label in the sensor's view",
            id: new Guid(annotationId));
        
    }

    float3x3 GetIntrinsic(Camera cam)
    {
        float pixel_aspect_ratio = (float)cam.pixelWidth / (float)cam.pixelHeight;

        float alpha_u = cam.focalLength * ((float)cam.pixelWidth / cam.sensorSize.x);
        float alpha_v = cam.focalLength * pixel_aspect_ratio * ((float)cam.pixelHeight / cam.sensorSize.y);

        float u_0 = (float)cam.pixelWidth / 2;
        float v_0 = (float)cam.pixelHeight / 2;

        //IntrinsicMatrix in row major
        float3x3 camIntriMatrix = new float3x3(alpha_u, 0f, u_0,
                                            0f, alpha_v, v_0,
                                            0f, 0f, 1f);
        return camIntriMatrix;
    }

    protected override void OnBeginRendering(ScriptableRenderContext scriptableRenderContext)
    {
        sensorHandle.ReportAnnotationValues(m_AnnotationDefinition, new [] { new CameraIntrinsicsSpec
        {
            cam_position = m_Camera.transform.position,
            cam_rotation = m_Camera.transform.rotation,
            cam_intristicMatrix = GetIntrinsic(m_Camera)
        }}
        );
    }


}
>>>>>>> 5ee80fd98b88603357853981fc41df1fc2d87b50

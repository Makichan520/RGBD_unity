using System;
using UnityEngine;
using UnityEngine.Perception.GroundTruth;
using UnityEngine.Rendering;
using Unity.Mathematics;

/// <summary>
/// Report the information of camera(about Intrinstic Matrix), and write into JSON files 
/// </summary>
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
        float v_fov = cam.fieldOfView;
        float h_fov = Camera.VerticalToHorizontalFieldOfView(cam.fieldOfView,cam.aspect);
        Debug.Log("v_fov: " + v_fov + "h_fov: " + h_fov);
        float f_x = (float)cam.pixelWidth / (float)Math.Tan(h_fov * Math.PI/360);
        float f_y = (float)cam.pixelHeight / (float)Math.Tan(v_fov * Math.PI/360);

        float u_0 = cam.pixelWidth;
        float v_0 = cam.pixelHeight;
        //IntrinsicMatrix in row major
        float3x3 camIntriMatrix = new float3x3(f_x, 0f, u_0,
                                            0f, f_y, v_0,
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
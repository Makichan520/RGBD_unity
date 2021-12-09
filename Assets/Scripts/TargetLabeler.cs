using System;
using UnityEngine;
using UnityEngine.Perception.GroundTruth;
using UnityEngine.Rendering;
using UnityEngine.Perception.Randomization.Randomizers;
using UnityEngine.Perception.Randomization.Randomizers.Tags;

[Serializable]
public class TargetLabeler : CameraLabeler
{
    public struct TargetInformation
    {
        public String name;
        public Vector3 position;
        public Quaternion rotation;
    }

    AnnotationDefinition m_AnnotationDefinition;

    /// <summary>
    /// Retrieves the RandomizerTagManager of the scenario containing this Randomizer
    /// </summary>
    public RandomizerTagManager tagManager => RandomizerTagManager.singleton;
    
    public string annotationId = "a644yhf1-7812-5as4-8762-32gs4f5ggbw8";
    
    public override string description
    {
        get => "Outputs the target object's position(world coordinate), rotation for each captured frame.";
        protected set { }
    }
    protected override bool supportsVisualization => false;
    
    protected override void Setup()
    {
        
        m_AnnotationDefinition = DatasetCapture.RegisterAnnotationDefinition(
            "Information of targets",
            "Display the position and rotation of targets for each frame in the sensor's view",
            id: new Guid(annotationId));
    }

    protected override void OnBeginRendering(ScriptableRenderContext scriptableRenderContext)
    {
        var objectTags = tagManager.Query<TargetTag>();
        foreach(var tag in objectTags){
            sensorHandle.ReportAnnotationValues(m_AnnotationDefinition, new [] { new TargetInformation
            {
            name = tag.name,
            position = tag.transform.position,
            rotation = tag.transform.rotation
            }}
        );            
        }
    }


}

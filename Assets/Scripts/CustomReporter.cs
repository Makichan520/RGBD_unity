using System;
using UnityEngine;
using UnityEngine.Perception.GroundTruth;

[RequireComponent(typeof(PerceptionCamera))]
public class CustomReporter : MonoBehaviour
{
    public GameObject[] targets;

    AnnotationDefinition TargetAnnotationDefinition;
    SensorHandle cameraSensorHandle;

    public struct TargetInformation{
        public String name;
        public Vector3 position;
        public Quaternion rotation;
    }

    private TargetInformation[] informations;

    public void Start()
    {
        //annotations are registered up-front
        TargetAnnotationDefinition = DatasetCapture.RegisterAnnotationDefinition(
            "Target position and rotation",
            "The position and rotation of the target in the world space",
            id: Guid.Parse("C0B4A22C-0420-4D9F-BAFC-954B8F7B35A7"));
    }

    public void Update()
    {
        //Report using the PerceptionCamera's SensorHandle if scheduled this frame
        var sensorHandle = GetComponent<PerceptionCamera>().SensorHandle;
        if (sensorHandle.ShouldCaptureThisFrame)
        {
            informations = new TargetInformation[targets.Length];
            int index = 0;
            foreach(var target in targets){
                informations[index++] = new TargetInformation {name = target.name, position = target.transform.position,rotation = target.transform.rotation};
            }
            sensorHandle.ReportAnnotationValues(
                TargetAnnotationDefinition,
                informations);
        }
    }
}

// Example metric that is added each frame in the dataset:
// {
//   "capture_id": null,
//   "annotation_id": null,
//   "sequence_id": "9768671e-acea-4c9e-a670-0f2dba5afe12",
//   "step": 1,
//   "metric_definition": "1f6bff46-f884-4cc5-a878-db987278fe35",
//   "values": [{ "x": 96.1856, "y": 192.676, "z": -193.8386 }]
// },

// Example annotation that is added to each capture in the dataset:
// {
//   "id": "33f5a3aa-3e5e-48f1-8339-6cbd64ed4562",
//   "annotation_definition": "c0b4a22c-0420-4d9f-bafc-954b8f7b35a7",
//   "values": [
//     [
//       -1.03097284,
//       0.07265166,
//       -6.318692
//     ]
//   ]
// }


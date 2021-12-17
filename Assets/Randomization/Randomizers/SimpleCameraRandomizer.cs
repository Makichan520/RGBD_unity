using System;
using UnityEngine.Perception.Randomization.Parameters;
using UnityEngine.Perception.Randomization.Randomizers.Tags;
using UnityEngine.Perception.Randomization.Samplers;

using Perc6d;
using UnityEngine.Perception.GroundTruth;

namespace UnityEngine.Perception.Randomization.Randomizers{
    /// <summary>
    /// Randomizes the rotation of objects tagged with a CameraRandomizerTag(for simple scene)
    /// </summary>
    [Serializable]
    [AddRandomizerMenu("Perception/Simple Camera Randomizer")]
    public class SimpleCameraRandomizer : Randomizer
    {
        /// <summary>
        /// The target object of camera, has to be in center of camera's screen.
        /// </summary>
        [Tooltip("The target object of camera.")]
        public GameObject target;


        /// <summary>
        /// The range of random rotations to assign to target objects
        /// </summary>
        [Tooltip("The range of random rotations to assign to target objects.")]
        public FloatParameter rotation_z = new FloatParameter { value = new UniformSampler(0,360) };

        
        [Tooltip("The range of random relative distance to assign to camera.")]
        public UniformSampler distance = new UniformSampler(0,3f);


        /// <summary>
        /// Randomizes the rotation and position of tagged objects at the start of each scenario iteration
        /// </summary>
        protected override void OnIterationStart()
        {

            var tags = tagManager.Query<CameraRandomizerTag>();
            foreach (var tag in tags){
                Vector3 position = Random.onUnitSphere * distance.Sample();
                position.y = Math.Abs(position.y);
                tag.transform.position = target.transform.position + position;
                tag.transform.LookAt(target.transform.position);
                tag.transform.Rotate(0,0,rotation_z.Sample(),Space.Self);
                Debug.Log("Camera in " + Time.frameCount + ". frame: pos = x:" + tag.transform.position.x + "y: " + tag.transform.position.y
                + "z: " + tag.transform.position.z +  "\n rot = " + tag.transform.rotation);
                RequestCapture(tag);
            }


        }
        
        private int RequestCapture(CameraRandomizerTag cameras){
            PerceptionCamera[] percCameras = cameras.GetComponentsInChildren<PerceptionCamera>();
            foreach (PerceptionCamera cam in percCameras)
            {
                Debug.Log("request perception...");
                cam.RequestCapture();
            }

            DepthCamera[] depthCameras = cameras.GetComponentsInChildren<DepthCamera>();
            foreach (DepthCamera cam in depthCameras)
            {
                Debug.Log("request depth...");
                cam.RequestCapture();
            }
            return 0;
        }

    }
}

using System;
using UnityEngine.Perception.Randomization.Parameters;
using UnityEngine.Perception.Randomization.Randomizers.Tags;
using UnityEngine.Perception.Randomization.Samplers;

namespace UnityEngine.Perception.Randomization.Randomizers{
    /// <summary>
    /// Randomizes the rotation of objects tagged with a CameraRandomizerTag
    /// </summary>
    [Serializable]
    [AddRandomizerMenu("Perception/Camera Randomizer")]
    public class CameraRandomizer : Randomizer
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

        /// <summary>
        /// The range of random relative distance to assign to camera.
        /// </summary>
        [Tooltip("The range of random relative distance to assign to camera.")]
        public Vector3Parameter position = new Vector3Parameter
        {
            x = new UniformSampler(0, 360),
            y = new UniformSampler(0, 360),
            z = new UniformSampler(0, 360)
        };

        /// <summary>
        /// Randomizes the rotation and position of tagged objects at the start of each scenario iteration
        /// </summary>
        protected override void OnIterationStart()
        {
            var tags = tagManager.Query<CameraRandomizerTag>();
            foreach (var tag in tags){
                tag.transform.position = position.Sample();
                tag.transform.LookAt(target.transform.position);
                tag.transform.Rotate(0,0,rotation_z.Sample(),Space.Self);
            }
        }
    }
}

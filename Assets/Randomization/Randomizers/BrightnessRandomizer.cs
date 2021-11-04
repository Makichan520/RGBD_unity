using System;
using UnityEngine.Perception.Randomization.Parameters;
using UnityEngine.Perception.Randomization.Randomizers.Tags;
using UnityEngine.Perception.Randomization.Samplers;

namespace UnityEngine.Perception.Randomization.Randomizers{
    /// <summary>
    /// Randomizes the rotation of objects tagged with a BrightnessRandomizerTag
    /// </summary>
    [Serializable]
    [AddRandomizerMenu("Perception/Brightness Randomizer")]
    public class BrightnessRandomizer : Randomizer
    {
        /// <summary>
        /// The range of random intensity to assign to target objects
        /// </summary>
        [Tooltip("The range of random rotations to assign to target objects.")]
        public FloatParameter brightness = new FloatParameter { value = new UniformSampler(0, 12)};


        /// <summary>
        /// Randomizes the rotation of tagged objects at the start of each scenario iteration
        /// </summary>
        protected override void OnIterationStart()
        {
            var tags = tagManager.Query<BrightnessRandomizerTag>();
            foreach (var tag in tags)
            {
                var light = tag.GetComponent<Light>();
                light.intensity = brightness.Sample();
            }
        }
    }
}

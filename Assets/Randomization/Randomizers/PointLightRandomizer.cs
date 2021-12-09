using System;
using UnityEngine.Perception.Randomization.Parameters;
using UnityEngine.Perception.Randomization.Randomizers.Tags;
using UnityEngine.Perception.Randomization.Samplers;

using Perc6d;
using UnityEngine.Perception.GroundTruth;

namespace UnityEngine.Perception.Randomization.Randomizers{
    /// <summary>
    /// Randomizes the rotation of objects tagged with a PointlightRandomizerTag(for simple scene)
    /// </summary>
    [Serializable]
    [AddRandomizerMenu("Perception/PointLightRandomizer")]
    public class PointLightRandomizer : Randomizer
    {
        /// <summary>
        /// The target object of Pointlight
        /// </summary>
        [Tooltip("The target object of light.")]
        public GameObject target;


        
        [Tooltip("The range of random relative distance to assign to light.")]
        public UniformSampler distance = new UniformSampler(0,3f);

<<<<<<< HEAD
=======
        [Tooltip("If x direction to assign to Pointlight positive.")]
        public bool direction = false;
>>>>>>> 5ee80fd98b88603357853981fc41df1fc2d87b50



        /// <summary>
        /// Randomizes the rotation and position of tagged objects at the start of each scenario iteration
        /// </summary>
        protected override void OnIterationStart()
        {

            var tags = tagManager.Query<PointLightRandomizerTag>();
            foreach (var tag in tags){
                Vector3 position = Random.onUnitSphere * distance.Sample();
                position.y = Math.Abs(position.y);
                if(tag.direction_x){
                    position.x = Math.Abs(position.x);
                }else{
                    position.x = -1 * Math.Abs(position.x);
                }
                if(tag.direction_z){
                    position.z = Math.Abs(position.z);
                }else{
                    position.z = -1 * Math.Abs(position.z);
                }
                tag.transform.position = target.transform.position + position;
            }


        }
        
    }
<<<<<<< HEAD
}
=======
}
>>>>>>> 5ee80fd98b88603357853981fc41df1fc2d87b50

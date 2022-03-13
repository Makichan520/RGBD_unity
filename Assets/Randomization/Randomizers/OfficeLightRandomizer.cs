using System;
using UnityEngine.Perception.Randomization.Parameters;
using UnityEngine.Perception.Randomization.Randomizers.Tags;
using UnityEngine.Perception.Randomization.Samplers;

using Perc6d;
using UnityEngine.Perception.GroundTruth;

namespace UnityEngine.Perception.Randomization.Randomizers{
    /// <summary>
    /// Randomizes the rotation of objects tagged with a PointlightRandomizerTag(for office scene)
    /// </summary>
    [Serializable]
    [AddRandomizerMenu("Perception/OfficeLightRandomizer")]
    public class OfficeLightRandomizer : Randomizer
    {
        /// <summary>
        /// The target object of Pointlight (Point light will move on a ceiling)
        /// </summary>
        [Tooltip("The center of target ceiling.")]
        public Vector3 targetCenter;


        /// <summary>
        /// The 2D size of the generated background layers(only useful in autoSet = false)
        /// </summary>
        [Tooltip("The width and length of the area in which the pointlights will be placed.(only useful in autoSet = false)")]
        public Vector2 placementArea;




        /// <summary>
        /// Randomizes the rotation and position of tagged objects at the start of each scenario iteration
        /// </summary>
        protected override void OnIterationStart()
        {

            var tags = tagManager.Query<PointLightRandomizerTag>();
            foreach (var tag in tags){
                Vector3 position = new Vector3(Random.value*placementArea.x/2 , 0 , Random.value*placementArea.y/2);
                Debug.Log("random relative pos: " + position);
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
                tag.transform.position = targetCenter + position;
            }


        }
        
    }
}
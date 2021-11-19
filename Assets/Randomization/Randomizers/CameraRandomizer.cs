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

        [Tooltip("The max random rounds of camera randomazition to capture all the objects.")]
        public int maxIterations = 40;

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

        private BooleanParameter pos_neg = new BooleanParameter();

        private Vector3 z_Position = Vector3.negativeInfinity;

        private Vector3 minPosition = Vector3.positiveInfinity;

        private Vector3 maxPosition = Vector3.negativeInfinity;


        /// <summary>
        /// Randomizes the rotation and position of tagged objects at the start of each scenario iteration
        /// </summary>
        protected override void OnIterationStart()
        {
            var objectTags = tagManager.Query<ObjectsRandomizerTag>();
            foreach (var tag in objectTags){
                if(tag.GetComponent<StaticController>().isStatic()){
                    Debug.Log("object position: " + tag.transform.position);
                    Vector3 pivot = tag.transform.position;
                    if(pivot.x > maxPosition.x){
                        maxPosition.x = pivot.x;
                         Debug.Log("sub x generating");
                    }
                    if(pivot.x < minPosition.x){
                        minPosition.x = pivot.x;
                        Debug.Log("sub x generating");
                    }
                    if(pivot.y > maxPosition.y){
                        maxPosition.y = pivot.y;
                        Debug.Log("sub y generating");
                    }
                    if(pivot.y < minPosition.y){
                        minPosition.y = pivot.y;
                        Debug.Log("sub y generating");
                    }
                    if(pivot.z > maxPosition.z){
                        maxPosition.z = pivot.z;
                        Debug.Log("sub y generating");
                    }
                    if(pivot.z < minPosition.z){
                        minPosition.z = pivot.z;
                        Debug.Log("sub y generating");
                    }
                }else{
                    maxPosition = z_Position;
                    minPosition = -z_Position;
                    return;
                }
            }

            Vector3 target_position = (maxPosition + minPosition)/2f;
            Debug.Log("max position: " + maxPosition + "\nmin position " + minPosition);

            var cameraTags = tagManager.Query<CameraRandomizerTag>();
            foreach (var tag in cameraTags){
                bool status = true;
                for(int i = 0; i < maxIterations; i++){
                    Vector3 offset = position.Sample() + (maxPosition-minPosition)/2f;
                    offset.Scale(scaleVector());
                    tag.transform.position = offset + target_position;
                    tag.transform.LookAt(target_position);
                    tag.transform.Rotate(0,0,rotation_z.Sample(),Space.Self);
                    foreach (var tag_1 in objectTags){
                        if(!tag_1.GetComponent<StaticController>().isRendering()){
                            status = false;
                            break;
                        }
                    }
                    if(!status){
                        continue;
                    }else{
                        // request camera capture
                        Debug.Log("Request camera capture...");
                        break;
                    }
                }
            }
        }
        
        /// <summary>
        /// Generate a Vector for scale.(in this case (±1,1，±1))
        /// </summary>
        private Vector3 scaleVector(){
            Vector3 result = Vector3.one;
            if(pos_neg.Sample()){
                result.x *= -1;
            }
            if(pos_neg.Sample()){
                result.z *= -1;
            }
            return result;
        }
    }
}

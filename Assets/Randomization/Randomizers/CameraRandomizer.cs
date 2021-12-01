using System;
using UnityEngine.Perception.Randomization.Parameters;
using UnityEngine.Perception.Randomization.Randomizers.Tags;
using UnityEngine.Perception.Randomization.Samplers;

using Perc6d;
using UnityEngine.Perception.GroundTruth;

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

        
        [Tooltip("The range of random relative distance to assign to camera.")]
        public UniformSampler distance = new UniformSampler(0,3f);

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
                    float radius = Vector3.Distance(maxPosition,minPosition)/2f;
                    Vector3 offset = Random.onUnitSphere * ( distance.Sample()+radius );
                    offset.y = Math.Abs(offset.y);
                    
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
                        RequestCapture(tag);
                        break;
                    }
                }
            }

            var tags = tagManager.Query<CameraRandomizerTag>();
            foreach (var tag in tags){
                Vector3 position = Random.onUnitSphere * distance.Sample();
                position.y = Math.Abs(position.y);
                tag.transform.position = target.transform.position + position;
                tag.transform.LookAt(target.transform.position);
                tag.transform.Rotate(0,0,rotation_z.Sample(),Space.Self);
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

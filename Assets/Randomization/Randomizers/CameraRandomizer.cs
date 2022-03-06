using System;
using UnityEngine.Perception.Randomization.Parameters;
using UnityEngine.Perception.Randomization.Randomizers.Tags;
using UnityEngine.Perception.Randomization.Samplers;

using Perc6d;
using UnityEngine.Perception.GroundTruth;
using UnityEditor;

namespace UnityEngine.Perception.Randomization.Randomizers{
    /// <summary>
    /// Randomizes the rotation and position of specific camera(should be a GameObject included Perception camera and Depth camera)
    /// </summary>
    [Serializable]
    [AddRandomizerMenu("Perception/Camera Randomizer")]
    public class CameraRandomizer : Randomizer
    {

        [Tooltip("Which frame that should start to be captured. Normally, startAtFrame = waitFrame*2(from backgroundRandomizer)")]
        public int startAtFrame = 300;

        /// <summary>
        /// The number of frames that should be captured between 2 background randomizations.
        /// </summary>
        [Tooltip("The number of frames that should be captured between 2 background randomizations.")]
        public int captureFrames = 500;

        /// <summary>
        /// The range of random rotations to assign to target objects
        /// </summary>
        [Tooltip("The range of random rotations to assign to target objects.")]
        public FloatParameter rotation_z = new FloatParameter { value = new UniformSampler(0,360) };

        
        [Tooltip("The range of random relative distance to assign to camera.")]
        public UniformSampler distance = new UniformSampler(0,3f);

        /// <summary>
        /// @params [z_Position、minPosition、maxPosition]The support parameter for calculate randomization spaces
        /// </summary>
        private Vector3 z_Position = Vector3.negativeInfinity;

        private Vector3 minPosition = Vector3.positiveInfinity;

        private Vector3 maxPosition = Vector3.negativeInfinity;

        // Counter for time of iteration, the startAtFrame works based on iterationCounter
        private int iterationCounter = 0;

        // Indicates whether the camera is ready to capture, or already has captured in this frame
        private bool status = true;

        private bool captured = false;

        [Tooltip("The camera that need to be randomized (should be a GameObject included Perception camera and Depth camera)")]
        public GameObject camera;

        [Tooltip("Target objects, which should be captured by cameras")]
        public GameObject[] targets;


        /// <summary>
        /// Randomizes the rotation and position of camera at the start of each scenario iteration
        /// </summary>
        protected override void OnIterationStart()
        {
            //Randomize only when the background is ready to be caputured
            if(iterationCounter++ < startAtFrame){
                return;
            }

            //Calculate randomize space according to targets objects
            var objectTags = tagManager.Query<ObjectsRandomizerTag>();
            foreach (var tag in objectTags){
                if(tag.GetComponent<StaticController>().isStatic()){
                    Vector3 pivot = tag.transform.position;
                    if(pivot.x > maxPosition.x){
                        maxPosition.x = pivot.x;
                    }
                    if(pivot.x < minPosition.x){
                        minPosition.x = pivot.x;

                    }
                    if(pivot.y > maxPosition.y){
                        maxPosition.y = pivot.y;

                    }
                    if(pivot.y < minPosition.y){
                        minPosition.y = pivot.y;

                    }
                    if(pivot.z > maxPosition.z){
                        maxPosition.z = pivot.z;

                    }
                    if(pivot.z < minPosition.z){
                        minPosition.z = pivot.z;

                    }
                }else{
                    maxPosition = z_Position;
                    minPosition = -z_Position;
                    return;
                }
            }
            
            Vector3 target_position = (maxPosition + minPosition)/2f;
            Debug.Log("max position: " + maxPosition + "\nmin position " + minPosition);
            
            //set captured = false, and randomizes position and rotation of camera
            captured = false;
            float radius = Vector3.Distance(maxPosition,minPosition)/2f;
            Vector3 offset = Random.onUnitSphere * ( distance.Sample()+radius );
            offset.y = Math.Abs(offset.y);
            
            //The camera should focus on the center of random space
            camera.transform.position = offset + target_position;
            camera.transform.LookAt(target_position);
            camera.transform.Rotate(0,0,rotation_z.Sample(),Space.Self);
/**
            foreach (var tag_1 in targets){
                if(Physics.Linecast(camera.transform.position,tag_1.transform.position,LayerMask.GetMask("Default"))){
                    Debug.Log("occlusion on " + tag_1.name + " in " + Time.frameCount + "frames, " + iterationCounter + " .counter");
                    status = false;
                    break;
                }
            }
            Debug.Log("in " + iterationCounter + " .iter is state: " + status.ToString());
            if(status == true){
                // request camera capture
                Debug.Log("Request camera capture in " + Time.frameCount + "frames, " + iterationCounter + " .counter");
                RequestCapture(camera);
            }
**/
            if(iterationCounter >= captureFrames + startAtFrame){
                iterationCounter = 0;
            }
        }
        
        /// <summary>
        /// After randomization, randomizer will check the status of camera and determine to request caputre
        /// </summary>
        protected override void OnUpdate(){
            status = true;
            if(captured){
                return;
            }
            foreach(var obj in targets){
                // check rendering status for targets
                if(!obj.GetComponent<StaticController>().isRendering()){
                    status = false;
                    Debug.Log("invisible on " + obj.name + " in " + Time.frameCount + "frames, " + iterationCounter + " .counter");
                    //break;
                }
                RaycastHit info;
                // use physic raycast to check the occlusion status of targets
                if(Physics.Linecast(camera.transform.position,obj.transform.position,out info)){
                    if(info.collider.name != obj.name){
                        Debug.Log("occlusion on " + obj.name + " in " + Time.frameCount + "frames, " + iterationCounter + " .counter");
                        status = false;
                        //break;
                    }
                }
            }
            if(status && !captured){
                Debug.Log("Request camera capture in " + Time.frameCount + "frames, " + iterationCounter + " .counter");
                RequestCapture(camera);
            }

        }
        
        /// <summary>
        /// Request camera to capture RGB image and Depth graph at same frame, show the message if capture success
        /// </summary>
        private int RequestCapture(GameObject cameras){
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
            captured = true;
            return 0;
        }
    }
}

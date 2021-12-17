using System;
using System.Linq;
using UnityEngine.Perception.Randomization.Parameters;
using UnityEngine.Perception.Randomization.Randomizers.Utilities;
using UnityEngine.Perception.Randomization.Samplers;
using System.Collections.Generic;

namespace UnityEngine.Perception.Randomization.Randomizers.SampleRandomizers
{
    /// <summary>
    /// Creates multiple layers of evenly distributed but randomly placed objects
    /// </summary>
    [Serializable]
    [AddRandomizerMenu("Perception/Background Randomizer")]
    public class BackgroundRandomizer : Randomizer
    {
        /// <summary>
        /// The number of frames that should be captured between 2 background randomizations.
        /// </summary>
        [Tooltip("The number of frames that should be captured between 2 background randomizations.")]
        public int captureFrames;


        /// <summary>
        /// The number of frames that should be captured between 2 background randomizations.
        /// </summary>
        [Tooltip("The number of frames that should be captured between 2 background randomizations.")]
        public int waitFrame;

        /// <summary>
        /// The Y offset component applied to all generated background layers
        /// </summary>
        [Tooltip("The Y offset applied to positions of all placed objects.")]
        public float depth;

        /// <summary>
        /// The number of background layers to generate. (>=2)
        /// </summary>
        [Tooltip("The number of background layers to generate. (>=2)")]
        public int layerCount = 2;

        /// <summary>
        /// The number of object above target to generate.
        /// </summary>
        [Tooltip("The number of object above target to generate.")]
        public int maxTopObject = 4;

        /// <summary>
        /// The minimum distance between placed background objects
        /// </summary>
        [Tooltip("The minimum distance between the centers of the placed objects.")]
        public float separationDistance = 2f;

        /// <summary>
        /// The minimum distance between placed background objects lower
        /// </summary>
        [Tooltip("The minimum distance between the centers of the placed objects lower.")]
        public float separationDistance_lower = 1f;

                /// <summary>
        /// The minimum distance between placed background objects lower
        /// </summary>
        [Tooltip("The minimum distance between the centers of the placed objects upper.")]
        public float separationDistance_upper = 1f;

        /// <summary>
        /// The 2D size of the generated background layers
        /// </summary>
        [Tooltip("The width and height of the area in which objects will be placed. These should be positive numbers and sufficiently large in relation with the Separation Distance specified.")]
        public Vector2 placementArea;

        /// <summary>
        /// The range of random positions to assign to target objects
        /// </summary>
        [Tooltip("The range of random positions to assign to target objects.")]
        public Vector2Parameter targetArea = new Vector2Parameter
        {
            x = new UniformSampler(0, 360),
            y = new UniformSampler(0, 360)
        };



        /// <summary>
        /// The range of random rotations to assign to target and upper objects
        /// </summary>
        [Tooltip("The range of random positions to assign to target and upper objects.")]
        public Vector3Parameter rotation = new Vector3Parameter
        {
            x = new UniformSampler(0, 360),
            y = new UniformSampler(0, 360),
            z = new UniformSampler(0, 360)
        };

        /// <summary>
        /// A categorical parameter for sampling random prefabs to place
        /// </summary>
        [Tooltip("The list of Prefabs to be placed by this Randomizer.Which putted on the ground.")]
        public GameObjectParameter prefabsLower;

        [Tooltip("The list of Prefabs to be placed by this Randomizer.Above the prefebsLower.")]
        public GameObjectParameter prefabsUpper;

        [Tooltip("The list of target object to be placed by this Randomizer.Above the prefebsUpper.")]
        public GameObject[] targets;

        GameObject m_Container_lower;
        GameObject m_Container_upper;
        GameObjectOneWayCache m_GameObjectOneWayCache_1;
        GameObjectOneWayCache m_GameObjectOneWayCache_2;
        List<GameObject> topList = new List<GameObject>();

        int interationCounter = 0; 

        /// <inheritdoc/>
        protected override void OnAwake()
        {
            m_Container_lower = new GameObject("BackgroundContainer_L");
            m_Container_lower.transform.parent = scenario.transform;
            m_Container_upper = new GameObject("BackgroundContainer_U");
            m_Container_upper.transform.parent = scenario.transform;
            m_GameObjectOneWayCache_1 = new GameObjectOneWayCache(
                m_Container_lower.transform, prefabsLower.categories.Select((element) => element.Item1).ToArray());
            m_GameObjectOneWayCache_2 = new GameObjectOneWayCache(
                m_Container_upper.transform, prefabsUpper.categories.Select((element) => element.Item1).ToArray());
        }

        /// <summary>
        /// Generates background layers of objects at the start of each scenario iteration
        /// </summary>
        protected override void OnIterationStart()
        {
            int counter = interationCounter % (captureFrames+waitFrame);
            if(counter == 0){
                var seed_0 = SamplerState.NextRandomState();
                var placementSamples_0 = PoissonDiskSampling.GenerateSamples(
                    placementArea.x, placementArea.y, separationDistance_lower, seed_0);
                var offset_0 = new Vector3(placementArea.x, 0f, placementArea.y) * -0.5f;
                topList.Clear();
                foreach (var sample in placementSamples_0)
                {
                    var instance = m_GameObjectOneWayCache_1.GetOrInstantiate(prefabsLower.Sample());
                    instance.transform.position = new Vector3(sample.x, depth ,sample.y) + offset_0;
                    UniformSampler y_sample = new UniformSampler(0,360);
                    var rigid = instance.GetComponent<Rigidbody>();
                    rigid.velocity = Vector3.zero;
                    rigid.angularVelocity = Vector3.zero;
                    var info = instance.GetComponent<LowerInfos>();
                    instance.transform.rotation = Quaternion.Euler(info.rotate_euler.x,y_sample.Sample(),info.rotate_euler.z);
                    if(info.isTop){
                        topList.Add(instance);
                    }
                }
                placementSamples_0.Dispose();

                for (var i = 1; i < layerCount; i++)
                {
                    var seed = SamplerState.NextRandomState();
                    var placementSamples = PoissonDiskSampling.GenerateSamples(
                        placementArea.x, placementArea.y, separationDistance_upper, seed);
                    var offset = new Vector3(placementArea.x, 0f, placementArea.y) * -0.5f;
                    foreach (var sample in placementSamples)
                    {
                        var instance = m_GameObjectOneWayCache_2.GetOrInstantiate(prefabsUpper.Sample());
                        instance.transform.position = new Vector3(sample.x, separationDistance * i + depth, sample.y) + offset;
                        instance.transform.rotation = Quaternion.Euler(rotation.Sample());
                        var rigid = instance.GetComponent<Rigidbody>();
                        rigid.velocity = Vector3.zero;
                        rigid.angularVelocity = Vector3.zero;
                    }
                    placementSamples.Dispose();

                }
            }
            if(counter == waitFrame){
                UniformSampler index_sample = new UniformSampler(0,topList.Count-0.1f);
                int index = (int)Math.Abs(index_sample.Sample());
                GameObject targetTop = topList.ElementAt(index);
                var infos = targetTop.GetComponent<LowerInfos>();
                infos.LogInfos();
                foreach(var obj in targets){
                    var rot_y = targetTop.transform.eulerAngles.y;
                    Vector3 pos_xz = Quaternion.Euler(0,rot_y,0)*infos.area.Sample() + targetTop.transform.position;
                    //TODO: change the sample area to fit the rotation of target top
                    obj.transform.position = new Vector3(pos_xz.x, pos_xz.y + 0.5f, pos_xz.z);
                    obj.transform.rotation = Quaternion.Euler(rotation.Sample());
                    var rigid = obj.GetComponent<Rigidbody>();
                    rigid.velocity = Vector3.zero;
                    rigid.angularVelocity = Vector3.zero;
                }
                int count = -1;
                if(maxTopObject != 0){
                    count = (int)new UniformSampler(0,maxTopObject + 0.99f).Sample();
                    Debug.Log("gen " + count + " objects...");
                }
                for(int i=1; i <= count;i++){
                    Debug.Log("Generate obj above top");
                    var rot_y = targetTop.transform.eulerAngles.y;
                    Vector3 sample = Quaternion.Euler(0,rot_y,0)*infos.area.Sample() + targetTop.transform.position;
                    var instance = m_GameObjectOneWayCache_2.GetOrInstantiate(prefabsUpper.Sample());
                    instance.transform.position = new Vector3(sample.x, sample.y + separationDistance, sample.y);
                    instance.transform.rotation = Quaternion.Euler(rotation.Sample());
                    var rigid = instance.GetComponent<Rigidbody>();
                    rigid.velocity = Vector3.zero;
                    rigid.angularVelocity = Vector3.zero;
                }
            }

            interationCounter++;
        }

        /// <summary>
        /// Deletes generated background objects after each scenario iteration is complete
        /// </summary>
        protected override void OnIterationEnd()
        {
            int counter = interationCounter % (waitFrame+captureFrames);
            if(counter == waitFrame+captureFrames-1 ){
                m_GameObjectOneWayCache_1.ResetAllObjects();
                m_GameObjectOneWayCache_2.ResetAllObjects();
            }
        }

    }
}

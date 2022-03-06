using System;
using System.Linq;
using UnityEngine.Perception.Randomization.Parameters;
using UnityEngine.Perception.Randomization.Randomizers.Utilities;
using UnityEngine.Perception.Randomization.Samplers;

namespace UnityEngine.Perception.Randomization.Randomizers.SampleRandomizers
{
    /// <summary>
    /// Creates multiple layers of evenly distributed but randomly placed objects
    /// </summary>
    [Serializable]
    [AddRandomizerMenu("Perception/Background Randomizer")]
    public class SimpleBackgroundRandomizer : Randomizer
    {
        /// <summary>
        /// The number of frames that should be captured between 2 background randomizations.
        /// </summary>
        [Tooltip("The number of frames that should be captured between 2 background randomizations.")]
        public int captureFrames;


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
        /// The minimum distance between placed background objects
        /// </summary>
        [Tooltip("The minimum distance between the centers of the placed objects.")]
        public float separationDistance = 2f;

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
        /// The range of random rotations to assign to target objects
        /// </summary>
        [Tooltip("The range of random positions to assign to target objects.")]
        public Vector3Parameter rotation = new Vector3Parameter
        {
            x = new UniformSampler(0, 360),
            y = new UniformSampler(0, 360),
            z = new UniformSampler(0,360)
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

        GameObject m_Container;
        GameObjectOneWayCache m_GameObjectOneWayCache_1;
        GameObjectOneWayCache m_GameObjectOneWayCache_2;

        int interationCounter = 0; 

        /// <inheritdoc/>
        protected override void OnAwake()
        {
            m_Container = new GameObject("BackgroundContainer");
            m_Container.transform.parent = scenario.transform;
            m_GameObjectOneWayCache_1 = new GameObjectOneWayCache(
                m_Container.transform, prefabsLower.categories.Select((element) => element.Item1).ToArray());
            m_GameObjectOneWayCache_2 = new GameObjectOneWayCache(
                m_Container.transform, prefabsUpper.categories.Select((element) => element.Item1).ToArray());
        }

        /// <summary>
        /// Generates background layers of objects at the start of each scenario iteration
        /// </summary>
        protected override void OnIterationStart()
        {
            int counter = interationCounter % captureFrames;
            if(counter == 0){

                var seed_0 = SamplerState.NextRandomState();
                var placementSamples_0 = PoissonDiskSampling.GenerateSamples(
                    placementArea.x, placementArea.y, separationDistance, seed_0);
                var offset_0 = new Vector3(placementArea.x, 0f, placementArea.y) * -0.5f;
                foreach (var sample in placementSamples_0)
                {
                    var instance = m_GameObjectOneWayCache_1.GetOrInstantiate(prefabsLower.Sample());
                    instance.transform.position = new Vector3(sample.x, depth ,sample.y) + offset_0;
                    instance.transform.rotation = new Quaternion(0,0,0,1);
                    var rigid = instance.GetComponent<Rigidbody>();
                    rigid.velocity = Vector3.zero;
                    rigid.angularVelocity = Vector3.zero;
                }
                placementSamples_0.Dispose();

                for (var i = 1; i < layerCount; i++)
                {
                    var seed = SamplerState.NextRandomState();
                    var placementSamples = PoissonDiskSampling.GenerateSamples(
                        placementArea.x, placementArea.y, separationDistance, seed);
                    var offset = new Vector3(placementArea.x, 0f, placementArea.y) * -0.5f;
                    foreach (var sample in placementSamples)
                    {
                        var instance = m_GameObjectOneWayCache_2.GetOrInstantiate(prefabsUpper.Sample());
                        instance.transform.position = new Vector3(sample.x, separationDistance * i + depth, sample.y) + offset;
                        instance.transform.rotation = new Quaternion(0,0,0,1);
                        var rigid = instance.GetComponent<Rigidbody>();
                        rigid.velocity = Vector3.zero;
                        rigid.angularVelocity = Vector3.zero;
                    }
                    placementSamples.Dispose();

                }
                
                foreach(var obj in targets){
                    Vector2 pos_xz = targetArea.Sample();
                    obj.transform.position = new Vector3(pos_xz.x, depth + 1 + separationDistance*(layerCount-1),pos_xz.y);
                    obj.transform.rotation = Quaternion.Euler(rotation.Sample());
                    var rigid = obj.GetComponent<Rigidbody>();
                    rigid.velocity = Vector3.zero;
                    rigid.angularVelocity = Vector3.zero;
                }
            }
        }

        /// <summary>
        /// Deletes generated background objects after each scenario iteration is complete
        /// </summary>
        protected override void OnIterationEnd()
        {
            m_GameObjectOneWayCache_1.ResetAllObjects();
            m_GameObjectOneWayCache_2.ResetAllObjects();
        }

        protected override void OnUpdate()
        {
            base.OnUpdate();
        }
    }
}

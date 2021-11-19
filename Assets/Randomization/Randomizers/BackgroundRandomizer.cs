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
    public class BackgroundRandomizer : Randomizer
    {
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
        /// A categorical parameter for sampling random prefabs to place
        /// </summary>
        [Tooltip("The list of Prefabs to be placed by this Randomizer.Which putted on the ground.")]
        public GameObjectParameter prefabsLower;

        [Tooltip("The list of Prefabs to be placed by this Randomizer.Above the prefebsLower.")]
        public GameObjectParameter prefabsUpper;

        GameObject m_Container;
        GameObjectOneWayCache m_GameObjectOneWayCache_1;
        GameObjectOneWayCache m_GameObjectOneWayCache_2;

        /// <inheritdoc/>
        protected override void OnAwake()
        {
            m_Container = new GameObject("BackgroundContainer");
            m_Container.transform.parent = scenario.transform;
            m_GameObjectOneWayCache_1 = new GameObjectOneWayCache(
                m_Container.transform, prefabsLower.categories.Select((element) => element.Item1).ToArray());
            m_GameObjectOneWayCache_2 = new GameObjectOneWayCache(
                m_Container.transform, prefabsUpper.categories.Select((element) => element.Item1).ToArray());
            Debug.Log("Background OnAwake");
        }

        /// <summary>
        /// Generates background layers of objects at the start of each scenario iteration
        /// </summary>
        protected override void OnIterationStart()
        {
            Debug.Log("Background Iteration----");
            var seed_0 = SamplerState.NextRandomState();
            var placementSamples_0 = PoissonDiskSampling.GenerateSamples(
                placementArea.x, placementArea.y, separationDistance, seed_0);
            var offset_0 = new Vector3(placementArea.x, 0f, placementArea.y) * -0.5f;
            foreach (var sample in placementSamples_0)
            {
                var instance = m_GameObjectOneWayCache_1.GetOrInstantiate(prefabsLower.Sample());
                instance.transform.position = new Vector3(sample.x, depth ,sample.y) + offset_0;
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
                }
                placementSamples.Dispose();
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
    }
}

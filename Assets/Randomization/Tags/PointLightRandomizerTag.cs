namespace UnityEngine.Perception.Randomization.Randomizers.Tags
{
    /// <summary>
    /// Used in conjunction with a PointLightRandomizer to vary the position of PointLight.
    /// </summary>
    [AddComponentMenu("Perception/RandomizerTags/PointLightRandomizerTag")]
    public class PointLightRandomizerTag : RandomizerTag {
        public bool direction_x = false;
        public bool direction_z = false;
    }
}
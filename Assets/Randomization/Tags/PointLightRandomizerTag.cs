namespace UnityEngine.Perception.Randomization.Randomizers.Tags
{
    /// <summary>
    /// Used in conjunction with a PointLightRandomizer to vary the position of PointLight.
    /// </summary>
    [AddComponentMenu("Perception/RandomizerTags/PointLightRandomizerTag")]
    public class PointLightRandomizerTag : RandomizerTag {
        // Point lights are randomly moved to positions in a specific direction
        ///<@param> specifies the position of the light source relative to the target on corresponding axis
        public bool direction_x = false;
        public bool direction_z = false;
    }
}

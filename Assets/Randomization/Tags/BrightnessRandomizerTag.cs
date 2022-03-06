namespace UnityEngine.Perception.Randomization.Randomizers.Tags
{
    /// <summary>
    /// Used in conjunction with a BrightnessRandomizer to vary the intensity of Light
    /// </summary>
    [AddComponentMenu("Perception/RandomizerTags/Brightness Randomizer Tag")]
    [RequireComponent(typeof(Light))]
    public class BrightnessRandomizerTag : RandomizerTag { }
}

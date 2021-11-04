namespace UnityEngine.Perception.Randomization.Randomizers.Tags
{
    /// <summary>
    /// Used in conjunction with a RotationRandomizer to vary the rotation of GameObjects
    /// </summary>
    [AddComponentMenu("Perception/RandomizerTags/Brightness Randomizer Tag")]
    [RequireComponent(typeof(Light))]
    public class BrightnessRandomizerTag : RandomizerTag { }
}

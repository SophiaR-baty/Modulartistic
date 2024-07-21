namespace Modulartistic.Core
{
    /// <summary>
    /// Represents the different properties available in a <see cref="State"/> configuration.
    /// </summary>
    /// <remarks>
    /// This enumeration defines various properties that can be used to configure a <see cref="State"/>. 
    /// Each member corresponds to a specific property, including position, rotation, color attributes, and additional parameters.
    /// The members are used for indexing and referring to <see cref="State"/> properties in a structured manner.
    /// </remarks>
    public enum StateProperty : int
    {
        X0,
        Y0,
        XRotationCenter,
        YRotationCenter,
        XFactor,
        YFactor,
        Rotation,

        Mod,
        ModLowerLimit,
        ModUpperLimit,


        ColorRedHue,
        ColorGreenSaturation,
        ColorBlueValue,
        InvalidColorRedHue,
        InvalidColorGreenSaturation,
        InvalidColorBlueValue,

        ColorFactorRedHue,
        ColorFactorGreenSaturation,
        ColorFactorBlueValue,

        ColorAlpha,
        InvalidColorAlpha,

        i0, i1, i2, i3, i4, i5, i6, i7, i8, i9
    }
}

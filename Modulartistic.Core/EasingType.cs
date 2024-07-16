using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modulartistic.Core
{
    /// <summary>
    /// Types of easing
    /// </summary>
    public enum EasingType
    {
        // basic
        Linear,
        Multiplicative,
        
        // sine
        SineIn,
        SineOut,
        SineInOut,

        // quadratic
        QuadIn,
        QuadOut,
        QuadInOut,

        // cubic
        CubicIn,
        CubicOut,
        CubicInOut,
        
        // quartic
        QuartIn,
        QuartOut,
        QuartInOut,

        // quintic
        QuintIn,
        QuintOut,
        QuintInOut,

        // exponential
        ExpoIn,
        ExpoOut,
        ExpoInOut,

        // circle
        CircIn,
        CircOut,
        CircInOut,

        // back
        BackIn,
        BackOut,
        BackInOut,

        // elastic
        ElasticIn,
        ElasticOut,
        ElasticInOut,

        // bounce
        BounceIn,
        BounceOut,
        BounceInOut
    }
}

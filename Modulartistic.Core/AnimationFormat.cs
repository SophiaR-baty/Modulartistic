using System;
using System.Collections.Generic;
using System.Text;

namespace Modulartistic.Core
{
    /// <summary>
    /// Specifies the formats for animations.
    /// </summary>
    public enum AnimationFormat
    {
        /// <summary>
        /// No animation format is selected.
        /// </summary>
        None = 0,

        /// <summary>
        /// Graphics Interchange Format (GIF), suitable for simple animations with limited colors.
        /// </summary>
        Gif = 1,

        /// <summary>
        /// MPEG-4 Part 14 (MP4), a widely used format for video and higher-quality animations.
        /// </summary>
        Mp4 = 2,
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace Modulartistic
{
    public enum ErrorCode
    {
        Success = 0,
        Help = -1,
        UnexpectedArgument = -2,
        JsonParsingError = -3,
        GenerationError = -4,
    }
}

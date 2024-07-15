using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Modulartistic.Core.Tests
{
    public class StateTests
    {
        [Fact]
        public void Test()
        {
            GenerationOptions options = new GenerationOptions()
            {
                Height = 10,
                Width = 10,
                UseRGB = false,
                Circular = false,
                InvalidColorGlobal = true,
            };
            State state = new State();
            state.
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Modulartistic.Core;
using Xunit;

namespace Modulartistic.Core.Tests
{
    public class HelperTests
    {
        [Theory]
        [InlineData(5, 2, 1)]
        [InlineData(5, 3, 2)]
        [InlineData(5, 5, 0)]
        [InlineData(5, 1, 0)]
        [InlineData(4.2, 2, 0.2)]
        [InlineData(4.2, 3, 1.2)]
        [InlineData(5, 4.2, 0.8)]
        [InlineData(3, 4.2, 3)]
        [InlineData(10, 4.2, 1.6)]
        [InlineData(4.2, 4.2, 0)]
        [InlineData(0, 4.2, 0)]
        [InlineData(-3, 5, 2)]
        [InlineData(-2, 6, 4)]
        public void ModTest(double a, double b, double res)
        {
            Assert.Equal(res, Helper.mod(a, b), 0.00000001);
        }

        [Theory]
        [InlineData(5, 2, 1)]
        [InlineData(5, 3, 2)]
        [InlineData(5, 5, 5)]
        [InlineData(5, 1, 1)]
        [InlineData(4.2, 2, 0.2)]
        [InlineData(4.2, 3, 1.2)]
        [InlineData(5, 4.2, 0.8)]
        [InlineData(3, 4.2, 3)]
        [InlineData(10, 4.2, 1.6)]
        [InlineData(4.2, 4.2, 4.2)]
        [InlineData(0, 4.2, 0)]
        [InlineData(-3, 5, 2)]
        [InlineData(-2, 6, 4)]
        public void InclusiveModTest(double a, double b, double res)
        {
            Assert.Equal(res, Helper.inclusiveMod(a, b), 0.00000001);
        }

        [Theory]
        [InlineData(5, 2, 1)]
        [InlineData(5, 3, 1)]
        [InlineData(3, 3, 3)]
        [InlineData(5, 5, 5)]
        [InlineData(5, 1, 1)]
        [InlineData(4.2, 2, 0.2)]
        [InlineData(4.2, 3, 1.8)]
        [InlineData(5, 4.2, 3.4)]
        public void CircTest(double a, double b, double res)
        {
            Assert.Equal(res, Helper.circ(a, b), 0.00000001);
        }

        [Theory]
        [InlineData(1, 0)]
        [InlineData(2, 0)]
        [InlineData(3, 0)]
        [InlineData(2.4, 0)]
        [InlineData(3.5, -1)]
        [InlineData(-8.3, -1.5)]
        [InlineData(0, -0.5)]
        public void ModZeroThrowsException(double a, double b)
        {
            Assert.Throws<DivideByZeroException>(() => Helper.mod(a, b));
        }
    }
}

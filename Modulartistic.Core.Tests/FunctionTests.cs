using Modulartistic.Core;
using Xunit;

namespace Modulartistic.Core.Tests
{
    public class FunctionTests
    {

        [Theory]
        [InlineData("x*y", 2, 3, 6)]
        [InlineData("x*y", 4, 4, 16)]
        [InlineData("x*y", -4, 3, -12)]
        [InlineData("x*y", -2, -5, 10)]
        [InlineData("x*y", 0.5, 10, 5)]
        [InlineData("x*y", 0.01, 100, 1)]
        [InlineData("x*y", 0.01, 0.5, 0.005)]
        [InlineData("x+y", 1, 1, 2)]
        [InlineData("x+y", 3, 1, 4)]
        [InlineData("x+y", -3, 2, -1)]
        [InlineData("x*x+2*x*y+y*y", 2, 4, 36)]
        [InlineData("x*x+2*x*y+y*y", 5, 7, 144)]
        [InlineData("x*x+2*x*y+y*y", 1, 1, 4)]
        [InlineData("12", 1, 3, 12)]
        [InlineData("12", 100000000, 31, 12)]
        [InlineData("12", 0.00000000001, 0.000000003, 12)]
        [InlineData("x+y", 1000000000000, 1000000000000, 2000000000000)]
        public void ExpressionTest(string expression, double x, double y, double expected)
        {
            Function f = new Function(expression);
            Assert.Equal(expected, f.Evaluate(x, y), 0.00001);
        }
    }
}

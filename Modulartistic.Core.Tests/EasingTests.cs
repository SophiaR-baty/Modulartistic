using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Modulartistic.Core;

namespace Modulartistic.Core.Tests
{
    public class EasingTests
    {
        [Theory]
        [MemberData(nameof(EasingTypeEnumData.GetEnumValues), MemberType = typeof(EasingTypeEnumData))]
        public void FromEasingType_GetsCorrectEasingType(EasingType type)
        {
            Easing easing = Easing.FromType(type);

            Assert.Equal(type, easing.Type);
        }
    }

    internal class EasingTypeEnumData
    {
        public static IEnumerable<object[]> GetEnumValues()
        {
            foreach (var value in Enum.GetValues(typeof(EasingType)))
            {
                yield return new object[] { value };
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Modulartistic.Core
{
    public class IndexableBase
    {
        /// <summary>
        /// Gets or Sets a Property using its name, using nameof is recommended
        /// </summary>
        /// <param name="propertyName">The name of the Property</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">If the propertyName was not recognized</exception>
        [JsonIgnore]
        public object? this[string propertyName]
        {
            get
            {
                PropertyInfo? prop = GetType().GetProperty(propertyName);
                if (prop is null)
                {
                    throw new ArgumentException($"Property '{propertyName}' does not exist on type '{GetType().Name}'.");
                }
                return prop.GetValue(this);
            }

            set
            {
                PropertyInfo? prop = GetType().GetProperty(propertyName);
                if (prop is null)
                {
                    throw new ArgumentException($"Property '{propertyName}' does not exist on type '{GetType().Name}'.");
                }
                prop.SetValue(this, value);
            }
        }
    }
}

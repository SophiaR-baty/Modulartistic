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
    /// <summary>
    /// Provides a base class with indexer support for accessing and modifying properties by their name.
    /// </summary>
    /// <remarks>
    /// The <see cref="IndexableBase"/> class allows derived classes to access their properties using a string indexer.
    /// This can be useful for dynamic property access, such as in scenarios involving JSON serialization or dynamic object manipulation.
    /// It provides a way to get or set property values using the property name, which should be specified as a string.
    /// </remarks>
    public class IndexableBase
    {
        /// <summary>
        /// Gets or sets the value of a property identified by its name.
        /// </summary>
        /// <param name="propertyName">The name of the property to get or set.</param>
        /// <returns>The value of the property if getting; otherwise, sets the value of the property if setting.</returns>
        /// <exception cref="ArgumentException">Thrown when the specified <paramref name="propertyName"/> does not correspond to a valid property of the current type.</exception>
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

using Modulartistic.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using System.Threading.Tasks;

namespace Modulartistic.AudioGeneration
{
    public static class Schema
    {
        public static JsonObject GetGenerationDataSchemaObject()
        {
            // set general structure of Schema
            JsonObject schema = new JsonObject()
            {
                ["$schema"] = "https://json-schema.org/draft/2020-12/schema",
                ["$id"] = "http://modulartistic.com/",

                ["$defs"] = new JsonObject() { },

                ["type"] = "object",
                ["properties"] = new JsonObject()
                {
                    ["State"] = new JsonObject()
                    {
                        ["$ref"] = $"#/$defs/{nameof(State)}"
                    },
                    ["Options"] = new JsonObject()
                    {
                        ["$ref"] = $"#/$defs/{nameof(StateOptions)}"
                    },
                },
                ["additionalProperties"] = false
            };

            JsonObject defs = schema["$defs"].AsObject();
            Type currentType;

            #region Add StateOptions definitions to schema
            currentType = typeof(StateOptions);
            defs[$"{currentType.Name}"] = new JsonObject()
            {
                ["$comment"] = $"{currentType.Name} object",
                ["type"] = "object",
                ["properties"] = new JsonObject(),
                ["additionalProperties"] = false,
            };
            foreach (PropertyInfo propInf in currentType.GetProperties().Where((prop) => !prop.IsDefined(typeof(JsonIgnoreAttribute))))
            {
                // Functions (RH, GS, BV, alpha)
                if (propInf.PropertyType == typeof(string))
                {
                    defs[$"{currentType.Name}"]["properties"].AsObject().Add(propInf.Name, new JsonObject() { ["type"] = "string" });
                }
                // Width, Height, Framerate
                else if (propInf.PropertyType == typeof(int) || propInf.PropertyType == typeof(uint))
                {
                    defs[$"{currentType.Name}"]["properties"].AsObject().Add(propInf.Name,
                        new JsonObject()
                        {
                            ["anyOf"] = new JsonArray()
                            {
                                new JsonObject() { ["type"] = "integer", ["minimum"] = 1 },
                                new JsonObject() { ["type"] = "string" },
                            }
                        });
                }
                // InvalidColorGlobal, CircularMod, UseRGB
                else if (propInf.PropertyType == typeof(bool))
                {
                    defs[$"{currentType.Name}"]["properties"].AsObject().Add(propInf.Name, new JsonObject() { ["type"] = "boolean" });
                }
                // AddOns
                else if (propInf.PropertyType == typeof(List<string>))
                {
                    defs[$"{currentType.Name}"]["properties"].AsObject().Add(propInf.Name,
                        new JsonObject()
                        {
                            ["type"] = "array",
                            ["items"] = new JsonObject() { ["type"] = "string" }
                        }
                        );
                }
                // in case of changes that aren't being handled well
                else { throw new Exception($"{currentType.Name} should not contain Serializable Properties of type {propInf.PropertyType.Name}. You might be missing a JsonIgnore Attribute or a case isn't handled. "); }
            }
            #endregion

            #region Add State definitions to schema
            currentType = typeof(State);
            defs[$"{currentType.Name}"] = new JsonObject()
            {
                ["$comment"] = $"{currentType.Name} object",
                ["type"] = "object",
                ["properties"] = new JsonObject(),
                ["additionalProperties"] = false,
            };
            foreach (PropertyInfo propInf in currentType.GetProperties().Where((prop) => !prop.IsDefined(typeof(JsonIgnoreAttribute))))
            {
                // Name
                if (propInf.PropertyType == typeof(string))
                {
                    defs[$"{currentType.Name}"]["properties"].AsObject().Add(propInf.Name, new JsonObject() { ["type"] = "string" });
                }
                // double Properties
                else if (propInf.PropertyType == typeof(double))
                {
                    defs[$"{currentType.Name}"]["properties"].AsObject().Add(propInf.Name,
                        new JsonObject()
                        {
                            ["anyOf"] = new JsonArray()
                            {
                                new JsonObject() { ["type"] = "number" },
                                new JsonObject() { ["type"] = "string" }
                            }
                        });
                }
                // Parameters
                else if (propInf.PropertyType == typeof(double[]))
                {
                    defs[$"{currentType.Name}"]["properties"].AsObject().Add(propInf.Name,
                        new JsonObject()
                        {
                            ["type"] = "array",
                            ["items"] = new JsonObject()
                            {
                                ["anyOf"] = new JsonArray()
                                {
                                    new JsonObject() { ["type"] = "number" },
                                    new JsonObject() { ["type"] = "string" }
                                },
                                ["maxItems"] = 10
                            }
                        });
                }
                else { throw new Exception($"{currentType.Name} should not contain Serializable Properties of type {propInf.PropertyType.Name}. You might be missing a JsonIgnore Attribute or a case isn't handled. "); }
            }

            #endregion

            #region Add EasingType definitions to schema
            currentType = typeof(EasingType);
            defs[$"{currentType.Name}"] = new JsonObject()
            {
                ["$comment"] = $"{currentType.Name} enum",
                ["enum"] = new JsonArray(),
            };
            foreach (EasingType easingType in Enum.GetValues(typeof(EasingType)))
            {
                defs[$"{currentType.Name}"]["enum"].AsArray().Add(easingType.ToString());
            }
            #endregion

            return schema;
        }

        public static string GetGenerationDataSchema()
        {
            var options = new JsonSerializerOptions
            {
                TypeInfoResolver = new DefaultJsonTypeInfoResolver(),
                WriteIndented = true
            };

            return GetGenerationDataSchemaObject().ToJsonString(new JsonSerializerOptions(options));
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using Json.Schema;
using System.Reflection;
using System.Text.Json.Serialization.Metadata;
using System.Reflection.Metadata;

namespace Modulartistic.Core
{
    /// <summary>
    /// Provides methods for generating JSON schemas related to generation and validation of <see cref="GenerationData"/> and State objects (<see cref="StateOptions"/>, <see cref="State"/>, <see cref="StateSequence"/>,<see cref="Scene"/>, <see cref="StateTimeline"/>, <see cref="StateEvent"/>)
    /// </summary>
    public static class Schemas
    {
        /// <summary>
        /// Generates a JSON schema object for <see cref="GenerationData"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="JsonObject"/> representing the schema for <see cref="GenerationData"/>..
        /// </returns>
        public static JsonObject GetGenerationDataSchemaObject()
        {
            // set general structure of Schema
            JsonObject schema = new JsonObject() {
                ["$schema"] = "https://json-schema.org/draft/2020-12/schema",
                ["$id"] = "http://modulartistic.com/",

                ["$defs"] = new JsonObject() { },

                ["type"] = "array",
                ["items"] = new JsonObject() {
                    ["oneOf"] = new JsonArray() { },
                },
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
                            ["items"] = new JsonObject() { ["type"] = "string" } }
                        );
                }
                // Function Parameters
                else if (propInf.PropertyType == typeof(List<StateOptionsParameter>))
                {
                    defs[$"{currentType.Name}"]["properties"].AsObject().Add(propInf.Name,
                        new JsonObject()
                        {
                            ["type"] = "array",
                            ["items"] = new JsonObject() { 
                                ["type"] = "object",
                                ["properties"] = new JsonObject {
                                    [nameof(StateOptionsParameter.Name)] = new JsonObject() { ["type"] = "string" },
                                    [nameof(StateOptionsParameter.Expression)] = new JsonObject() { ["type"] = "string" },
                                    [nameof(StateOptionsParameter.Evaluation)] = new JsonObject() 
                                    { 
                                        ["type"] = "string", 
                                        ["enum"] = new JsonArray() { "global", "state", "pixel", "auto" } 
                                    },
                                }
                            }
                        });
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

            #region Add StateSequence definitions to schema
            currentType = typeof(StateSequence);
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
                // Scenes
                else if (propInf.PropertyType == typeof(List<Scene>))
                {
                    defs[$"{currentType.Name}"]["properties"].AsObject().Add(propInf.Name,
                        new JsonObject()
                        {
                            ["type"] = "array",
                            ["items"] = new JsonObject() { ["$ref"] = $"#/$defs/{nameof(Scene)}" },
                        });
                }
                // in case of changes that aren't being handled well
                else { throw new Exception($"{currentType.Name} should not contain Serializable Properties of type {propInf.PropertyType.Name}. You might be missing a JsonIgnore Attribute or a case isn't handled. "); }
            }
            #endregion

            #region Add Scene definitions to schema
            currentType = typeof(Scene);
            defs[$"{currentType.Name}"] = new JsonObject()
            {
                ["$comment"] = $"{currentType.Name} object",
                ["type"] = "object",
                ["properties"] = new JsonObject(),
                ["additionalProperties"] = false,
            };
            foreach (PropertyInfo propInf in currentType.GetProperties().Where((prop) => !prop.IsDefined(typeof(JsonIgnoreAttribute))))
            {
                // State
                if (propInf.PropertyType == typeof(State))
                {
                    defs[$"{currentType.Name}"]["properties"].AsObject().Add(propInf.Name, new JsonObject() { ["$ref"] = $"#/$defs/{nameof(State)}" });
                }
                // Length
                else if (propInf.PropertyType == typeof(double))
                {
                    defs[$"{currentType.Name}"]["properties"].AsObject().Add(propInf.Name,
                        new JsonObject()
                        {
                            ["type"] = "number",
                            ["minimum"] = 0
                        });
                }
                // EasingType
                else if (propInf.PropertyType == typeof(EasingType))
                {
                    defs[$"{currentType.Name}"]["properties"].AsObject().Add(propInf.Name,
                        new JsonObject()
                        {
                            ["$ref"] = $"#/$defs/{nameof(EasingType)}"
                        });
                }
                // in case of changes that aren't being handled well
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

            #region Add StateEvent definitions to schema
            currentType = typeof(StateEvent);
            defs[$"{currentType.Name}"] = new JsonObject()
            {
                ["$comment"] = $"{currentType.Name} object",
                ["type"] = "object",
                ["properties"] = new JsonObject(),
                ["additionalProperties"] = false,
            };
            foreach (PropertyInfo propInf in currentType.GetProperties().Where((prop) => !prop.IsDefined(typeof(JsonIgnoreAttribute))))
            {
                // times (uint values)
                if (propInf.PropertyType == typeof(uint))
                {
                    defs[$"{currentType.Name}"]["properties"].AsObject().Add(propInf.Name,
                        new JsonObject()
                        {
                            ["type"] = "number",
                            ["minimum"] = 0
                        });
                }
                // EasingTypes
                else if (propInf.PropertyType == typeof(EasingType))
                {
                    defs[$"{currentType.Name}"]["properties"].AsObject().Add(propInf.Name,
                        new JsonObject()
                        {
                            ["$ref"] = $"#/$defs/{nameof(EasingType)}"
                        });
                }
                // Values
                else if (propInf.PropertyType == typeof(Dictionary<StateProperty, double>))
                {
                    defs[$"{currentType.Name}"]["properties"].AsObject().Add(propInf.Name,
                        new JsonObject()
                        {
                            ["$ref"] = $"#/$defs/{currentType.Name}Values"
                        });
                }
                // in case of changes that aren't being handled well
                else { throw new Exception($"{currentType.Name} should not contain Serializable Properties of type {propInf.PropertyType.Name}. You might be missing a JsonIgnore Attribute or a case isn't handled. "); }
            }
            #endregion

            #region Add StateEventValues definitions to schema
            
            defs[$"{currentType.Name}Values"] = new JsonObject()
            {
                ["type"] = "object",
                ["properties"] = new JsonObject(),
                ["additionalProperties"] = false,
            };
            foreach (PropertyInfo propInf in typeof(State).GetProperties().Where((prop) => !prop.IsDefined(typeof(JsonIgnoreAttribute))))
            {
                if (propInf.PropertyType == typeof(string)) continue;
                // double Properties
                else if (propInf.PropertyType == typeof(double))
                {
                    defs[$"{currentType.Name}"]["properties"].AsObject().Add(propInf.Name,
                        new JsonObject()
                        {
                            ["anyOf"] = new JsonArray()
                            {
                                new JsonObject() { ["type"] = "number" }
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
                                    new JsonObject() { ["type"] = "number" }
                                },
                                ["maxItems"] = 10
                            }
                        });
                }
                else { throw new Exception($"{currentType.Name} should not contain Serializable Properties of type {propInf.PropertyType.Name}. You might be missing a JsonIgnore Attribute or a case isn't handled. "); }
            }
            #endregion

            #region Add StateTimeline definitions to schema
            currentType = typeof(StateTimeline);
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
                // Length
                else if (propInf.PropertyType == typeof(uint))
                {
                    defs[$"{currentType.Name}"]["properties"].AsObject().Add(propInf.Name,
                        new JsonObject()
                        {
                            ["type"] = "number",
                            ["minimum"] = 0
                        });
                }
                // Base State
                else if (propInf.PropertyType == typeof(State))
                {
                    defs[$"{currentType.Name}"]["properties"].AsObject().Add(propInf.Name, new JsonObject() { ["$ref"] = $"#/$defs/{nameof(State)}" });
                }
                // Events
                else if (propInf.PropertyType == typeof(List<StateEvent>))
                {
                    defs[$"{currentType.Name}"]["properties"].AsObject().Add(propInf.Name, 
                        new JsonObject() 
                        {
                            ["type"] = "array",
                            ["items"] = new JsonObject() { ["$ref"] = $"#/$defs/{nameof(State)}" }
                        });
                }
                // in case of changes that aren't being handled well
                else { throw new Exception($"{currentType.Name} should not contain Serializable Properties of type {propInf.PropertyType.Name}. You might be missing a JsonIgnore Attribute or a case isn't handled. "); }
            }
            #endregion

            var possibleItems = schema["items"]["oneOf"].AsArray();
            possibleItems.Add(new JsonObject() { ["$ref"] = $"#/$defs/{nameof(StateOptions)}" });
            possibleItems.Add(new JsonObject() { ["$ref"] = $"#/$defs/{nameof(State)}" });
            possibleItems.Add(new JsonObject() { ["$ref"] = $"#/$defs/{nameof(StateSequence)}" });
            possibleItems.Add(new JsonObject() { ["$ref"] = $"#/$defs/{nameof(StateTimeline)}" });

            return schema;
        }

        /// <summary>
        /// Gets a JSON schema as a formatted string for <see cref="GenerationData"/>..
        /// </summary>
        /// <returns>
        /// A JSON schema string representing the schema for <see cref="GenerationData"/>..
        /// </returns>
        public static string GetGenerationDataSchema()
        {
            var options = new JsonSerializerOptions
            {
                TypeInfoResolver = new DefaultJsonTypeInfoResolver(),
                WriteIndented = true
            };

            return GetGenerationDataSchemaObject().ToJsonString(new JsonSerializerOptions(options));
        }

        /// <summary>
        /// Generates a JSON schema string for a specific type of State objects (must be in $defs of schema generated by <see cref="GetGenerationDataSchemaObject"/>).
        /// </summary>
        /// <param name="type">The <see cref="Type"/> to generate the schema for.</param>
        /// <returns>
        /// A JSON schema string representing the schema for the specified type.
        /// </returns>
        private static string GetTypeDataSchema(Type type)
        {
            JsonObject typeDataSchema = new JsonObject()
            {
                ["$schema"] = "https://json-schema.org/draft/2020-12/schema",
                ["$id"] = "http://modulartistic.com/",

                ["$defs"] = new JsonObject() { },

                ["$ref"] = $"#/$defs/{type.Name}"
            };
            JsonObject generationDataSchema = GetGenerationDataSchemaObject();

            foreach (KeyValuePair<string, JsonNode> kvp in generationDataSchema["$defs"].AsObject())
            {
                typeDataSchema["$defs"].AsObject()[kvp.Key] = kvp.Value.DeepClone();
            }


            var options = new JsonSerializerOptions
            {
                TypeInfoResolver = new DefaultJsonTypeInfoResolver(),
                WriteIndented = true
            };

            return typeDataSchema.ToJsonString(new JsonSerializerOptions(options));
        }

        /// <summary>
        /// Validates whether a <see cref="JsonElement"/> is valid according to the schema of the specified type as defined in the $defs of the schema generated by <see cref="GetGenerationDataSchemaObject"/>.
        /// </summary>
        /// <param name="el">The <see cref="JsonElement"/> to validate.</param>
        /// <param name="elementType">The <see cref="Type"/> representing the expected schema of the element.</param>
        /// <returns>
        /// A <see cref="bool"/> indicating whether the <see cref="JsonElement"/> is valid according to the schema.
        /// </returns>
        public static bool IsElementValid(JsonElement el, Type elementType)
        {
            string s = GetTypeDataSchema(elementType);

            // Get Schema
            JsonSchema typeDataSchema = JsonSchema.FromText(s);

            // evaluate and return valid
            return typeDataSchema.Evaluate(el).IsValid;
        }
    }
}

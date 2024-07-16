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
    public static class Schemas
    {
        public static string GetGenerationDataSchema()
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

            var options = new JsonSerializerOptions
            {
                TypeInfoResolver = new DefaultJsonTypeInfoResolver(),
                WriteIndented = true
            };

            return schema.ToJsonString(new JsonSerializerOptions(options)); ;
        }
    
        public static bool IsElementValid(JsonElement el, Type elementType)
        {
            // Get Schema
            JsonSchema stateSchema;
            JsonSchema generationDataSchema = JsonSchema.FromText(Schemas.GetGenerationDataSchema());
            // Get State def from Schema
            generationDataSchema.GetDefs().TryGetValue(elementType.Name, out stateSchema);

            // evaluate and return valid
            return stateSchema.Evaluate(el).IsValid;
        }
    
    }
}

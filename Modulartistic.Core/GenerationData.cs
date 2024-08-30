using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using Json.Schema;
using System.Linq;
using System.Diagnostics;
using System.Threading.Tasks;
using System.ComponentModel.Design;
using NCalc;
using MathNet.Numerics;
using Antlr4.Runtime.Misc;
using Modulartistic.Common;

namespace Modulartistic.Core
{
    public class GenerationData : IList<object>
    {
        #region Properties

        /// <summary>
        /// Gets the collection of objects of type <see cref="StateOptions"/>, <see cref="State"/>, <see cref="StateSequence"/> and <see cref="StateTimeline"/>.
        /// </summary>
        /// <value>
        /// A <see cref="List{T}"/> of <see cref="object"/> representing the collection of data objects.
        /// </value>
        public List<object> Data { get => _data; }

        /// <summary>
        /// Gets the number of objects in the <see cref="GenerationData"/> instance.
        /// </summary>
        /// <value>
        /// An <see cref="int"/> representing the count of objects in the <see cref="Data"/> collection.
        /// </value>
        [JsonIgnore]
        public int Count => _data.Count;

        #endregion

        #region Fields

        /// <summary>
        /// Private field for storing the collection of objects.
        /// </summary>
        private List<object> _data;

        /// <summary>
        /// Collection of supported types
        /// </summary>
        private static readonly HashSet<Type> _supportedTypes = new HashSet<Type>
        {
            typeof(StateOptions),
            typeof(State),
            typeof(StateSequence),
            typeof(StateTimeline)
        };

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="GenerationData"/> class with an empty data collection.
        /// </summary>
        public GenerationData()
        {
            _data = new List<object>();
        }

        #endregion

        #region IList implementation

        /// <summary>
        /// Determines the index of a specific item in the <see cref="IList{T}"/>.
        /// </summary>
        /// <param name="item">The item to locate in the list.</param>
        /// <returns>The index of the item if found in the list; otherwise, -1.</returns>
        /// <exception cref="ArgumentException">Thrown when the item is not of a supported type.</exception>
        int IList<object>.IndexOf(object item)
        {
            // Check if the item's type is in the set of supported types
            if (!_supportedTypes.Contains(item.GetType()))
            {
                string supportedTypesList = string.Join(", ", _supportedTypes.Select(t => t.Name));
                throw new ArgumentException($"Item must be one of the following types: {supportedTypesList}.", nameof(item));
            }
            return _data.IndexOf(item);
        }

        /// <summary>
        /// Inserts an item into the <see cref="IList{T}"/> at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which the item should be inserted.</param>
        /// <param name="item">The item to insert into the list.</param>
        /// <exception cref="ArgumentException">Thrown when the item is not of a supported type.</exception>
        void IList<object>.Insert(int index, object item)
        {
            // Check if the item's type is in the set of supported types
            if (!_supportedTypes.Contains(item.GetType()))
            {
                string supportedTypesList = string.Join(", ", _supportedTypes.Select(t => t.Name));
                throw new ArgumentException($"Item must be one of the following types: {supportedTypesList}.", nameof(item));
            }
            _data.Insert(index, item);
        }

        /// <summary>
        /// Adds an item to the <see cref="ICollection{T}"/>.
        /// </summary>
        /// <param name="item">The item to add to the collection.</param>
        /// <exception cref="ArgumentException">Thrown when the item is not of a supported type.</exception>
        void ICollection<object>.Add(object item)
        {
            // Check if the item's type is in the set of supported types
            if (!_supportedTypes.Contains(item.GetType()))
            {
                string supportedTypesList = string.Join(", ", _supportedTypes.Select(t => t.Name));
                throw new ArgumentException($"Item must be one of the following types: {supportedTypesList}.", nameof(item));
            }
            _data.Add(item);
        }

        /// <summary>
        /// Determines whether the <see cref="ICollection{T}"/> contains a specific value.
        /// </summary>
        /// <param name="item">The object to locate in the collection.</param>
        /// <returns><c>true</c> if the item is found in the collection; otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentException">Thrown when the item is not of a supported type.</exception>
        bool ICollection<object>.Contains(object item)
        {
            // Check if the item's type is in the set of supported types
            if (!_supportedTypes.Contains(item.GetType()))
            {
                string supportedTypesList = string.Join(", ", _supportedTypes.Select(t => t.Name));
                throw new ArgumentException($"Item must be one of the following types: {supportedTypesList}.", nameof(item));
            }
            return _data.Contains(item);
        }

        /// <summary>
        /// Copies the elements of the <see cref="ICollection{T}"/> to an <see cref="Array"/>, starting at a particular array index.
        /// </summary>
        /// <param name="array">The one-dimensional array that is the destination of the copied elements.</param>
        /// <param name="arrayIndex">The zero-based index in the destination array at which storing elements will begin.</param>
        public void CopyTo(object[] array, int arrayIndex)
        {
            _data.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Removes the first occurrence of a specific object from the <see cref="ICollection{T}"/>.
        /// </summary>
        /// <param name="item">The object to remove from the collection.</param>
        /// <returns><c>true</c> if the item was successfully removed; otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentException">Thrown when the item is not of a supported type.</exception>
        bool ICollection<object>.Remove(object item)
        {
            // Check if the item's type is in the set of supported types
            if (!_supportedTypes.Contains(item.GetType()))
            {
                string supportedTypesList = string.Join(", ", _supportedTypes.Select(t => t.Name));
                throw new ArgumentException($"Item must be one of the following types: {supportedTypesList}.", nameof(item));
            }
            return _data.Remove(item);
        }


        /// <summary>
        /// Returns an enumerator that iterates through the <see cref="IEnumerable{T}"/>.
        /// </summary>
        /// <returns>An enumerator that can be used to iterate through the collection.</returns>
        IEnumerator<object> IEnumerable<object>.GetEnumerator()
        {
            return _data.GetEnumerator();
        }

        /// <summary>
        /// Removes the element at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to remove.</param>
        public void RemoveAt(int index)
        {
            _data.RemoveAt(index);
        }

        /// <summary>
        /// Removes all items from the <see cref="ICollection{T}"/>.
        /// </summary>
        public void Clear()
        {
            _data.Clear();
        }

        /// <summary>
        /// Returns an enumerator that iterates through a non-generic <see cref="IEnumerable"/> collection.
        /// </summary>
        /// <returns>An enumerator that can be used to iterate through the collection.</returns>
        public IEnumerator GetEnumerator()
        {
            return _data.GetEnumerator();
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="ICollection{T}"/> is read-only.
        /// </summary>
        /// <value><c>false</c> because the collection is not read-only.</value>
        public bool IsReadOnly => false;

        /// <summary>
        /// Gets or sets the element at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get or set.</param>
        /// <returns>The element at the specified index.</returns>
        /// <exception cref="ArgumentException">Thrown when the value being set is not of a supported type.</exception>
        object IList<object>.this[int index] 
        { 
            get => _data[index];
            set
            {
                // Check if the item's type is in the set of supported types
                if (!_supportedTypes.Contains(value.GetType()))
                {
                    string supportedTypesList = string.Join(", ", _supportedTypes.Select(t => t.Name));
                    throw new ArgumentException($"Item must be one of the following types: {supportedTypesList}.", nameof(value));
                }
                _data[index] = value;
            }
        }

        #endregion

        #region Json Serialization Methods

        /// <summary>
        /// Uses the GenerationData schema to validate if a specified JSON element is a valid <see cref="GenerationData"/> object.
        /// </summary>
        /// <param name="root">The <see cref="JsonElement"/> to validate.</param>
        /// <returns><see cref="Json.Schema.EvaluationResults"/> containing the results of the validation.</returns>
        /// <exception cref="Exception">Thrown if the validation fails and the JSON is not in the correct format.</exception>
        private static EvaluationResults ValidateGenerationDataJson(JsonElement root)
        {
            // load jsonSchema
            JsonSchema schema = JsonSchema.FromText(Schemas.GetGenerationDataSchema());
            
            // validate
            EvaluationResults result = schema.Evaluate(root);

            // return evaluation results
            return result;
        }

        /// <summary>
        /// Serializes the current <see cref="GenerationData"/> object to a JSON string.
        /// </summary>
        /// <returns>The serialized <see cref="GenerationData"/> object as a JSON string.</returns>
        /// <remarks>
        /// This method uses <see cref="JsonSerializer"/> with options to ignore null values, format the JSON with indentation, and use a custom dictionary converter.
        /// </remarks>
        public string ToJson()
        {
            var options = new JsonSerializerOptions
            {
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                WriteIndented = true,
                Converters =
                {
                    new DictionaryTKeyEnumTValueConverter(),
                },
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            };
            return JsonSerializer.Serialize(_data, options);
        }

        /// <summary>
        /// Saves the current <see cref="GenerationData"/> object as a JSON file.
        /// </summary>
        /// <param name="out_dir">The directory where the JSON file should be saved.</param>
        /// <param name="name">The name of the JSON file (excluding extension).</param>
        /// <exception cref="DirectoryNotFoundException">Thrown if the specified directory does not exist.</exception>
        /// <remarks>
        /// This method checks if the directory exists and validates the filename to avoid overwriting existing files. The file is saved with a ".json" extension.
        /// </remarks>
        public void SaveJson(string out_dir, string name)
        {
            // check if it exists
            if (!Directory.Exists(out_dir)) { throw new DirectoryNotFoundException("The Directory " + out_dir + " was not found."); }

            // set the absolute path for the file to be save
            string file_path_out = Path.Join(out_dir, name);

            // Validate (if file with same name exists already, append index)
            file_path_out = Helper.GetValidFileName(file_path_out);

            // Write contents and Save the file
            File.WriteAllText(file_path_out + @".json", ToJson());
        }


        /// <summary>
        /// Loads <see cref="GenerationData"/> properties from a JSON element.
        /// </summary>
        /// <param name="root">The <see cref="JsonElement"/> containing the <see cref="GenerationData"/> properties.</param>
        /// <exception cref="KeyNotFoundException">Thrown if the JSON element contains an unrecognized property type.</exception>
        /// <remarks>
        /// This method validates the JSON element using the <see cref="ValidateGenerationDataJson"/> method, parses each element, and adds it to the <see cref="GenerationData"/> collection.
        /// </remarks>
        public void LoadJson(JsonElement root, GenerationOptions gOpts)
        {
            // make sure json is valid GenerationData
            EvaluationResults res = ValidateGenerationDataJson(root);
            if (!res.IsValid)
            {
                throw new Exception($"Json file has invalid format for GenerationData object, view GenerationDataSchema at {res.SchemaLocation}\n");
            }

            // set options for json parsing
            var options = new JsonSerializerOptions
            {
                Converters =
                {
                    new DictionaryTKeyEnumTValueConverter(),
                },
            };

            // set initial GenerationArgs
            StateOptions currentArgs = new StateOptions();

            // go through each element of array
            foreach (JsonElement element in root.EnumerateArray())
            {
                // Check if element is valid StateOptions
                if (StateOptions.IsJsonElementValid(element))
                {
                    StateOptions args = StateOptions.FromJson(element, gOpts);
                    Data.Add(args);
                    currentArgs = args;
                    continue;
                }

                // Check for State
                if (State.IsJsonElementValid(element))
                {
                    State s = State.FromJson(element, currentArgs);
                    Data.Add(s);
                    continue;
                }

                // Check for StateSequence
                if (StateSequence.IsJsonElementValid(element))
                {
                    StateSequence stateSequence = StateSequence.FromJson(element, currentArgs);
                    Data.Add(stateSequence);
                    continue;
                }

                // Check for StateTimeline
                if (StateTimeline.IsJsonElementValid(element))
                {
                    StateTimeline stateSequence = StateTimeline.FromJson(element, currentArgs);
                    Data.Add(stateSequence);
                    continue;
                }

                throw new KeyNotFoundException($"Unrecognized Property");
            }
        }

        /// <summary>
        /// Creates a new <see cref="GenerationData"/> instance from a JSON element.
        /// </summary>
        /// <param name="element">The <see cref="JsonElement"/> containing the <see cref="GenerationData"/>.</param>
        /// <returns>A new instance of <see cref="GenerationData"/> populated with the data from the JSON element.</returns>
        /// <exception cref="KeyNotFoundException">Thrown if the JSON element contains an unrecognized property type.</exception>
        public static GenerationData FromJson(JsonElement element, GenerationOptions gOpts)
        {
            GenerationData data= new GenerationData();
            data.LoadJson(element, gOpts);

            return data;
        }

        /// <summary>
        /// Loads <see cref="GenerationData"/> from a JSON file.
        /// </summary>
        /// <param name="file">The path to the JSON file.</param>
        /// <returns>A new instance of <see cref="GenerationData"/> populated with the data from the JSON file.</returns>
        /// <exception cref="FileNotFoundException">Thrown if the specified file does not exist.</exception>
        /// <exception cref="KeyNotFoundException">Thrown if the JSON file contains an unrecognized property type.</exception>
        /// <exception cref="JsonException">Thrown if there is an error parsing the JSON file.</exception>
        /// <remarks>
        /// This method reads the JSON file, parses it into a <see cref="JsonDocument"/>, and creates a <see cref="GenerationData"/> instance from the root JSON element.
        /// </remarks>
        public static GenerationData FromFile(string file, GenerationOptions gOpts)
        {
            if (!File.Exists(file)) { throw new FileNotFoundException($"The file '{file}' does not exist.", file); }
            
            try
            {
                using (var stream = File.OpenRead(file))
                {
                    using (var jdoc = JsonDocument.Parse(stream))
                    {
                        return FromJson(jdoc.RootElement, gOpts);
                    }
                }
            }
            catch (JsonException ex)
            {
                throw new JsonException($"Error parsing JSON in file '{file}': {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error processing file '{file}': {ex.Message}", ex);
            }
        }

        #endregion

        #region Generating Methods

        /// <summary>
        /// Generates all <see cref="State"/>, <see cref="StateSequence"/>, and <see cref="StateTimeline"/> objects in the collection.
        /// </summary>
        /// <param name="options">The options for generation, including logger, path provider, and progress reporter.</param>
        /// <param name="path_out">The folder path where the generated files will be saved.</param>
        /// <exception cref="ArgumentException">Thrown if the provided <paramref name="path_out"/> is null or empty.</exception>
        /// <exception cref="Exception">Thrown if an object in the collection is not of type <see cref="State"/>, <see cref="StateSequence"/>, or <see cref="StateTimeline"/>.</exception>
        /// <remarks>
        /// This method iterates through all objects in the <see cref="GenerationData"/> collection and generates files based on their types:
        /// <list type="bullet">
        /// <item>
        /// <description><see cref="State"/> objects: Generates an image and saves it to the specified path.</description>
        /// </item>
        /// <item>
        /// <description><see cref="StateSequence"/> objects: Generates an animation and saves it to the specified path.</description>
        /// </item>
        /// <item>
        /// <description><see cref="StateTimeline"/> objects: Generates an animation and saves it to the specified path.</description>
        /// </item>
        /// </list>
        /// </remarks>
        public async Task GenerateAll(GenerationOptions options, string path_out)
        {
            ILogger? logger = options.Logger;

            string guid = options.GenerationDataGuid.ToString();

            string? backups_folder = options.PathProvider?.GetBackUpPath();
            if (backups_folder is not null) SaveJson(backups_folder, DateTime.Now.ToString("yyyyMMdd-HHmmss")+"-"+guid);
            
            Progress? loopProgress = options.ProgressReporter?.AddTask($"{Guid.NewGuid()}", $"Generating all...", Count);


            // set initial StateOptions
            StateOptions currentArgs = new StateOptions();
            logger?.LogInfo("Generating all generatable objects in GenerationData");
            logger?.LogDebug($"GUID for GenerationData: {guid}");

            // StateOptions Parameter
            object?[] parameters = Array.Empty<object>();
            ParameterEvaluationStrategy currentStrategy = ParameterEvaluationStrategy.Global;

            // loop over all objects in collection
            for (int i = 0; i < Count; i++)
            {
                object obj = Data[i];

                // if the object is StateOptions update the current StateOptions
                if (obj.GetType() == typeof(StateOptions))
                {
                    logger?.Log("Set new StateOptions. ");
                    currentArgs = (StateOptions)obj;

                    currentArgs.TestLoadingAddOns(options);

                    #region load global parameters
                    parameters = currentArgs.Parameters.Select(p => p.InitialValue).ToArray();
                    currentStrategy = ParameterEvaluationStrategy.Global;
                    for (int param_i = 0; param_i < currentArgs.Parameters.Count; param_i++)
                    {
                        StateOptionsParameter param = currentArgs.Parameters[param_i];

                        if (param.Evaluation == ParameterEvaluationStrategy.Auto || param.Evaluation == currentStrategy)
                        {
                            Function f = new Function(param.Expression);
                            f.RegisterStateOptionsProperties(currentArgs);

                            for (int param_j = 0; param_j <= param_i; param_j++)
                            {
                                lock (parameters)
                                {
                                    object? param_value = parameters[param_j];
                                    if (param_value == null) { continue; }

                                    string param_name = currentArgs.Parameters[param_j].Name;

                                    f.RegisterParameter(param_name, param_value);
                                }
                            }

                            f.RegisterStateOptionsProperties(currentArgs);

                            if (f.CanEvaluate())
                            {
                                param.Evaluation = currentStrategy;

                                lock (parameters)
                                {
                                    f.LoadAddOns(currentArgs, options);
                                    parameters[param_i] = f.Evaluate();
                                }
                            }
                        }
                    }
                    #endregion

                    loopProgress?.IncrementProgress();
                    continue;
                }

                // evaluate per generation parameters
                #region load perGen parameters
                currentStrategy = ParameterEvaluationStrategy.PerGeneration;
                for (int param_i = 0; param_i < currentArgs.Parameters.Count; param_i++)
                {
                    StateOptionsParameter param = currentArgs.Parameters[param_i];

                    if (param.Evaluation == ParameterEvaluationStrategy.Auto || param.Evaluation == currentStrategy)
                    {
                        Function f = new Function(param.Expression);
                        f.RegisterStateOptionsProperties(currentArgs);

                        for (int param_j = 0; param_j <= param_i; param_j++)
                        {
                            lock (parameters)
                            {
                                object? param_value = parameters[param_j];
                                if (param_value == null) { continue; }

                                string param_name = currentArgs.Parameters[param_j].Name;

                                f.RegisterParameter(param_name, param_value);
                            }
                        }

                        f.RegisterStateOptionsProperties(currentArgs);

                        if (f.CanEvaluate())
                        {
                            param.Evaluation = currentStrategy;

                            lock (parameters)
                            {
                                f.LoadAddOns(currentArgs, options);
                                parameters[param_i] = f.Evaluate();
                            }
                        }
                    } 
                }
                #endregion

                for (int param_i = 0; param_i < parameters.Length; param_i++) 
                { 
                    if (currentArgs.Parameters[param_i].Evaluation == ParameterEvaluationStrategy.PerState) 
                    {
                        parameters[param_i] = currentArgs.Parameters[param_i].InitialValue;
                    }
                }

                // else if the object is a state, generate said state
                if (obj.GetType() == typeof(State))
                {
                    State S = (State)obj;
                    
                    // Generate the Image
                    try 
                    {

                        logger?.Log($"Generating State: {S.Name}");

                        string filename = S.GenerateImage(currentArgs, parameters, options, path_out);

                        logger?.LogInfo($"Finished Generating State: {S.Name}");
                        loopProgress?.IncrementProgress();
                        continue;
                    }
                    catch (Exception e)
                    {
                        logger?.LogError($"Error Generating State: {S.Name}");
                        if (options.PrintDebugInfo) { logger?.LogException(e); }
                        logger?.LogInfo("Skipping...");
                        loopProgress?.IncrementProgress();
                        continue;
                    }
                }

                // if the object is a StateSequence, generate that StateSequence
                else if (obj.GetType() == typeof(StateSequence))
                {
                    StateSequence SS = (StateSequence)obj;

                    // generate Animation
                    try
                    {
                        logger?.Log($"Generating StateSequence: {SS.Name}");
                        logger?.Log($"Total Scenes: {SS.Count}");
                        logger?.Log($"Total Frames: {SS.TotalFrameCount(currentArgs.Framerate)}");
                        
                        string filename = await SS.GenerateAnimation(currentArgs, parameters, options, path_out);

                        logger?.LogInfo($"Finished Generating StateSequence: {SS.Name}");
                        loopProgress?.IncrementProgress();
                        continue;
                    }
                    catch (Exception e)
                    {
                        logger?.LogError($"Generating StateSequence:  {SS.Name}");
                        if (options.PrintDebugInfo) { logger?.LogException(e); }
                        logger?.LogInfo("Skipping...");
                        loopProgress?.IncrementProgress();
                        continue;
                    }
                }

                // if the object is a StateTimeline
                else if (obj.GetType() == typeof(StateTimeline))
                {
                    StateTimeline ST = (StateTimeline)obj;

                    // generate Animation
                    try
                    {
                        logger?.Log($"Generating StateTimeline: {ST.Name}");
                        logger?.Log($"Total Frames: {ST.TotalFrameCount(currentArgs.Framerate)}");

                        string filename = await ST.GenerateAnimation(currentArgs, parameters, options, path_out);

                        logger?.LogInfo($"Finished Generating StateTimeline: {ST.Name}");
                        loopProgress?.IncrementProgress();
                        continue;
                    }
                    catch (Exception e)
                    {
                        logger?.LogError($"Generating StateTimeline:  {ST.Name}");
                        if (options.PrintDebugInfo) { logger?.LogException(e); }
                        logger?.LogInfo("Skipping...");
                        loopProgress?.IncrementProgress();
                        continue;
                    }
                }

                else
                {
                    logger?.LogError($"Unrecognized object at index {i}");
                    logger?.LogInfo("Skipping...");
                    loopProgress?.IncrementProgress();
                    continue;
                }
            }

            options.ProgressReporter?.RemoveTask(loopProgress);
            logger?.LogInfo("Finished generating all generatable objects in GenerationData");
        }
        
        #endregion
    }
}

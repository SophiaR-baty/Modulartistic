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

namespace Modulartistic.Core
{
    public class GenerationData : IList<object>
    {
        #region subscribable event
        public event EventHandler? GenerationProgressChanged;
        #endregion


        #region Properties
        /// <summary>
        /// Collection of StateOptions, State, StateSequence and StateTimeline Objects
        /// </summary>
        public List<object> Data { get => _data; }

        /// <summary>
        /// Number of Objects in this GenerationData
        /// </summary>
        [JsonIgnore]
        public int Count => _data.Count;

        /// <summary>
        /// Name of this GenerationData
        /// </summary>
        [JsonIgnore]
        public string Name { get; set; }
        #endregion

        #region Fields
        /// <summary>
        /// private field for collection of objects
        /// </summary>
        private List<object> _data;
        #endregion

        #region Constructors
        /// <summary>
        /// Construct an empty GenerationData object
        /// </summary>
        public GenerationData()
        {
            _data = new List<object>();
            Name = "GenerationData";
        }

        /// <summary>
        /// Construct an empty GenerationData object with specified name
        /// </summary>
        public GenerationData(string name)
            : this()
        {
            Name = name;
        }
        #endregion

        #region IList implementation
        int IList<object>.IndexOf(object item)
        {
            return _data.IndexOf(item);
        }

        void IList<object>.Insert(int index, object item)
        {
            _data.Insert(index, item);
        }

        void ICollection<object>.Add(object item)
        {
            _data.Add(item);
        }

        bool ICollection<object>.Contains(object item)
        {
            return _data.Contains(item);
        }

        public void CopyTo(object[] array, int arrayIndex)
        {
            _data.CopyTo(array, arrayIndex);
        }

        bool ICollection<object>.Remove(object item)
        {
            return _data.Remove(item);
        }

        IEnumerator<object> IEnumerable<object>.GetEnumerator()
        {
            return _data.GetEnumerator();
        }
        public void RemoveAt(int index)
        {
            _data.RemoveAt(index);
        }

        public void Clear()
        {
            _data.Clear();
        }

        public IEnumerator GetEnumerator()
        {
            return _data.GetEnumerator();
        }

        public bool IsReadOnly => false;

        object IList<object>.this[int index] { get => _data[index]; set => _data[index] = value; }
        #endregion

        #region Json Serialization Methods
        /// <summary>
        /// Uses the GenerationData schema to validate if a specified json element is a valid GenerationData object
        /// </summary>
        /// <param name="root">The JsonElement to validate</param>
        /// <returns>Json.Schema.EvaluationResults</returns>
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
        /// Serializes the GenerationData object to json
        /// </summary>
        /// <returns> the serialized object as string </returns>
        public string ToJson()
        {
            var options = new JsonSerializerOptions
            {
                IgnoreNullValues = true,
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
        /// Saves the GenerationData as a json file
        /// </summary>
        /// <param name="out_dir"></param>
        /// <exception cref="DirectoryNotFoundException"></exception>
        public void SaveJson(string out_dir = "")
        {
            // check if it exists
            if (!Directory.Exists(out_dir)) { throw new DirectoryNotFoundException("The Directory " + out_dir + " was not found."); }

            // set the absolute path for the file to be save
            string file_path_out = Path.Join(out_dir, (Name == "" ? Constants.GenerationData.GENERATIONDATA_NAME_DEFAULT : Name));
            // Validate (if file with same name exists already, append index)
            file_path_out = Helper.ValidFileName(file_path_out);

            // Write contents and Save the file
            File.WriteAllText(file_path_out + @".json", ToJson());
        }


        /// <summary>
        /// Load GenerationData properties from Json
        /// </summary>
        /// <param name="root">Json Element for GenerationData</param>
        /// <exception cref="KeyNotFoundException"></exception>
        public void LoadJson(JsonElement root)
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
                    StateOptions args = StateOptions.FromJson(element);
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
        /// Load GenerationData from Json
        /// </summary>
        /// <param name="element">Json Element for GenerationData</param>
        /// <exception cref="KeyNotFoundException"></exception>
        public static GenerationData FromJson(JsonElement element)
        {
            GenerationData data= new GenerationData();
            data.LoadJson(element);

            return data;
        }

        /// <summary>
        /// Loads GenerationData from a json file
        /// </summary>
        /// <param name="file">path to the json file</param>
        /// <returns></returns>
        /// <exception cref="FileNotFoundException"></exception>
        /// <exception cref="KeyNotFoundException"></exception>
        public static GenerationData FromFile(string file)
        {
            if (!File.Exists(file)) { throw new FileNotFoundException($"The file '{file}' does not exist.", file); }
            
            try
            {
                using (var stream = File.OpenRead(file))
                {
                    using (var jdoc = JsonDocument.Parse(stream))
                    {
                        return FromJson(jdoc.RootElement);
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
        /// Generates all States, StateSequences and StateTimelines in the Collection
        /// </summary>
        /// <param name="flags">Generation flags</param>
        /// <param name="path_out">the folder to save in</param>
        /// <exception cref="Exception">If an object is neither State, StateSequence nor StateTimeline</exception>
        public async Task GenerateAll(GenerationOptions options)
        {
            ILogger logger = options.Logger;

            // initiates a stopwatch for the whole execution and for each iteration
            Stopwatch totalTime = Stopwatch.StartNew();
            Stopwatch iterationTime = new Stopwatch();

            logger.Log("Start generating all. ");

            // set initial StateOptions
            StateOptions currentArgs = new StateOptions();
            // loop over all objects in collection
            for (int i = 0; i < Count; i++)
            {
                // set clock
                iterationTime.Restart();

                object obj = Data[i];

                // if the object is StateOptions update the current StateOptions
                if (obj.GetType() == typeof(StateOptions))
                {
                    currentArgs = (StateOptions)obj;
                    logger.Log("Set new StateOptions. ");
                    continue;
                }

                // else if the object is a state, generate said state
                else if (obj.GetType() == typeof(State))
                {
                    State S = obj as State;
                    logger.Log($"Generating State {S.Name}. ");

                    // Generate the Image
                    try
                    {
                        string filename = S.GenerateImage(currentArgs, options);
                        logger.Log($"Finished Generating State {S.Name}. ");
                    }
                    catch (Exception e)
                    {
                        throw;
                    }
                }

                // if the object is a StateSequence, generate that StateSequence
                else if (obj.GetType() == typeof(StateSequence))
                {
                    StateSequence SS = obj as StateSequence;
                    logger.Log($"Generating StateSequence {SS.Name}. ");
                    logger.Log($"Total Frames: {SS.TotalFrameCount(currentArgs.Framerate)}. ");

                    // generate Animation
                    try
                    {
                        string filename = await SS.GenerateAnimation(currentArgs, options);
                        logger.Log($"Finished Generating StateSequence {SS.Name}. ");
                    }
                    catch (Exception e)
                    {
                        throw;
                    }
                }

                // if the object is a StateTimeline
                else if (obj.GetType() == typeof(StateTimeline))
                {
                    StateTimeline ST = obj as StateTimeline;
                    logger.Log($"Generating StateTimeline {ST.Name}. ");
                    logger.Log($"Total Frames: {ST.TotalFrameCount(currentArgs.Framerate)}. ");

                    // generate Animation
                    try
                    {
                        string filename = await ST.GenerateAnimation(currentArgs, options);
                        logger.Log($"Finished Generating StateTimeline {ST.Name}. ");
                    }
                    catch (Exception e)
                    {
                        throw;
                    }
                }
                else
                {
                    throw new Exception();
                }

                logger.Log($"Took {iterationTime.Elapsed}. ");
            }

            logger.Log($"Generating all took {totalTime.Elapsed}. ");
        }
            #endregion
    }
}

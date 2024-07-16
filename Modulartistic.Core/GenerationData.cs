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
        /// <param name="path_out">the folder to save in</param>
        /// <exception cref="Exception">If an object is neither State, StateSequence nor StateTimeline</exception>
        public async Task GenerateAll(string path_out = @"")
        {
            await GenerateAll(GenerationDataFlags.None, path_out);
        }

        /// <summary>
        /// Generates all States, StateSequences and StateTimelines in the Collection
        /// </summary>
        /// <param name="flags">Generation flags</param>
        /// <param name="path_out">the folder to save in</param>
        /// <exception cref="Exception">If an object is neither State, StateSequence nor StateTimeline</exception>
        public async Task GenerateAll(GenerationDataFlags flags, string path_out = @"")
        {
            // initiates a stopwatch for the whole execution and for each iteration
            Stopwatch totalTime = Stopwatch.StartNew();
            Stopwatch iterationTime = new Stopwatch();

            // convert the flags to boolean variables
            bool Show = (flags & GenerationDataFlags.Show) == GenerationDataFlags.Show;
            bool Debug = (flags & GenerationDataFlags.Debug) == GenerationDataFlags.Debug;
            bool Faster = (flags & GenerationDataFlags.Faster) == GenerationDataFlags.Faster;
            bool MP4 = (flags & GenerationDataFlags.MP4) == GenerationDataFlags.MP4;
            bool KeepFrames = (flags & GenerationDataFlags.KeepFrames) == GenerationDataFlags.KeepFrames;

            // set initial GenerationArgs
            StateOptions currentArgs = new StateOptions();

            // loop over all objects in collection
            for (int i = 0; i < Count; i++)
            {
                // set clock
                iterationTime.Restart();

                object obj = Data[i];

                // if the object is GenerationArgs update the current GenerationArgs
                if (obj.GetType() == typeof(StateOptions))
                {
                    currentArgs = (StateOptions)obj;

                    // Print Debug Information
                    if (Debug)
                    {
                        Console.WriteLine(currentArgs.GetDebugInfo());
                        Console.WriteLine();
                    }
                }

                // else if the object is a state, generate said state
                else if (obj.GetType() == typeof(State))
                {
                    State S = obj as State;

                    // print Debug Information Pre-Generating
                    if (Debug)
                    {
                        Console.WriteLine("Generating Image for State: ");
                        Console.WriteLine(S.GetDetailsString());
                        Console.WriteLine();
                    }

                    // generate Image, if Faster Flag use Multithreaded
                    int max_threads = Faster ? -1 : 1;

                    // Generate the Image
                    try
                    {
                        string filename = S.GenerateImage(currentArgs, max_threads, path_out);

                        // print Debug Information Post Generating
                        if (Debug)
                        {
                            Console.WriteLine($"Done Generating \"{filename}\"\n");
                        }

                        // if Show Flag, show Image
                        if (Show)
                        {
                            var p = new Process();
                            p.StartInfo = new ProcessStartInfo(filename) { UseShellExecute = true };
                            p.Start();
                        }
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

                    // print Debug Information Pre Generating
                    if (Debug)
                    {
                        Console.WriteLine("Generating Animation for StateSequence: ");
                        Console.WriteLine(SS.GetDetailsString(currentArgs.Framerate));

                        Console.WriteLine();
                    }

                    AnimationFormat type = MP4 ? AnimationFormat.Mp4 : AnimationFormat.Gif;
                    int max_threads = Faster ? -1 : 1;

                    // generate Animation
                    try
                    {
                        string filename = await SS.GenerateAnimation(currentArgs, max_threads, type, KeepFrames, path_out);
                        //print Debug Information Post Generating
                        if (Debug)
                        {
                            Console.WriteLine($"Done Generating \"{filename}\"\n");
                        }

                        // if Show Flag, show Image
                        if (Show)
                        {
                            var p = new Process();
                            p.StartInfo = new ProcessStartInfo(filename) { UseShellExecute = true };
                            p.Start();
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"Error Generating \"{SS.Name}\"\n");
                        if (Debug)
                        {
                            Console.Error.WriteLine(e.StackTrace);
                            Exception ex = e;
                            while (ex != null)
                            {
                                Console.Error.WriteLine(e.Message);
                                e = e.InnerException;
                            }
                            continue;
                        }
                    }
                }

                // if the object is a StateTimeline
                else if (obj.GetType() == typeof(StateTimeline))
                {
                    StateTimeline ST = obj as StateTimeline;

                    // print Debug Information Pre Generating
                    if (Debug)
                    {
                        Console.WriteLine("Generating Animation for StateTimeline: ");
                        Console.WriteLine(ST.GetDetailsString(currentArgs.Framerate));

                        Console.WriteLine();
                    }

                    AnimationFormat type = MP4 ? AnimationFormat.Mp4 : AnimationFormat.Gif;
                    int max_threads = Faster ? -1 : 1;

                    // generate Animation
                    try
                    {
                        string filename = await ST.GenerateAnimation(currentArgs, max_threads, type, KeepFrames, path_out);
                        //print Debug Information Post Generating
                        if (Debug)
                        {
                            Console.WriteLine($"Done Generating \"{filename}\"\n");
                        }

                        // if Show Flag, show Image
                        if (Show)
                        {
                            var p = new Process();
                            p.StartInfo = new ProcessStartInfo(filename) { UseShellExecute = true };
                            p.Start();
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"Error Generating \"{ST.Name}\"\n");
                        if (Debug)
                        {
                            Console.Error.WriteLine(e.StackTrace);
                            Exception ex = e;
                            while (ex != null)
                            {
                                Console.Error.WriteLine(e.Message);
                                e = e.InnerException;
                            }
                            continue;
                        }
                    }
                }
                else
                {
                    throw new Exception();
                }

                if (Debug)
                {
                    Console.WriteLine("Took " + iterationTime.Elapsed.ToString());
                }
            }
            Console.WriteLine("Generating all took: " + totalTime.Elapsed.ToString());
        }
        #endregion
    }


    /// <summary>
    /// Enum console argument flags
    /// </summary>
    [Flags]
    public enum GenerationDataFlags
    {
        None = 0b_0000_0000,
        Show = 0b_0000_0001,
        Debug = 0b_0000_0010,
        Faster = 0b_0000_0100,
        MP4 = 0b_0000_1000,
        KeepFrames = 0b_0001_0000,
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Linq;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Modulartistic.Core
{
    public class GenerationData
    {
        #region Properties
        public List<object> Data { get => data; }

        [JsonIgnore]
        public int Count => data.Count;

        public object this[int index] { get => data[index]; }

        [JsonIgnore]
        public string Name { get; set; }
        #endregion

        #region Fields
        private List<object> data;
        #endregion

        #region Constructors
        public GenerationData()
        {
            data = new List<object>();
            Name = "GenerationData";
        }
        #endregion

        #region List Methods
        public void Add(GenerationArgs value) => data.Add(value);
        public void Add(State value) => data.Add(value);
        public void Add(StateSequence value) => data.Add(value);
        public void Add(StateTimeline value) => data.Add(value);

        public void Clear() => data.Clear();

        public bool Contains(object value) => data.Contains(value);

        public int IndexOf(object value) => data.IndexOf(value);

        public void Insert(int index, GenerationArgs value) => data.Insert(index, value);
        public void Insert(int index, State value) => data.Insert(index, value);
        public void Insert(int index, StateSequence value) => data.Insert(index, value);
        public void Insert(int index, StateTimeline value) => data.Insert(index, value);

        public void Remove(GenerationArgs value) => data.Remove(value);
        public void Remove(State value) => data.Remove(value);
        public void Remove(StateSequence value) => data.Remove(value);
        public void Remove(StateTimeline value) => data.Remove(value);

        public void RemoveAt(int index) => data.RemoveAt(index);

        public IEnumerator GetEnumerator() => data.GetEnumerator();
        #endregion

        #region Json Serialization Methods
        
        /// <summary>
        /// Serializes the GenerationData object to json
        /// </summary>
        /// <returns> the seriealized object as string </returns>
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
            return JsonSerializer.Serialize(data, options);
        }

        /// <summary>
        /// Saves the GenerationData as a json file
        /// </summary>
        /// <param name="out_dir"></param>
        /// <exception cref="DirectoryNotFoundException"></exception>
        public void SaveJson(string out_dir = "")
        {
            // If out-dir is empty set to default, then check if it exists
            out_dir = out_dir == "" ? Constants.DEMOFOLDER : out_dir;
            if (!Directory.Exists(out_dir)) { throw new DirectoryNotFoundException("The Directory " + out_dir + " was not found."); }

            // set the absolute path for the file to be save
            string file_path_out = Path.Join(out_dir, (Name == "" ? Constants.GENERATIONDATA_NAME_DEFAULT : Name));
            // Validate (if file with same name exists already, append index)
            file_path_out = Helper.ValidFileName(file_path_out);

            // Write contents and Save the file
            File.WriteAllText(file_path_out + @".json", ToJson());
        }

        /// <summary>
        /// Load GenerationData from a json file
        /// </summary>
        /// <param name="file_name">the absolute path of the file</param>
        /// <exception cref="FileNotFoundException"></exception>
        /// <exception cref="Exception"></exception>
        public void LoadJson(string file_name)
        {
            if (!File.Exists(file_name))
            {
                throw new FileNotFoundException($"The specified File {file_name} does not exist. ");
            }

            string jsontext = File.ReadAllText(file_name);

            JsonDocument jd = JsonDocument.Parse(jsontext);


            JsonElement root = jd.RootElement;

            if (root.ValueKind != JsonValueKind.Array)
            {
                throw new Exception($"Error: Expected ArrayType RootElement in Json File {file_name} but got {root.ValueKind.ToString()}");
            }

            var options = new JsonSerializerOptions
            {
                Converters =
                {
                    new DictionaryTKeyEnumTValueConverter(),
                },
            };

            for (int i = 0; i < root.GetArrayLength(); i++)
            {
                JsonElement element = root[i];

                // This is terrible implementation but it became frustrating trying to find an alternative
                // what I actually want is the following: 
                // Try to deserialize the object as (Type) but if the JsonElement has a Property that (Type) has not, the next Type should be tested. if all types fail an Exception should be raised
                if (element.EnumerateObject().All(prop => typeof(GenerationArgs).GetProperty(prop.Name) != null))
                {
                    GenerationArgs generationArgs = JsonSerializer.Deserialize<GenerationArgs>(element.GetRawText(), options);
                    Data.Add(generationArgs);
                }
                else if (element.EnumerateObject().All(prop => typeof(State).GetProperty(prop.Name) != null))
                {
                    State state = JsonSerializer.Deserialize<State>(element.GetRawText(), options);
                    Data.Add(state);
                }
                else if (element.EnumerateObject().All(prop => typeof(StateSequence).GetProperty(prop.Name) != null))
                {
                    StateSequence stateSequence = JsonSerializer.Deserialize<StateSequence>(element.GetRawText(), options);
                    Data.Add(stateSequence);
                }
                else if (element.EnumerateObject().All(prop => typeof(StateTimeline).GetProperty(prop.Name) != null))
                {
                    StateTimeline stateTimeLine = JsonSerializer.Deserialize<StateTimeline>(element.GetRawText(), options);
                    Data.Add(stateTimeLine);
                }
                else { throw new Exception($"Parsing Error in file {file_name}: Unrecognized Type"); }
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
            GenerationArgs currentArgs = new GenerationArgs();

            // loop over all objects in collection
            for (int i = 0; i < Count; i++)
            {
                // set clock
                iterationTime.Restart();

                object obj = Data[i];

                // if the object is GenerationArgs update the current GenerationArgs
                if (obj.GetType() == typeof(GenerationArgs))
                {
                    currentArgs = (GenerationArgs)obj;

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
                        Console.WriteLine(S.GetDebugInfo());
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
                        Console.WriteLine($"Error Generating \"{S.Name}\"\n");
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

                // if the object is a StateSequence, generate that StateSequence
                else if (obj.GetType() == typeof(StateSequence))
                {
                    StateSequence SS = obj as StateSequence;

                    // print Debug Information Pre Generating
                    if (Debug)
                    {
                        Console.WriteLine("Generating Animation for StateSequence: ");
                        Console.WriteLine(SS.GetDetailsString(currentArgs.Framerate.GetValueOrDefault(Constants.FRAMERATE_DEFAULT)));

                        Console.WriteLine();
                    }

                    AnimationType type = MP4 ? AnimationType.Mp4 : AnimationType.Gif;
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
                else if (obj.GetType() == typeof(StateTimeline))
                {
                    if (Faster) { Console.Error.WriteLine("Faster Mode not implemented for StateTimeline. Using normal mode. "); } // ADD FASTER HERE
                    if (Debug) { Console.Error.WriteLine("Debug Mode not implemented for StateTimeline. Using normal mode. "); } // ADD DEBUG HERE
                    if (Show) { Console.Error.WriteLine("Show Mode not implemented for StateTimeline. Using normal mode. "); } // ADD SHOW HERE
                    if (MP4) { Console.Error.WriteLine("MP4 Mode not implemented for StateTimeline. Using normal mode. "); } // ADD SHOW HERE

                    StateTimeline ST = obj as StateTimeline;
                    ST.GenerateAnimation(currentArgs, path_out);
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

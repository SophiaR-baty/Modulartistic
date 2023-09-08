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
            };
            return JsonSerializer.Serialize(data, options);
            // return Newtonsoft.Json.JsonConvert.SerializeObject(this.data, Newtonsoft.Json.Formatting.Indented);
        }

        public void SaveJson(string path_out = "")
        {
            // Creating filename and path, checking if directory exists
            string path = path_out == "" ? AppDomain.CurrentDomain.BaseDirectory + Path.DirectorySeparatorChar + "demofiles" : path_out;
            if (!Directory.Exists(path)) { throw new DirectoryNotFoundException("The Directory " + path + " was not found."); }
            path += Path.DirectorySeparatorChar + (Name == "" ? "State" : Name);

            // Edit the filename so that it's unique
            path = Helper.ValidFileName(path);
            path += ".json";

            if (File.Exists(path)) { File.Delete(path); }

            File.WriteAllText(path, ToJson());
        }

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

                    // print Debug Information Pre Generating
                    if (Debug)
                    {
                        Console.WriteLine("Generating Image for State: ");
                        Console.WriteLine(S.GetDebugInfo());

                        Console.WriteLine();
                    }

                    // generate Image, if Faster Flag use Multithreaded
                    string filename;
                    if (Faster) { filename = S.GenerateImage(currentArgs, -1, path_out); }
                    else { filename = S.GenerateImage(currentArgs, path_out); }

                    // print Debug Information Post Generating
                    if (Debug)
                    {
                        Console.WriteLine("Done Generating \"{0}\"\n", filename);
                    }

                    // if Show Flag, show Image
                    if (Show)
                    {
                        var p = new Process();
                        p.StartInfo = new ProcessStartInfo(filename) { UseShellExecute = true };
                        p.Start();
                    }
                }

                // if the object is a StateSequence, generate that StateSequence
                else if (obj.GetType() == typeof(StateSequence))
                {
                    StateSequence SS = obj as StateSequence;

                    // print Debug Information Pre Generating
                    if (Debug)
                    {
                        Console.WriteLine("Generating Image for StateTimeline: ");
                        Console.WriteLine(SS.GetDetailsString(currentArgs.Framerate.GetValueOrDefault(Constants.FRAMERATE_DEFAULT)));

                        Console.WriteLine();
                    }

                    // generate Animation, if Faster Flag use Multithreaded 
                    string filename;
                    if (MP4)
                    { filename = await SS.CreateMp4(currentArgs, path_out); }
                    else if (Faster) { filename = SS.GenerateAnimation(currentArgs, -1, path_out); }
                    else { filename = SS.GenerateAnimation(currentArgs, path_out); }

                    //print Debug Information Post Generating
                    if (Debug)
                    {
                        Console.WriteLine("Done Generating \"{0}\"\n", filename);
                    }

                    // if Show Flag, show Image
                    if (Show)
                    {
                        var p = new Process();
                        p.StartInfo = new ProcessStartInfo(filename) { UseShellExecute = true };
                        p.Start();
                    }
                }
                else if (obj.GetType() == typeof(StateTimeline))
                {
                    if (Faster) { Console.Error.WriteLine("Faster Mode not implemented for StateTimeline. Using normal mode. "); } // ADD FASTER HERE
                    if (Debug) { Console.Error.WriteLine("Debug Mode not implemented for StateTimeline. Using normal mode. "); } // ADD DEBUG HERE
                    if (Show) { Console.Error.WriteLine("Show Mode not implemented for StateTimeline. Using normal mode. "); } // ADD SHOW HERE

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
    }
}

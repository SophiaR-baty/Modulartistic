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
        public void Add(GenerationOptions value) => data.Add(value);
        public void Add(State value) => data.Add(value);
        public void Add(StateSequence value) => data.Add(value);
        public void Add(StateTimeline value) => data.Add(value);

        public void Clear() => data.Clear();

        public bool Contains(object value) => data.Contains(value);

        public int IndexOf(object value) => data.IndexOf(value);

        public void Insert(int index, GenerationOptions value) => data.Insert(index, value);
        public void Insert(int index, State value) => data.Insert(index, value);
        public void Insert(int index, StateSequence value) => data.Insert(index, value);
        public void Insert(int index, StateTimeline value) => data.Insert(index, value);

        public void Remove(GenerationOptions value) => data.Remove(value);
        public void Remove(State value) => data.Remove(value);
        public void Remove(StateSequence value) => data.Remove(value);
        public void Remove(StateTimeline value) => data.Remove(value);

        public void RemoveAt(int index) => data.RemoveAt(index);

        public IEnumerator GetEnumerator() => data.GetEnumerator();
        #endregion

        #region Json Serialization Methods


        private static EvaluationResults isJsonValid(string file_name)
        {
            JsonSchema schema = JsonSchema.FromFile(Constants.GENERATIONDATA_SCHEMA_FILE);
            string file = File.ReadAllText(file_name);
            JsonDocument json = JsonDocument.Parse(file);

            EvaluationResults result = schema.Evaluate(json.RootElement);

            return result;
        }

        private static EvaluationResults validateObject(JsonElement el, string object_name)
        {
            JsonSchema schema;
            if (!JsonSchema.FromFile(Constants.GENERATIONDATA_SCHEMA_FILE).GetDefs().TryGetValue(object_name, out schema))
            {
                throw new KeyNotFoundException($"The Key {object_name} is not a recognized onject.");
            }

            EvaluationResults result = schema.Evaluate(el);
            
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
            out_dir = out_dir == "" ? PathConfig.DEMOFOLDER : out_dir;
            if (!Directory.Exists(out_dir)) { throw new DirectoryNotFoundException("The Directory " + out_dir + " was not found."); }

            // set the absolute path for the file to be save
            string file_path_out = Path.Join(out_dir, (Name == "" ? Constants.GENERATIONDATA_NAME_DEFAULT : Name));
            // Validate (if file with same name exists already, append index)
            file_path_out = Helper.ValidFileName(file_path_out);

            // Write contents and Save the file
            File.WriteAllText(file_path_out + @".json", ToJson());
        }

        public void LoadJson(string file_name)
        {
            // Make sure file exists
            if (!File.Exists(file_name))
            {
                throw new FileNotFoundException($"The specified File {file_name} does not exist. ");
            }

            // parse the json file
            string jsontext = File.ReadAllText(file_name);
            EvaluationResults res = isJsonValid(file_name);

            JsonDocument jd = JsonDocument.Parse(jsontext);
            JsonElement root = jd.RootElement;

            var options = new JsonSerializerOptions
            {
                Converters =
                {
                    new DictionaryTKeyEnumTValueConverter(),
                },
            };

            // set initial GenerationArgs
            GenerationOptions currentArgs = new GenerationOptions();

            // go through each element of array
            foreach (JsonElement element in root.EnumerateArray())
            {
                EvaluationResults el_res;

                // Check for GenerationOptions
                el_res = validateObject(element, "GenerationOptions");
                if (el_res.IsValid) 
                {
                    GenerationOptions args = getGenerationOptions(element);
                    if (args != null)
                    {
                        Data.Add(args);
                        currentArgs = args;
                        continue;
                    }
                }

                // Check for State
                el_res = validateObject(element, "State");
                if (el_res.IsValid)
                {
                    State s = getState(element, currentArgs);
                    if (s != null) 
                    { 
                        Data.Add(s); 
                        continue; 
                    }
                }

                // Check for StateSequence
                el_res = validateObject(element, "StateSequence");
                if (el_res.IsValid)
                {
                    StateSequence stateSequence = JsonSerializer.Deserialize<StateSequence>(element.GetRawText(), options);
                    Data.Add(stateSequence);
                    continue;
                }

                // Check for StateTimeline
                el_res = validateObject(element, "StateTimeline");
                if (el_res.IsValid)
                {
                    StateTimeline stateTimeLine = JsonSerializer.Deserialize<StateTimeline>(element.GetRawText(), options);
                    Data.Add(stateTimeLine);
                    continue;
                }

                throw new Exception($"Parsing Error in file {file_name}: Unrecognized Property");
            }
        }


        private double getValueOrEvaluate(JsonProperty elem, GenerationOptions args = null, State s = null, StateSequence ss = null, StateTimeline st = null)
        {
            // retrieve the value beforehand
            double value;
            if (elem.Value.ValueKind == JsonValueKind.String)
            {
                Expression expr = new Expression(elem.Value.GetString());
                if (s != null) { Helper.ExprRegisterStateProperties(ref expr, s); }
                if (args != null) { Helper.ExprRegisterGenArgs(ref expr, args); }
                value = (double)expr.Evaluate();
            }
            else if (elem.Value.ValueKind == JsonValueKind.Number) { value = elem.Value.GetDouble(); }
            else { throw new Exception($"Property {elem.Name} must be string or number. "); }

            return value;
        }

        private double getValueOrEvaluate(JsonElement elem, GenerationOptions args = null, State s = null, StateSequence ss = null, StateTimeline st = null)
        {
            // retrieve the value beforehand
            double value;
            if (elem.ValueKind == JsonValueKind.String)
            {
                Expression expr = new Expression(elem.GetString());
                if (s != null) { Helper.ExprRegisterStateProperties(ref expr, s); }
                if (args != null) { Helper.ExprRegisterGenArgs(ref expr, args); }
                value = (double)expr.Evaluate();
            }
            else if (elem.ValueKind == JsonValueKind.Number) { value = elem.GetDouble(); }
            else { throw new Exception($"Element must be string or number. "); }

            return value;
        }

        /// <summary>
        /// Tries to parse a json element to GenerationArgs, returns null if didn't work
        /// </summary>
        /// <param name="element">The JsonElement to be parsed</param>
        /// <returns>Parsed GenerationArgs or null</returns>
        /// <exception cref="Exception"></exception>
        private GenerationOptions getGenerationOptions(JsonElement element)
        {
            GenerationOptions args = new GenerationOptions();
            
            foreach (JsonProperty elem in element.EnumerateObject()) 
            {
                switch (elem.Name)
                {
                    case "UseRGB": 
                        {
                            args.UseRGB = elem.Value.GetBoolean();
                            break; 
                        }
                    case "InvalidColorGlobal": 
                        {
                            args.InvalidColorGlobal = elem.Value.GetBoolean(); 
                            break; 
                        }
                    case "Circular": 
                        {
                            args.Circular = elem.Value.GetBoolean(); 
                            break; 
                        }
                    case "Width":
                        {
                            args.Width = (int)getValueOrEvaluate(elem, args);
                            break;
                        }
                    case "Height":
                        {
                            args.Height = (int)getValueOrEvaluate(elem, args);
                            break;
                        }
                    case "Framerate": 
                        {
                            args.Framerate = (uint)getValueOrEvaluate(elem, args);
                            break; 
                        }
                    case "FunctionRH":
                        {
                            args.FunctionRH = elem.Value.GetString();
                            break;
                        }
                    case "FunctionGS":
                        {
                            args.FunctionGS = elem.Value.GetString();
                            break;
                        }
                    case "FunctionBV":
                        {
                            args.FunctionBV = elem.Value.GetString();
                            break;
                        }
                    case "FunctionAlp": 
                        {
                            args.FunctionAlpha = elem.Value.GetString(); 
                            break; 
                        }
                    case "AddOns":
                        {
                            foreach (JsonElement addon_elem in elem.Value.EnumerateArray())
                            {
                                args.AddOns.Add(addon_elem.GetString());
                            }
                            break;
                        }
                    default: { return null; }
                }
            }

            return args;
        }

        /// <summary>
        /// Tries to parse a json element to State, returns null if didn't work
        /// </summary>
        /// <param name="element">The JsonElement to be parsed</param>
        /// <param name="args">GenerationArgs</param>
        /// <returns>Parsed State or null</returns>
        private State getState(JsonElement element, GenerationOptions args)
        {
            State s = new State();

            foreach (JsonProperty elem in element.EnumerateObject())
            {
                // treat name and parmeters seperately, cause they are not numbers
                if (elem.Name == "Parameters")
                {
                    
                }
                
                switch (elem.Name)
                {
                    case "Name": { s.Name = elem.Value.GetString(); break; }
                    case "X0": { s.X0 = getValueOrEvaluate(elem, args, s); break; }
                    case "Y0": { s.Y0 = getValueOrEvaluate(elem, args, s); break; }
                    case "XRotationCenter": { s.XRotationCenter = getValueOrEvaluate(elem, args, s); break; }
                    case "YRotationCenter": { s.YRotationCenter = getValueOrEvaluate(elem, args, s); break; }
                    case "XFactor": { s.XFactor = getValueOrEvaluate(elem, args, s); break; }
                    case "YFactor": { s.YFactor = getValueOrEvaluate(elem, args, s); break; }
                    case "Rotation": { s.Rotation = getValueOrEvaluate(elem, args, s); break; }
                    
                    case "Mod": { s.Mod = getValueOrEvaluate(elem, args, s); break; }
                    case "ModLimLow": { s.ModLimLow = getValueOrEvaluate(elem, args, s); break; }
                    case "ModLimUpp": { s.ModLimUpp = getValueOrEvaluate(elem, args, s); break; }

                    case "ColorRH": { s.ColorRH = getValueOrEvaluate(elem, args, s); break; }
                    case "ColorGS": { s.ColorGS = getValueOrEvaluate(elem, args, s); break; }
                    case "ColorBV": { s.ColorBV = getValueOrEvaluate(elem, args, s); break; }
                    case "InvColorRH": { s.InvColorRH = getValueOrEvaluate(elem, args, s); break; }
                    case "InvColorGS": { s.InvColorGS = getValueOrEvaluate(elem, args, s); break; }
                    case "InvColorBV": { s.InvColorBV = getValueOrEvaluate(elem, args, s); break; }

                    case "ColorAlp": { s.ColorAlp = getValueOrEvaluate(elem, args, s); break; }
                    case "InvColorAlp": { s.InvColorAlp = getValueOrEvaluate(elem, args, s); break; }

                    case "ColorFactorRH": { s.ColorFactorRH = getValueOrEvaluate(elem, args, s); break; }
                    case "ColorFactorGS": { s.ColorFactorGS = getValueOrEvaluate(elem, args, s); break; }
                    case "ColorFactorBV": { s.ColorFactorBV = getValueOrEvaluate(elem, args, s); break; }
                    case "Parameters": 
                    { 
                        int i = 0; 
                        foreach (JsonElement param_elem in elem.Value.EnumerateArray())
                        { 
                            s.Parameters[i] = getValueOrEvaluate(param_elem, args, s); 
                            i++; 
                        } 
                        break; 
                    }

                    default: return null;
                }
            }

            return s;
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
            GenerationOptions currentArgs = new GenerationOptions();

            // loop over all objects in collection
            for (int i = 0; i < Count; i++)
            {
                // set clock
                iterationTime.Restart();

                object obj = Data[i];

                // if the object is GenerationArgs update the current GenerationArgs
                if (obj.GetType() == typeof(GenerationOptions))
                {
                    currentArgs = (GenerationOptions)obj;

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
                        Console.WriteLine(SS.GetDetailsString(currentArgs.Framerate));

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

                    AnimationType type = MP4 ? AnimationType.Mp4 : AnimationType.Gif;
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

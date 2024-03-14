using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Linq;
using System.Diagnostics;
using System.Threading.Tasks;
using System.ComponentModel.Design;
using NCalc;

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

        /// <summary>
        /// Load GenerationData from a json file
        /// </summary>
        /// <param name="file_name">the absolute path of the file</param>
        /// <exception cref="FileNotFoundException"></exception>
        /// <exception cref="Exception"></exception>
        public void LoadJson(string file_name)
        {
            // Make sure file exists
            if (!File.Exists(file_name))
            {
                throw new FileNotFoundException($"The specified File {file_name} does not exist. ");
            }

            // parse the json file
            string jsontext = File.ReadAllText(file_name);
            JsonDocument jd = JsonDocument.Parse(jsontext);
            JsonElement root = jd.RootElement;
            if (root.ValueKind != JsonValueKind.Array)
            {
                throw new Exception($"JsonParsingError: in file {file_name}, expected ArrayType RootElement but got {root.ValueKind}");
            }
            if (root.EnumerateArray().Any(elem => elem.ValueKind != JsonValueKind.Object))
            {
                throw new Exception($"JsonParsingError: in file {file_name}, all elements must be of type object, got a Non Object element.");
            }
            var options = new JsonSerializerOptions
            {
                Converters =
                {
                    new DictionaryTKeyEnumTValueConverter(),
                },
            };

            // set initial GenerationArgs
            GenerationArgs currentArgs = new GenerationArgs();

            // go through each element of array
            foreach (JsonElement element in root.EnumerateArray())
            {
                GenerationArgs args = TryGetGenerationArgs(element);
                if (args != null )
                {
                    Data.Add(args);
                    currentArgs = args;
                    continue;
                }
                State s = TryGetState(element, currentArgs);
                if (s != null) { Data.Add(s); continue; }

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
                else { throw new Exception($"Parsing Error in file {file_name}: Unrecognized Property"); }
            }
        }
        
        /// <summary>
        /// Tries to parse a json element to GenerationArgs, returns null if didn't work
        /// </summary>
        /// <param name="element">The JsonElement to be parsed</param>
        /// <returns>Parsed GenerationArgs or null</returns>
        /// <exception cref="Exception"></exception>
        private GenerationArgs TryGetGenerationArgs(JsonElement element)
        {
            if (element.ValueKind != JsonValueKind.Object) { return null; }

            GenerationArgs args = new GenerationArgs();

            bool used_size = false;
            bool used_height_width = false;
            foreach (JsonProperty elem in element.EnumerateObject()) 
            {
                switch (elem.Name)
                {
                    case "UseRGB": 
                        { 
                            if (elem.Value.ValueKind != JsonValueKind.False && elem.Value.ValueKind != JsonValueKind.True) { throw new Exception($"JsonParsingError: Expected a boolean value for property {elem.Name} but got a {elem.Value.ValueKind}"); }
                            args.UseRGB = elem.Value.GetBoolean();
                            break; 
                        }
                    case "InvalidColorGlobal": 
                        {
                            if (elem.Value.ValueKind != JsonValueKind.False && elem.Value.ValueKind != JsonValueKind.True) { throw new Exception($"JsonParsingError: Expected a boolean value for property {elem.Name} but got a {elem.Value.ValueKind}"); }
                            args.InvalidColorGlobal = elem.Value.GetBoolean(); 
                            break; 
                        }
                    case "Circular": 
                        {
                            if (elem.Value.ValueKind != JsonValueKind.False && elem.Value.ValueKind != JsonValueKind.True) { throw new Exception($"JsonParsingError: Expected a boolean value for property {elem.Name} but got a {elem.Value.ValueKind}"); }
                            args.Circular = elem.Value.GetBoolean(); 
                            break; 
                        }
                    case "Size":
                        {
                            if (used_height_width) { throw new Exception($"JsonParsingError: Can't use Size and Width/Height Properties together."); }
                            if (elem.Value.GetArrayLength() != 2) { throw new Exception($"JsonParsingError: Size must be an array of length 2."); }
                            if (elem.Value.EnumerateArray().Any(v => v.ValueKind != JsonValueKind.Number)) { throw new Exception($"JsonParsingError: Size must have only number elements."); }
                            args.Width = elem.Value[0].GetInt32();
                            args.Height = elem.Value[1].GetInt32();
                            used_size = true;
                            break;
                        }
                    case "Width":
                        {
                            if (used_size) { throw new Exception($"JsonParsingError: Can't use Size and Width/Height Properties together."); }
                            if (elem.Value.ValueKind != JsonValueKind.Number) { throw new Exception($"JsonParsingError: Expected a number value for property {elem.Name} but got a {elem.Value.ValueKind}"); }
                            args.Width = elem.Value.GetInt32();
                            used_height_width = true;
                            break;
                        }
                    case "Height":
                        {
                            if (used_size) { throw new Exception($"JsonParsingError: Can't use Size and Width/Height Properties together."); }
                            if (elem.Value.ValueKind != JsonValueKind.Number) { throw new Exception($"JsonParsingError: Expected a number value for property {elem.Name} but got a {elem.Value.ValueKind}"); }
                            args.Height = elem.Value.GetInt32();
                            used_height_width = true;
                            break;
                        }
                    case "Framerate": 
                        {
                            if (elem.Value.ValueKind != JsonValueKind.Number) { throw new Exception($"JsonParsingError: Expected a number value for property {elem.Name} but got a {elem.Value.ValueKind}"); }
                            args.Framerate = elem.Value.GetUInt32(); 
                            break; 
                        }
                    case "FunctionRH":
                        {
                            if (elem.Value.ValueKind != JsonValueKind.String) { throw new Exception($"JsonParsingError: Expected a string value for property {elem.Name} but got a {elem.Value.ValueKind}"); }
                            args.FunctionRH = elem.Value.GetString();
                            args.UseRGB = true;
                            break;
                        }
                    case "FunctionGS":
                        {
                            if (elem.Value.ValueKind != JsonValueKind.String) { throw new Exception($"JsonParsingError: Expected a string value for property {elem.Name} but got a {elem.Value.ValueKind}"); }
                            args.FunctionGS = elem.Value.GetString();
                            args.UseRGB = true;
                            break;
                        }
                    case "FunctionBV":
                        {
                            if (elem.Value.ValueKind != JsonValueKind.String) { throw new Exception($"JsonParsingError: Expected a string value for property {elem.Name} but got a {elem.Value.ValueKind}"); }
                            args.FunctionBV = elem.Value.GetString();
                            args.UseRGB = true;
                            break;
                        }
                    case "FunctionAlpha": 
                        {
                            if (elem.Value.ValueKind != JsonValueKind.String) { throw new Exception($"JsonParsingError: Expected a string value for property {elem.Name} but got a {elem.Value.ValueKind}"); }
                            args.FunctionAlpha = elem.Value.GetString(); 
                            break; 
                        }
                    case "AddOns":
                        {
                            if (elem.Value.ValueKind != JsonValueKind.Array) { throw new Exception($"JsonParsingError: Expected an array value for property {elem.Name} but got a {elem.Value.ValueKind}"); }
                            if (elem.Value.EnumerateArray().Any(v => v.ValueKind != JsonValueKind.String)) { throw new Exception($"JsonParsingError: AddOns property must only contain string elements"); }
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
        private State TryGetState(JsonElement element, GenerationArgs args)
        {
            State s = new State();

            foreach (JsonProperty elem in element.EnumerateObject())
            {
                // treat name and parmeters seperately, cause they are not numbers
                if (elem.Name == "Name") { s.Name = elem.Value.GetString(); continue; }
                if (elem.Name == "Parameters")
                {
                    int i = 0;
                    foreach (JsonElement param_elem in elem.Value.EnumerateArray())
                    {
                        double param_value;
                        if (param_elem.ValueKind == JsonValueKind.String)
                        {
                            Expression expr = new Expression(param_elem.GetString());
                            Helper.ExprRegisterStateProperties(ref expr, s);
                            Helper.ExprRegisterGenArgs(ref expr, args);
                            param_value = (double)expr.Evaluate();
                        }
                        else if (param_elem.ValueKind == JsonValueKind.Number) { param_value = param_elem.GetDouble(); }
                        else { return null; }
                        s.Parameters[i] = param_value;
                        i++;
                    }
                    continue;
                }

                // retrieve the value beforehand
                double value;
                if (elem.Value.ValueKind == JsonValueKind.String)
                {
                    Expression expr = new Expression(elem.Value.GetString());
                    Helper.ExprRegisterStateProperties(ref expr, s);
                    Helper.ExprRegisterGenArgs(ref expr, args);
                    value = (double)expr.Evaluate();
                }
                else if (elem.Value.ValueKind == JsonValueKind.Number) { value = elem.Value.GetDouble(); }
                else { return null; }
                
                switch (elem.Name)
                {
                    case "X0": { s.X0 = value; break; }
                    case "Y0": { s.Y0 = value; break; }
                    case "XRotationCenter": { s.XRotationCenter = value; break; }
                    case "YRotationCenter": { s.YRotationCenter = value; break; }
                    case "XFactor": { s.XFactor = value; break; }
                    case "YFactor": { s.YFactor = value; break; }
                    case "Rotation": { s.Rotation = value; break; }
                    
                    case "Mod": { s.Mod = value; break; }
                    case "ModLimLow": { s.ModLimLow = value; break; }
                    case "ModLimUp": { s.ModLimUp = value; break; }

                    case "ColorRedHue": { s.ColorRedHue = value; break; }
                    case "ColorGreenSaturation": { s.ColorGreenSaturation = value; break; }
                    case "ColorBlueValue": { s.ColorBlueValue = value; break; }
                    case "InvColorRedHue": { s.InvColorRedHue = value; break; }
                    case "InvColorGreenSaturation": { s.InvColorGreenSaturation = value; break; }
                    case "InvColorBlueValue": { s.InvColorBlueValue = value; break; }

                    case "ColorAlpha": { s.ColorAlpha = value; break; }
                    case "InvColorAlpha": { s.InvColorAlpha = value; break; }

                    case "ColorFactorHR": { s.ColorFactorRH = value; break; }
                    case "ColorFactorSG": { s.ColorFactorGS = value; break; }
                    case "ColorFactorVB": { s.ColorFactorBV = value; break; }

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

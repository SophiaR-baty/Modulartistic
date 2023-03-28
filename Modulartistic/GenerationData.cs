using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Linq;
using System.Globalization;
using System.Reflection;

namespace Modulartistic
{
    public class GenerationData
    {
        #region Properties
        public List<Object> Data { get => data; }

        [JsonIgnore]
        public int Count => data.Count;

        public object this[int index] { get => data[index]; }
        #endregion

        #region Fields
        private List<Object> data;
        #endregion

        #region Constructors
        public GenerationData() 
        {
            data = new List<Object>();
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
                WriteIndented = true,
                Converters = 
                { 
                    new DictionaryTKeyEnumTValueConverter(),
                },
            };
            return JsonSerializer.Serialize(this.data, options);
            // return Newtonsoft.Json.JsonConvert.SerializeObject(this.data, Newtonsoft.Json.Formatting.Indented);
        }
        
        public void SaveJson(string path)
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }

            File.WriteAllText(path, ToJson());
        }

        public void LoadJson(string path)
        {
            if (!File.Exists(path))
            {
                throw new FileNotFoundException("The specified File " + path + " does not exist. ");
            }

            string jsontext = File.ReadAllText(path);

            JsonDocument jd = JsonDocument.Parse(jsontext);
            
            
            JsonElement root = jd.RootElement;

            if (root.ValueKind != JsonValueKind.Array)
            {
                throw new Exception("Error: Expected ArrayType RootElement in Json File " + path);
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
                else { throw new Exception("Parsing Error in file " + path + ": Unrecognized Type"); }
            }
        }
        #endregion

        #region Generating Methods
        public void GenerateAll(string path_out = @"")
        {
            GenerationArgs currentArgs = new GenerationArgs();
            for (int i = 0; i < Count; i++)
            {
                object obj = Data[i];
                
                if (obj.GetType() == typeof(GenerationArgs))
                {
                    currentArgs = (GenerationArgs)obj;
                }
                else if (obj.GetType() == typeof(State))
                {
                    (obj as State).GenerateImage(currentArgs, path_out);
                }
                else if (obj.GetType() == typeof(StateSequence))
                {
                    (obj as StateSequence).GenerateAnimation(currentArgs, path_out);
                }
                else if (obj.GetType() == typeof(StateTimeline))
                {
                    (obj as StateTimeline).GenerateAnimation(currentArgs, path_out);
                }
                else
                {
                    throw new Exception();
                }
            }
        }
        #endregion
    }
}

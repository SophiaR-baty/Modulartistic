using System.Collections.Generic;

namespace Modulartistic
{
    public class GenerationArgs
    {
        #region Properties
        public string Function { get; set; }
        public int[] Size { get; set; }
        public uint Framerate { get; set; }
        public List<string> AddOns { get; set; }
        #endregion

        public GenerationArgs() 
        { 
            Size = new int[2] { 500, 500 };
            Function = "x*y";
            Framerate = 12;
            AddOns = new List<string>();
        }
    }
}

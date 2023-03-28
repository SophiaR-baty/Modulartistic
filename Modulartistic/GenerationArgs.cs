using System;
using System.Collections.Generic;
using System.Text;

namespace Modulartistic
{
    public class GenerationArgs
    {
        #region Properties
        public string Function { get; set; }
        public int[] Size { get; set; }
        public uint Framerate { get; set; }
        #endregion

        public GenerationArgs() 
        { 
            Size = new int[2] { 500, 500 };
            Function = "x*y";
            Framerate = 12;
        }
        public GenerationArgs(string func, int[] size, uint framerate = 12) 
        {
            Function = func;
            Size = size;
            Framerate = framerate;
        }

    }
}

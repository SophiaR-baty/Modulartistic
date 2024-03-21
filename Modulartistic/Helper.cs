using System;
using Modulartistic.Core;
using System.Collections.Generic;
using System.Text;
using System.IO;
using NCalc;

namespace Modulartistic
{
    public static class Helper
    {
        public static void CreateDemos()
        {
            string demofilesfolder = AppDomain.CurrentDomain.BaseDirectory + Path.DirectorySeparatorChar + "demofiles";
            string name;

            // delete contents of demofiles folder
            DirectoryInfo di = new DirectoryInfo(demofilesfolder);
            foreach (FileInfo file in di.EnumerateFiles())
            {
                file.Delete();
            }
            foreach (DirectoryInfo dir in di.EnumerateDirectories())
            {
                dir.Delete(true);
            }

            #region State Example 1
            /* Example 1 will create a simple 500x500 State with x*y HueFunction
             * and all other Color values set to 1 
             */
            name = "state_example_1";
            if (!File.Exists(demofilesfolder + Path.DirectorySeparatorChar + name))
            {
                // generation data
                GenerationData GD = new GenerationData();

                // Generation Args
                GenerationArgs GA = new GenerationArgs()
                {
                    Width = 500,
                    Height = 500,
                    FunctionRH = "x*y",
                    Circular = true,
                };
                GD.Add(GA);

                // add standard State
                State S = new State()
                {
                    Name = name,
                    ColorGreenSaturation = 1,
                    ColorBlueValue = 1,
                    ColorAlpha = 1,
                };
                GD.Add(S);

                // Save Generation Data
                GD.Name = name;
                GD.SaveJson();
            }
            #endregion

            #region State Example 2
            /* Example 2 will create a simple 500x500 image
             * showcasing UseRGB and using multiple functions
             * at once. 
             * Also since Circular is false, ColorAlpha is 0.99
             */
            name = "state_example_2";
            if (!File.Exists(demofilesfolder + Path.DirectorySeparatorChar + name))
            {
                // generation data
                GenerationData GD = new GenerationData();

                // Generation Args
                GenerationArgs GA = new GenerationArgs()
                {
                    Width = 500,
                    Height = 500,
                    FunctionRH = "x*x + y*y",
                    FunctionGS = "(x+1)*(x+1) + y*y",
                    FunctionBV = "(x-1)*(x-1) + y*y",
                    Circular = false,
                    UseRGB = true,
                };
                GD.Add(GA);

                // add standard State
                State S = new State()
                {
                    Name = name,
                    ColorRedHue = 0,
                    ColorGreenSaturation = 0,
                    ColorBlueValue = 0,
                    ColorAlpha = 1,
                };
                GD.Add(S);

                // Save Generation Data
                GD.Name = name;
                GD.SaveJson();
            }
            #endregion

            #region State Example 3
            /* Example 3 will be the same as Example 2
             * but with circular = true to see the difference
             */
            name = "state_example_3";
            if (!File.Exists(demofilesfolder + Path.DirectorySeparatorChar + name))
            {
                // generation data
                GenerationData GD = new GenerationData();

                // Generation Args
                GenerationArgs GA = new GenerationArgs()
                {
                    Width= 500,
                    Height = 500,
                    FunctionRH = "x*x + y*y",
                    FunctionGS = "(x+1)*(x+1) + y*y",
                    FunctionBV = "(x-1)*(x-1) + y*y",
                    Circular = true,
                    UseRGB = true,
                };
                GD.Add(GA);

                // add standard State
                State S = new State()
                {
                    Name = name,
                    ColorRedHue = 0,
                    ColorGreenSaturation = 0,
                    ColorBlueValue = 0,
                    ColorAlpha = 1,
                };
                GD.Add(S);

                // Save Generation Data
                GD.Name = name;
                GD.SaveJson();
            }
            #endregion

            #region State Example 4
            /* Example 4 showcases invalid color global and 
             * invalid color in general. It also introduces
             * Function for Alpha
             */
            name = "state_example_4";
            if (!File.Exists(demofilesfolder + Path.DirectorySeparatorChar + name))
            {
                // generation data
                GenerationData GD = new GenerationData();

                // Generation Args
                GenerationArgs GA = new GenerationArgs()
                {
                    Width = 500,
                    Height = 500,
                    FunctionAlpha = "1/x + 1/y + 1/(x+y)",
                    InvalidColorGlobal = true,
                    UseRGB = true,
                    Circular = false,
                };
                GD.Add(GA);

                // add standard State
                State S = new State()
                {
                    Name = name,
                    Mod = 1,
                    ModLimLow = 0,
                    ModLimUp = 1,
                    ColorRedHue = 1,
                    ColorGreenSaturation = 1,
                    ColorBlueValue = 1,

                    InvColorRedHue = 1,
                    InvColorAlpha = 1,

                };
                GD.Add(S);

                // Save Generation Data
                GD.Name = name;
                GD.SaveJson();
            }
            #endregion

            #region StateSequence Example 1
            /* Example 1 will create a simple 500x500 StateSequence Animation using the same Configuration as in
             * State Example 1. It will animate over the Hue with standard linear easing.
             */
            name = "state_sequence_example_1";
            if (!File.Exists(demofilesfolder + Path.DirectorySeparatorChar + name))
            {
                // generation data
                GenerationData GD = new GenerationData();

                // Generation Args
                GenerationArgs GA = new GenerationArgs()
                {
                    Width = 500,
                    Height = 500,
                    FunctionRH = "x*y",
                    Framerate = 12,
                    Circular = true,
                };
                GD.Add(GA);

                // create 2 States (standard state and one with hue shifted)
                State S1 = new State()
                {
                    Name = "scene1",
                    ColorGreenSaturation = 1,
                    ColorBlueValue = 1,
                    ColorAlpha = 1,
                };
                State S2 = new State()
                {
                    Name = "scene2",
                    ColorGreenSaturation = 1,
                    ColorBlueValue = 1,
                    ColorAlpha = 1,
                    ColorRedHue = 360,
                };

                // create new StateSequence and add the states inside scenes
                StateSequence SS = new StateSequence(name);
                SS.Scenes.Add(new Scene(S1, 3, Easing.Linear()));
                SS.Scenes.Add(new Scene(S2, 3, Easing.Linear()));

                // add the StateSequence to the GenerationData
                GD.Add(SS);

                // Save Generation Data
                GD.Name = name;
                GD.SaveJson();
            }
            #endregion
        }

        public static void PrintUsage()
        {
            Console.WriteLine("Usage: ");
            Console.WriteLine("Modulartistic.exe generate [<generationData.json>] [output_directory] [<flags>]");
            Console.WriteLine("Generates all images and animations defined in generationData.json. They will be created in the output directory. \n");

            Console.WriteLine("Write 'Modulartistic.exe command --help' to get more information about any command. \n");

            Console.WriteLine("Visit https://github.com/MaxGeo543/Modulartistic for more information. \n");
        }

        public static void CreateDirectories()
        {
            string basefolder = AppDomain.CurrentDomain.BaseDirectory + Path.DirectorySeparatorChar;
            foreach (string folder in new string[] { "demofiles", "output", "addons", "input" })
            {
                if (!Directory.Exists(basefolder + folder)) { Directory.CreateDirectory(basefolder + folder); }
            }
        }
    }
}

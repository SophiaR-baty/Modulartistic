using System;
using System.Collections;
using System.IO;
using System.Reflection;
using System.Text.Json;
using System.Xml.Linq;
using AnimatedGif;
using NCalc;
using Modulartistic.Common;

namespace Modulartistic.Core
{
    /// <summary>
    /// Provides utility methods for various operations related to file handling, animation formats, mathematical operations, and expression registration.
    /// </summary>
    public static class Helper
    {
        /// <summary>
        /// Generates a valid file name by appending an index if a file or directory with the same name already exists.
        /// </summary>
        /// <param name="filename">The base name of the file or directory.</param>
        /// <returns>A unique file or directory name by appending an index if necessary.</returns>
        /// <remarks>
        /// The method checks for existing files with common extensions and directories. If a file or directory with the given name already exists, it appends an index to generate a unique name.
        /// </remarks>
        public static string GetValidFileName(string filename)
        {
            if (File.Exists(filename + @".mp4") || File.Exists(filename + @".png") || File.Exists(filename + @".gif") || File.Exists(filename + @".txt") || File.Exists(filename + @".json") || Directory.Exists(filename))
            {
                int i = 1;
                while (File.Exists(filename + "_" + i + @".mp4") || File.Exists(filename + "_" + i + @".png") || File.Exists(filename + "_" + i + @".gif") || File.Exists(filename + "_" + i + @".txt") || File.Exists(filename + "_" + i + @".json") || Directory.Exists(filename + "_" + i))
                {
                    i++;
                }
                filename = filename + "_" + i;
            }

            return filename;
        }

        /// <summary>
        /// Gets the file extension for a given <see cref="AnimationFormat"/>.
        /// </summary>
        /// <param name="type">The animation format type.</param>
        /// <returns>The file extension associated with the specified animation format.</returns>
        /// <exception cref="ArgumentException">Thrown if the <paramref name="type"/> is <see cref="AnimationFormat.None"/>.</exception>
        /// <exception cref="NotImplementedException">Thrown if the <paramref name="type"/> is not supported.</exception>
        public static string GetAnimationFormatExtension(AnimationFormat type)
        {
            switch (type)
            {
                case AnimationFormat.None:
                    throw new ArgumentException("No AnimationFormat specified");
                case AnimationFormat.Gif:
                    return "gif";
                case AnimationFormat.Mp4:
                    return "mp4";
                default:
                    throw new NotImplementedException("Only gif and mpp4 are supported AnimationFormats");
            }
        }

        /// <summary>
        /// Calculates the modulus of two double values.
        /// </summary>
        /// <param name="d1">The dividend.</param>
        /// <param name="d2">The divisor.</param>
        /// <returns>The remainder after dividing <paramref name="d1"/> by <paramref name="d2"/>.</returns>
        /// <exception cref="DivideByZeroException">Thrown if <paramref name="d2"/> is less than or equal to zero.</exception>
        public static double Mod(double d1, double d2)
        {
            if (d2 <= 0)
                throw new DivideByZeroException();
            else
                return d1 - d2 * Math.Floor(d1 / d2);
        }

        /// <summary>
        /// Calculates the inclusive modulus of two double values.
        /// </summary>
        /// <param name="d1">The dividend.</param>
        /// <param name="d2">The divisor.</param>
        /// <returns>The remainder after dividing <paramref name="d1"/> by <paramref name="d2"/>. If the result is zero and <paramref name="d1"/> is not zero, returns <paramref name="d2"/> instead.</returns>
        public static double InclusiveMod(double d1, double d2)
        {
            double result = Mod(d1, d2);
            return d1 != 0 && result == 0 ? d2 : result;
        }

        /// <summary>
        /// Calculates a circular modulus to find the closest distance on a circular range.
        /// </summary>
        /// <param name="d1">The dividend.</param>
        /// <param name="d2">The divisor, representing the circular range.</param>
        /// <returns>The closest distance within a circular range defined by <paramref name="d2"/>.</returns>
        public static double CircularMod(double d1, double d2)
        {
            double result = Mod(d1, d2);
            return result < d2 / 2 ? 2*result : 2*(d2 - result);
        }

        /// <summary>
        /// Gets the absolute path for an add-on, converting a relative path to an absolute path using the provided path provider.
        /// </summary>
        /// <param name="path">The relative path to convert.</param>
        /// <param name="pathProvider">An object that provides the base directory for add-ons.</param>
        /// <returns>The absolute path for the add-on.</returns>
        /// <remarks>
        /// If <paramref name="path"/> is already an absolute path, it is returned as-is. Otherwise, the method uses <paramref name="pathProvider"/> 
        /// to obtain the base directory for add-ons and combines it with the relative <paramref name="path"/> to generate the absolute path.
        /// </remarks>
        public static string GetAddOnPath(string path, IPathProvider pathProvider)
        {
            if (Path.IsPathRooted(path))
            {
                // Path is already absolute
                return path;
            }
            else
            {
                // Convert to absolute using the current working directory
                string addonDir = pathProvider.GetAddonPath();
                return Path.Combine(addonDir, path);
            }
        }

        /// <summary>
        /// Registers state properties to an <see cref="ExtendedExpression"/> object.
        /// </summary>
        /// <param name="expr">The <see cref="ExtendedExpression"/> object to which properties are added.</param>
        /// <param name="s">The <see cref="State"/> object containing the properties to register.</param>
        public static void RegisterStateProperties(this ExtendedExpression expr, State s)
        {
            foreach (PropertyInfo propInf in typeof(State).GetProperties())
            {
                switch (propInf.Name)
                {
                    case nameof(State.X0):
                    case nameof(State.Y0):
                    case nameof(State.XRotationCenter):
                    case nameof(State.YRotationCenter):
                    case nameof(State.XFactor):
                    case nameof(State.YFactor):
                    case nameof(State.Rotation):
                    case nameof(State.Mod):
                    case nameof(State.ModLowerLimit):
                    case nameof(State.ModUpperLimit):
                    case nameof(State.ColorRedHue):
                    case nameof(State.ColorGreenSaturation):
                    case nameof(State.ColorBlueValue):
                    case nameof(State.ColorAlpha):
                    case nameof(State.InvalidColorRedHue):
                    case nameof(State.InvalidColorGreenSaturation):
                    case nameof(State.InvalidColorBlueValue):
                    case nameof(State.InvalidColorAlpha):
                    case nameof(State.ColorFactorRedHue):
                    case nameof(State.ColorFactorGreenSaturation):
                    case nameof(State.ColorFactorBlueValue):
                        expr.RegisterParameter($"{nameof(State)}_{propInf.Name}", propInf.GetValue(s));
                        break;
                    default:
                        continue;
                }
            }

            int i = 0;
            foreach (int p in s.Parameters)
            {
                expr.RegisterParameter($"i_{i}", p);
                i++;
            }

            expr.RegisterParameter("i", s.Parameters[0]);
            expr.RegisterParameter("j", s.Parameters[1]);
        }

        /// <summary>
        /// Registers state options to an <see cref="ExtendedExpression"/> object.
        /// </summary>
        /// <param name="expr">The <see cref="ExtendedExpression"/> object to which properties are added.</param>
        /// <param name="args">The <see cref="StateOptions"/> object containing the properties to register.</param>
        public static void RegisterStateOptions(this ExtendedExpression expr, StateOptions args)
        {
            expr.RegisterParameter("img_width", (double)args.Width);
            expr.RegisterParameter("img_height", (double)args.Height);
            expr.RegisterParameter("ani_framerate", (double)args.Framerate);
        }

        /// <summary>
        /// Registers state options and state properties to an <see cref="Common.ExtendedExpression"/> object.
        /// </summary>
        /// <param name="ex">The <see cref="Common.ExtendedExpression"/> object to which properties are added.</param>
        /// <param name="s">The <see cref="State"/> object containing the properties to register.</param>
        /// <param name="args">The <see cref="StateOptions"/> object containing the properties to register.</param>
        public static void RegisterStateAndOptionsProperties(this Common.ExtendedExpression ex, State s, StateOptions args)
        {
            ex.RegisterStateProperties(s);
            ex.RegisterStateOptions(args);
        }

        /// <summary>
        /// Load AddOns from <see cref="StateOptions"/>
        /// </summary>
        /// <param name="expr">The <see cref="ExtendedExpression"/> to load AddOns for</param>
        /// <param name="sOpts">The <see cref="StateOptions"/> to load AddOns form</param>
        /// <param name="gOpts">The <see cref="GenerationOptions"/> used for Logging</param>
        public static void LoadAddOns(this ExtendedExpression expr, StateOptions sOpts, GenerationOptions gOpts)
        {
            IEnumerable<string> dll_paths = sOpts.AddOns;

            IPathProvider pathProvider = gOpts.PathProvider;
            AddOnInitializationArgs initArgs = new AddOnInitializationArgs()
            {
                Framerate = sOpts.Framerate,
                Width = sOpts.Width,
                Height = sOpts.Height,
                Logger = gOpts.Logger,
                ProgressReporter = gOpts.ProgressReporter,
                Guid = gOpts.GenerationDataGuid,
                AddOns = sOpts.AddOns.ToArray(),
                AddOnDir = pathProvider.GetAddonPath(),

            };
            foreach (string dll in dll_paths)
            {
                expr.LoadAddOn(GetAddOnPath(dll, pathProvider), initArgs);
            }
        }


        #region test embedding guid in files

        public static void EmbedGuid(string file_path, Guid guid)
        {
            if (!File.Exists(file_path)) { throw new FileNotFoundException($"the specified file {file_path} was not found."); }

            string format = Path.GetExtension(file_path);
            switch (format)
            {
                case (".png"):
                    EmbedGuidInPng(file_path, guid);
                    break;
                case (".gif"):
                    EmbedGuidInGif(file_path, guid);
                    break;
                case (".mp4"):
                    EmbedGuidInMp4(file_path, guid);
                    break;
            }
        }

        private static void EmbedGuidInPng(string file_path, Guid guid)
        {
            byte[] guidBytes = guid.ToByteArray();

            // Read the original PNG file
            byte[] originalFileBytes = File.ReadAllBytes(file_path);

            using (FileStream fs = new FileStream(file_path, FileMode.Open, FileAccess.Write))
            {
                // Write the original file content
                fs.Write(originalFileBytes, 0, originalFileBytes.Length);

                // Add a custom chunk with the GUID
                byte[] chunkType = { 0x67, 0x75, 0x69, 0x64 }; // Chunk type "guid"
                byte[] chunkDataLength = BitConverter.GetBytes(guidBytes.Length);
                byte[] chunkCRC = { 0x00, 0x00, 0x00, 0x00 }; // Placeholder CRC

                // Write the custom chunk to the file
                fs.Write(chunkDataLength, 0, chunkDataLength.Length);
                fs.Write(chunkType, 0, chunkType.Length);
                fs.Write(guidBytes, 0, guidBytes.Length);
                fs.Write(chunkCRC, 0, chunkCRC.Length);
            }
        }
        
        private static void EmbedGuidInGif(string file_path, Guid guid)
        {
            byte[] guidBytes = guid.ToByteArray();

            string tmp_out = Path.Join(Path.GetDirectoryName(file_path), "_"+Path.GetFileName(file_path));

            using (FileStream inputFileStream = new FileStream(file_path, FileMode.Open, FileAccess.Read))
            using (FileStream outputFileStream = new FileStream(tmp_out, FileMode.Create))
            {
                byte[] header = new byte[6];
                inputFileStream.Read(header, 0, header.Length);
                outputFileStream.Write(header, 0, header.Length);

                // Write the rest of the file until the extension block
                byte[] buffer = new byte[4096];
                int bytesRead;
                bool foundExtension = false;
                while ((bytesRead = inputFileStream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    int i;
                    for (i = 0; i < bytesRead; i++)
                    {
                        if (buffer[i] == 0x21) // Extension introducer
                        {
                            if (buffer[i + 1] == 0xFF) // Application extension
                            {
                                foundExtension = true;
                                break;
                            }
                        }
                    }

                    if (foundExtension)
                    {
                        outputFileStream.Write(buffer, 0, i + 2); // Write up to the extension block
                                                                  // Write the custom extension data
                        outputFileStream.Write(new byte[] { 0x21, 0xFF, 0x0B, (byte)'G', (byte)'U', (byte)'I', (byte)'D', 0x00 }, 0, 8);
                        outputFileStream.Write(guidBytes, 0, guidBytes.Length);
                        outputFileStream.Write(new byte[] { 0x00 }, 0, 1); // Terminate extension block
                    }
                    else
                    {
                        outputFileStream.Write(buffer, 0, bytesRead);
                    }
                }
            }

            File.Copy(tmp_out, file_path, true);
            File.Delete(tmp_out);
        }
    
        private static void EmbedGuidInMp4(string file_path, Guid guid)
        {
            byte[] guidBytes = guid.ToByteArray();

            string tmp_out = Path.Join(Path.GetDirectoryName(file_path), "_" + Path.GetFileName(file_path));

            using (FileStream inputFileStream = new FileStream(file_path, FileMode.Open, FileAccess.Read))
            using (FileStream outputFileStream = new FileStream(tmp_out, FileMode.Create))
            {
                byte[] buffer = new byte[4096];
                int bytesRead;
                bool foundMoov = false;
                while ((bytesRead = inputFileStream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    int i;
                    for (i = 0; i < bytesRead; i++)
                    {
                        if (buffer[i] == 'm' && buffer[i + 1] == 'o' && buffer[i + 2] == 'o' && buffer[i + 3] == 'v')
                        {
                            foundMoov = true;
                            break;
                        }
                    }

                    if (foundMoov)
                    {
                        outputFileStream.Write(buffer, 0, i);
                        // Write custom udta box
                        byte[] customBox = new byte[]
                        {
                        // Box size (data size + 8 bytes for box header)
                        0x00, 0x00, 0x00, 0x14, // Replace 0x14 with the size of the box
                        // Box type 'udta'
                        (byte)'u', (byte)'d', (byte)'t', (byte)'a',
                        // Custom data (example: GUID)
                        0x00, 0x00, 0x00, 0x0C, (byte)'G', (byte)'U', (byte)'I', (byte)'D', 0x00
                        };
                        outputFileStream.Write(customBox, 0, customBox.Length);
                        outputFileStream.Write(guidBytes, 0, guidBytes.Length);
                    }
                    else
                    {
                        outputFileStream.Write(buffer, 0, bytesRead);
                    }
                }
            }

            File.Copy(tmp_out, file_path, true);
            File.Delete(tmp_out);
        }

        

        #endregion

    }
}

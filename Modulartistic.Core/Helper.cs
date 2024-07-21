﻿using System;
using System.IO;
using AnimatedGif;
using NCalc;

namespace Modulartistic.Core
{
    /// <summary>
    /// Provides utility methods for various operations related to file handling, animation formats, mathematical operations, and expression registration.
    /// </summary>
    public class Helper
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
        /// Registers state properties to an <see cref="Expression"/> object.
        /// </summary>
        /// <param name="expr">The <see cref="Expression"/> object to which properties are added.</param>
        /// <param name="s">The <see cref="State"/> object containing the properties to register.</param>
        public static void ExprRegisterStateProperties(ref Expression expr, State s)
        {
            expr.Parameters["x_0"] = s.X0;
            expr.Parameters["y_0"] = s.Y0;
            expr.Parameters["x_fact"] = s.XFactor;
            expr.Parameters["y_fact"] = s.YFactor;
            expr.Parameters["x_rotcent"] = s.XRotationCenter;
            expr.Parameters["y_rotcent"] = s.YRotationCenter;
            expr.Parameters["rotation"] = s.Rotation;

            expr.Parameters["modnum"] = s.Mod;
            expr.Parameters["num"] = s.Mod;
            expr.Parameters["modlimlow"] = s.ModLowerLimit;
            expr.Parameters["modlimup"] = s.ModUpperLimit;

            expr.Parameters["col_rh"] = s.ColorRedHue;
            expr.Parameters["col_gs"] = s.ColorGreenSaturation;
            expr.Parameters["col_bv"] = s.ColorBlueValue;
            expr.Parameters["col_alp"] = s.ColorAlpha;

            expr.Parameters["inv_col_rh"] = s.InvalidColorRedHue;
            expr.Parameters["inv_col_gs"] = s.InvalidColorGreenSaturation;
            expr.Parameters["inv_col_bv"] = s.InvalidColorBlueValue;
            expr.Parameters["inv_col_alp"] = s.InvalidColorAlpha;

            expr.Parameters["col_fact_rh"] = s.ColorFactorRedHue;
            expr.Parameters["col_fact_gs"] = s.ColorFactorGreenSaturation;
            expr.Parameters["col_fact_bv"] = s.ColorFactorBlueValue;

            expr.Parameters["i_0"] = s.Parameters[0];
            expr.Parameters["i"] = s.Parameters[0];
            expr.Parameters["i_1"] = s.Parameters[1];
            expr.Parameters["j"] = s.Parameters[1];
            expr.Parameters["i_2"] = s.Parameters[2];
            expr.Parameters["i_3"] = s.Parameters[3];
            expr.Parameters["i_4"] = s.Parameters[4];
            expr.Parameters["i_5"] = s.Parameters[5];
            expr.Parameters["i_6"] = s.Parameters[6];
            expr.Parameters["i_7"] = s.Parameters[7];
            expr.Parameters["i_8"] = s.Parameters[8];
            expr.Parameters["i_9"] = s.Parameters[9];
        }

        /// <summary>
        /// Registers state options to an <see cref="Expression"/> object.
        /// </summary>
        /// <param name="expr">The <see cref="Expression"/> object to which properties are added.</param>
        /// <param name="args">The <see cref="StateOptions"/> object containing the properties to register.</param>
        public static void ExprRegisterStateOptions(ref Expression expr, StateOptions args)
        {
            expr.Parameters["img_width"] = (double)args.Width;
            expr.Parameters["img_height"] = (double)args.Height;
            expr.Parameters["ani_framerate"] = (double)args.Framerate;
        }
    }
}

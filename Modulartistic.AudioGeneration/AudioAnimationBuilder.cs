using System;
using System.Collections.Generic;
using System.Linq;
using NCalc;
using System.Text;
using System.Threading.Tasks;
using Antlr4.Runtime.Misc;
using FFMpegCore.Extensions.SkiaSharp;
using FFMpegCore.Pipes;
using Modulartistic.Core;
using NAudio.Wave;
using System.Reflection;
using FFMpegCore;
using Modulartistic.Drawing;

namespace Modulartistic.AudioGeneration
{
    public class AudioAnimationBuilder
    {
        public string File { get; set; }
        public StateOptions Options { get; set; }
        public State State { get; set; }
        public Dictionary<StateProperty, string> StatePropertyFunctions { get; set; }

        public AudioAnimationBuilder(string path) 
        {
            File = path;
            Options = new StateOptions();
            State = new State();
            StatePropertyFunctions = new Dictionary<StateProperty, string>();
        }

        private IEnumerable<IVideoFrame> EnumerateFrames(GenerationOptions options)
        {
            // parses GenerationArgs
            uint framerate = Options.Framerate;

            AudioAnalysis audioAnalysis = new AudioAnalysis(File, (int)framerate);
            int framecount = (int)((audioAnalysis.AudioLength.TotalSeconds + 3) * framerate);

            Progress? visProgress = options.ProgressReporter?.AddTask($"{Guid.NewGuid()}", $"Audio visualization...", framecount);
            // loops through the scenes
            for (int i = 0; i < framecount; i++)
            {
                foreach (KeyValuePair<StateProperty, string> kvp in StatePropertyFunctions)
                {
                    State[kvp.Key] = EvaluateFunction(kvp.Value, audioAnalysis, i);
                }

                yield return new BitmapVideoFrameWrapper(State.GetBitmap(Options, options));
                visProgress?.IncrementProgress();
            }
            options.ProgressReporter?.RemoveTask(visProgress);
        }

        private string GenerateFrames(GenerationOptions options, string frames_dir)
        {
            // parses GenerationArgs
            uint framerate = Options.Framerate;

            AudioAnalysis audioAnalysis = new AudioAnalysis(File, (int)framerate);
            int framecount = (int)((audioAnalysis.AudioLength.TotalSeconds + 3) * framerate);

            // create Directory for frames if not exist
            if (!Directory.Exists(frames_dir)) { Directory.CreateDirectory(frames_dir); }

            Progress? visProgress = options.ProgressReporter?.AddTask($"{Guid.NewGuid()}", $"Audio visualization...", framecount);
            // loops through the scenes
            for (int i = 0; i < framecount; i++)
            {
                foreach (KeyValuePair<StateProperty, string> kvp in StatePropertyFunctions)
                {
                    State[kvp.Key] = EvaluateFunction(kvp.Value, audioAnalysis, i);
                }

                State.GenerateImage(Options, options, frames_dir);
            }
            options.ProgressReporter?.RemoveTask(visProgress);

            return frames_dir;
        }

        private async Task CreateAnimation(GenerationOptions options, AnimationFormat type, string absolute_out_filepath)
        {
            // parsing framerate and setting piping source
            uint framerate = Options.Framerate;
            var videoFramesSource = new RawVideoPipeSource(EnumerateFrames(options))
            {
                FrameRate = framerate, // set source frame rate
            };

            switch (type)
            {
                case AnimationFormat.Gif:
                    // parsing size
                    System.Drawing.Size size = new System.Drawing.Size(Options.Width, Options.Height);

                    // generate the gif file
                    try
                    {
                        await FFMpegArguments
                        .FromPipeInput(videoFramesSource)
                        .OutputToFile(absolute_out_filepath + @".gif", false, options => options
                            .WithGifPaletteArgument(0, size, (int)framerate)
                            .WithFramerate(framerate))
                        .ProcessAsynchronously();
                    }
                    catch (Exception e)
                    {
                        throw new Exception("Error generating animation. ", e);
                    }
                    break;
                case AnimationFormat.Mp4:
                    // generate the mp4 file
                    try
                    {
                        await FFMpegArguments
                        .FromPipeInput(videoFramesSource)
                        .OutputToFile(absolute_out_filepath + @".mp4", false, options => options
                            // .WithVideoCodec(VideoCodec.LibX265)
                            // .WithVideoBitrate(16000) // find a balance between quality and file size
                            .WithFramerate(framerate))
                        .ProcessAsynchronously();
                    }
                    catch (Exception e)
                    {
                        throw new Exception("Error generating animation. ", e);
                    }
                    break;
                default:
                    throw new NotImplementedException("only gif and mp4 are supported for animation cration");
            }
        }

        private async Task CreateAnimationFromFolder(GenerationOptions options, AnimationFormat type, string folder)
        {
            // Creating the image list
            List<string> imgPaths = Directory.GetFiles(folder).ToList();

            // Enumerater for image files
            IEnumerable<IVideoFrame> EnumerateFrames()
            {
                // loops through the all img paths
                Progress? joinProgress = options.ProgressReporter?.AddTask($"{Guid.NewGuid().ToString()}", $"Joining frames", imgPaths.Count);
                for (int i = 0; i < imgPaths.Count; i++)
                {
                    joinProgress?.IncrementProgress();
                    yield return new BitmapVideoFrameWrapper(new Bitmap(imgPaths[i]));
                }
                options.ProgressReporter?.RemoveTask(joinProgress);
            }

            uint framerate = Options.Framerate;
            var videoFramesSource = new RawVideoPipeSource(EnumerateFrames())
            {
                FrameRate = framerate, // set source frame rate
            };


            switch (type)
            {
                case AnimationFormat.Gif:
                    // parsing size
                    System.Drawing.Size size = new System.Drawing.Size(Options.Width, Options.Height);

                    // generate the gif file
                    try
                    {
                        await FFMpegArguments
                        .FromPipeInput(videoFramesSource)
                        .OutputToFile(folder + @".gif", false, options => options
                            .WithGifPaletteArgument(0, size, (int)framerate)
                            .WithFramerate(framerate))
                        .ProcessAsynchronously();
                    }
                    catch (Exception e)
                    {
                        throw new Exception("Error generating animation. ", e);
                    }
                    break;
                case AnimationFormat.Mp4:
                    // generate the mp4 file
                    try
                    {
                        await FFMpegArguments
                        .FromPipeInput(videoFramesSource)
                        .OutputToFile(folder + @".mp4", false, options => options
                            // .WithVideoCodec(VideoCodec.LibX265)
                            // .WithVideoBitrate(16000) // find a balance between quality and file size
                            .WithFramerate(framerate))
                        .ProcessAsynchronously();
                    }
                    catch (Exception e)
                    {
                        throw new Exception("Error generating animation. ", e);
                    }
                    break;
                default:
                    throw new NotImplementedException("only gif and mp4 are supported for animation cration");
            }
        }

        public async Task<string> GenerateAnimation(GenerationOptions options, string out_dir)
        {
            // check if it exists
            if (!Directory.Exists(out_dir)) { throw new DirectoryNotFoundException("The Directory " + out_dir + " was not found."); }

            // parse options
            bool keepframes = options.KeepAnimationFrames;
            AnimationFormat type = options.AnimationFormat;

            // set the absolute path for the file to be save
            string file_path_out = Path.Join(out_dir, Path.GetFileNameWithoutExtension(File));
            // Validate (if file with same name exists already, append index)
            file_path_out = Helper.GetValidFileName(file_path_out);

            if (keepframes)
            {
                // the folder where the frames are saved
                string folder = GenerateFrames(options, file_path_out);
                await CreateAnimationFromFolder(options, type, folder);
            }
            else
            {
                await CreateAnimation(options, type, file_path_out);
            }

            return file_path_out + $".{Helper.GetAnimationFormatExtension(type)}";
        }




        private double EvaluateFunction(string func, AudioAnalysis analysis, int idx)
        {
            Expression expression = new Expression(func);
            expression.Parameters[$"Volume"] = analysis.Volumes[idx];
            foreach (PropertyInfo propInf in typeof(Frequencies).GetProperties())
            {
                expression.Parameters[propInf.Name] = propInf.GetValue(analysis);
            }

            return (double)expression.Evaluate();
        }
    }
}


using Khernet.UI.Cache;
using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace Khernet.UI.Media
{
    /// <summary>
    /// Audio and Video formats.
    /// </summary>
    public enum MediaFormat
    {
        GIF = 0,
        AVI = 1,
        MP3 = 2,
        MP4 = 3,
    }
    public static class MediaHelper
    {
        /// <summary>
        /// Converts and video file to a specific format.
        /// </summary>
        /// <param name="inputFile">The path of source video.</param>
        /// <param name="outpuFile">The path of converted video.</param>
        /// <param name="outputFormat">The format to convert to.</param>
        public static void ConvertTo(string inputFile, string outpuFile, MediaFormat outputFormat)
        {
            try
            {
                string argument = string.Format("-y -i \"{0}\" -f {1} \"{2}\"",
                        inputFile,
                        Enum.GetName(typeof(MediaFormat), outputFormat),
                        outpuFile);

                StartProcess(argument);
            }
            catch (Exception)
            {

                throw;
            }
        }

        /// <summary>
        /// Converts and video file to a specific format with given size.
        /// </summary>
        /// <param name="inputFile">The path of source video.</param>
        /// <param name="outpuFile">The path of converted video.</param>
        /// <param name="width">The width of converted video.</param>
        /// <param name="height">The height of converted video.</param>
        public static void ConvertTo(string inputFile, string outpuFile, int width, int height)
        {
            try
            {
                string argument = string.Format("-y -i \"{0}\" -s {1}x{2} \"{3}\"",
                        inputFile,
                        width,
                        height,
                        outpuFile);

                StartProcess(argument);
            }
            catch (Exception)
            {

                throw;
            }
        }

        /// <summary>
        /// Gets image size.
        /// </summary>
        /// <param name="fileName">The path of image file.</param>
        /// <returns><see cref="Size"/> that contains width and height of image.</returns>
        public static Size GetImageSize(string fileName)
        {
            BitmapImage image = new BitmapImage();
            image.BeginInit();
            image.UriSource = new Uri(fileName);
            image.EndInit();

            Size imageSize = new Size(image.Width, image.Height);
            return imageSize;
        }

        /// <summary>
        /// Gets and thumbnail image from video.
        /// </summary>
        /// <param name="videoFile">The path of video.</param>
        /// <param name="outputFile">The path of thumbnail image extracted from video.</param>
        /// <param name="startTime">The point in time from to take thumbnail.</param>
        public static void GetVideoThumbnail(string videoFile, string outputFile, TimeSpan startTime)
        {
            try
            {
                string argument = string.Format("-y -ss {0} -i \"{1}\"  -frames 1 \"{2}\"",
                    startTime.TotalSeconds,
                        videoFile,
                        outputFile);

                StartProcess(argument);
            }
            catch (Exception)
            {

                throw;
            }
        }

        /// <summary>
        /// Starts the ffmpeg.exe tool to do operations with video files.
        /// </summary>
        /// <param name="arguments">The arguments for FFMEPG tool.</param>
        private static void StartProcess(string arguments)
        {
            ProcessStartInfo processInfo = new ProcessStartInfo();
            processInfo.FileName = Path.Combine(Configurations.AppDirectory, "media", "ffmpeg.exe");
            processInfo.Arguments = arguments;
            processInfo.CreateNoWindow = true;
            processInfo.UseShellExecute = false;
            processInfo.WindowStyle = ProcessWindowStyle.Hidden;
            processInfo.RedirectStandardOutput = true;
            processInfo.RedirectStandardError = true;

            Process process = null;
            DataReceivedEventHandler receivedEventHandler = null;
            DataReceivedEventHandler errorReceivedEventHandler = null;

            try
            {
                using (process = new Process())
                {
                    process.StartInfo = processInfo;

                    string errorDetail = string.Empty;
                    receivedEventHandler = new DataReceivedEventHandler((s, e) => { errorDetail = string.Concat(e.Data); });
                    errorReceivedEventHandler = new DataReceivedEventHandler((s, e) => { errorDetail = string.Concat(e.Data); });

                    process.OutputDataReceived += receivedEventHandler;
                    process.ErrorDataReceived += errorReceivedEventHandler;
                    process.Start();
                    process.BeginOutputReadLine();
                    process.BeginErrorReadLine();
                    process.WaitForExit();

                    if (process.ExitCode != 0)
                    {
                        throw new Exception($"Error while processing file: {errorDetail}");
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                if (process != null)
                {
                    process.OutputDataReceived -= receivedEventHandler;
                    process.ErrorDataReceived -= errorReceivedEventHandler;
                }
            }
        }

        /// <summary>
        /// Extracts information about audio and video files using the ffprobe.exe utility.
        /// </summary>
        /// <param name="arguments">The arguments for ffprobe.exe</param>
        /// <returns>A string with the requested information.</returns>
        private static string StartProbeProcess(string arguments)
        {
            ProcessStartInfo processInfo = new ProcessStartInfo();
            processInfo.FileName = Path.Combine(Configurations.AppDirectory, "media", "ffprobe.exe");
            processInfo.Arguments = arguments;
            processInfo.CreateNoWindow = true;
            processInfo.UseShellExecute = false;
            processInfo.WindowStyle = ProcessWindowStyle.Hidden;
            processInfo.RedirectStandardOutput = true;
            processInfo.RedirectStandardError = true;

            Process process = null;
            DataReceivedEventHandler receivedEventHandler = null;
            DataReceivedEventHandler errorReceivedEventHandler = null;

            try
            {
                using (process = new Process())
                {
                    process.StartInfo = processInfo;

                    string result = string.Empty;
                    string errorDetail = string.Empty;
                    receivedEventHandler = new DataReceivedEventHandler((s, e) =>
                    {
                        if (e.Data != null)
                            result += string.Concat(e.Data);
                    });
                    errorReceivedEventHandler = new DataReceivedEventHandler((s, e) =>
                    {
                        if (e.Data != null)
                            errorDetail += string.Concat(e.Data);
                    });

                    process.OutputDataReceived += receivedEventHandler;
                    process.ErrorDataReceived += errorReceivedEventHandler;
                    process.Start();
                    process.BeginOutputReadLine();
                    process.BeginErrorReadLine();
                    process.WaitForExit();

                    if (process.ExitCode != 0)
                    {
                        throw new Exception($"Error while processing file: {errorDetail}");
                    }
                    return result;
                }
            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                if (process != null)
                {
                    process.OutputDataReceived -= receivedEventHandler;
                    process.ErrorDataReceived -= errorReceivedEventHandler;
                }
            }
        }

        /// <summary>
        /// Get duration of video.
        /// </summary>
        /// <param name="fileName">The path of video.</param>
        /// <returns>The duration of video.</returns>
        public static async Task<TimeSpan> GetVideoDuration(string fileName)
        {
            return await Task.Run(() =>
            {
                TimeSpan duration = TimeSpan.Zero;
                string argument = $"-loglevel error -show_entries format=duration -of csv=p=0 \"{fileName}\"";

                string result = StartProbeProcess(argument);
                double seconds;
                if (double.TryParse(result, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out seconds))
                {
                    duration = TimeSpan.FromSeconds(seconds);
                }

                return duration;
            });

        }

        /// <summary>
        /// Get width and height of video.
        /// </summary>
        /// <param name="fileName">The path of video.</param>
        /// <returns>The duration of video.</returns>
        public static async Task<Size> GetVideoSize(string fileName)
        {
            return await Task.Run(() =>
            {
                Size videoSize = new Size();
                string argument = $"-loglevel error -show_entries stream=width,height -of csv=p=0 \"{fileName}\"";

                string result = StartProbeProcess(argument);
                string[] stringSize = null;
                if (!string.IsNullOrEmpty(result) && result.Contains(","))
                {
                    stringSize = result.Split(',');
                    videoSize.Width = double.Parse(stringSize[0]);
                    videoSize.Height = double.Parse(stringSize[1]);
                }

                return videoSize;
            });

        }

        /// <summary>
        /// Indicates if file has video tracks.
        /// </summary>
        /// <param name="fileName">The path of file.</param>
        /// <returns></returns>
        public static async Task<bool> HasVideo(string fileName)
        {
            return await Task.Run(() =>
            {
                bool hasVideo = false;
                string argument = $"-loglevel error -show_entries stream=codec_type -of csv=p=0 \"{fileName}\"";

                string result = StartProbeProcess(argument);
                hasVideo = !string.IsNullOrEmpty(result) && result.Contains("video");

                return hasVideo;
            });

        }
    }
}

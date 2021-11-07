
using Khernet.UI.Cache;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using Vlc.DotNet.Core;
using Vlc.DotNet.Core.Interops;

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
        static string[] options = new[]
                   {
                        "--intf", "dummy", /* no interface                   */
                        "--vout", "dummy", /* we don't want video output     */
                        "--no-audio", /* we don't want audio decoding   */
                        "--no-video-title-show", /* nor the filename displayed     */
                        "--no-stats", /* no stats */
                        "--no-sub-autodetect-file", /* we don't want subtitles        */
                        "--no-snapshot-preview", /* no blending in dummy vout      */
                    };

        /// <summary>
        /// Converts and video file ro a specific format.
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
        /// Get duration of video.
        /// </summary>
        /// <param name="fileName">The path of video.</param>
        /// <returns>The duration of video.</returns>
        public static async Task<TimeSpan> GetVideoDuration(string fileName)
        {
            return await Task.Run(() =>
            {
                TimeSpan duration = TimeSpan.Zero;
                AutoResetEvent reset = new AutoResetEvent(false);

                using (var mediaPlayer = new Vlc.DotNet.Core.VlcMediaPlayer(Configurations.VlcDirectory, options))
                {
                    mediaPlayer.SetMedia(new Uri(fileName));

                    mediaPlayer.EncounteredError += (sender, e) =>
                    {
                        //Release the waiting process so it can continue executing in case of error
                        reset.Set();
                    };

                    mediaPlayer.Playing += (sender, e) =>
                    {
                        var media = (VlcMediaPlayer)sender;

                        //Get duration of video in milliseconds and convert it to TimeSpan object
                        duration = TimeSpan.FromMilliseconds(media.Length);

                        //Set position to final so video can stop immediately
                        media.Position = media.Length;

                        //Release the waiting process so it can continue executing
                        reset.Set();
                    };

                    //Play video
                    mediaPlayer.Play();

                    //Wait current process until duration of video is captured.
                    reset.WaitOne();

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
                AutoResetEvent reset = new AutoResetEvent(false);

                using (var mediaPlayer = new Vlc.DotNet.Core.VlcMediaPlayer(Configurations.VlcDirectory, options))
                {
                    mediaPlayer.SetMedia(new Uri(fileName));

                    mediaPlayer.EncounteredError += (sender, e) =>
                    {
                        //Release the waiting process so it can continue executing in case of error
                        reset.Set();
                    };

                    mediaPlayer.Playing += (sender, e) =>
                    {
                        var media = (VlcMediaPlayer)sender;

                        //Get video width and height
                        var mediaInfo = media.GetMedia();

                        //Search for video track to get width and height
                        for (int i = 0; i < mediaInfo.Tracks.Length; i++)
                        {
                            if (mediaInfo.Tracks[i].Type != Vlc.DotNet.Core.Interops.Signatures.MediaTrackTypes.Video)
                                continue;

                            VideoTrack track = (VideoTrack)mediaInfo.Tracks[i].TrackInfo;

                            videoSize.Width = track.Width;
                            videoSize.Height = track.Height;
                        }

                        //Set position to final so video can stop immediately
                        media.Position = media.Length;

                        //Release the waiting process so it can continue executing
                        reset.Set();
                    };

                    //Play video
                    mediaPlayer.Play();

                    //Wait current process until duration of video is captured.
                    reset.WaitOne();

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
                AutoResetEvent reset = new AutoResetEvent(false);

                using (var mediaPlayer = new Vlc.DotNet.Core.VlcMediaPlayer(Configurations.VlcDirectory, options))
                {
                    mediaPlayer.SetMedia(new Uri(fileName));

                    mediaPlayer.EncounteredError += (sender, e) =>
                    {
                        //Release the waiting process so it can continue executing in case of error
                        reset.Set();
                    };

                    mediaPlayer.Playing += (sender, e) =>
                    {
                        var media = (VlcMediaPlayer)sender;

                        //Get duration of video in milliseconds and convert it to TimeSpan object
                        hasVideo = media.Video.Tracks.Count > 0;

                        //Set position to final so video can stop immediately
                        media.Position = media.Length;

                        //Release the waiting process so it can continue executing
                        reset.Set();
                    };

                    //Play video
                    mediaPlayer.Play();

                    //Wait current process until duration of video is captured.
                    reset.WaitOne();
                }

                return hasVideo;
            });

        }
    }
}

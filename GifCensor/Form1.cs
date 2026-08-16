using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using Microsoft.Web.WebView2.Core;
using System.Collections.Generic;
using ImageMagick;
using System.IO;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Drawing.Drawing2D;
//using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Formats.Gif;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Metadata;
using SixLabors.ImageSharp.Metadata.Profiles.Exif;
using SixLabors.ImageSharp.Metadata.Profiles;
using Microsoft.WindowsAPICodePack.Shell;
using Xabe.FFmpeg;
using System.Linq;




namespace GifCensor
{
    public partial class Form1 : Form
    {
        //The currently viewed file. Gif is the actual image data
        Image gif;

        enum MediaType { Image, Gif, Video }

        class MediaItem
        {
            public string Path { get; set; }
            public MediaType Type { get; set; }
            public Image[] Frames { get; set; } // For images or GIFs
            public Image[] MaskFrames { get; set; } // For images or GIFs
            public int FrameDelay { get; set; } // Optional, for GIF
            public string extractedFramePath { get; set; }// Optional, for video
            public string processedFramePath { get; set; }// Optional, for video
        }


        string path;

        int delay; //Frame delay of the currently loaded gif / file

        //Store loaded MEDIA, and which is selected
        List<MediaItem> mediaHistory = new List<MediaItem>();
        int mediaIndex = -1;

        private Bitmap maskBitmap = null;
        private bool maskReady = false;

        private bool updatedStartEnd = false;

        private bool webView2Initialized = false;

        private bool logDetails = true;

        bool isPickingColor = false;

        public Form1()
        {
            InitializeComponent();
            InitializeAsync();
            this.AllowDrop = true;
            WireDragDrop(this.Controls);


            // Setup WebMessageReceived handler to get messages from JS
            webView21.CoreWebView2InitializationCompleted += (s, e) =>
            {
                if (e.IsSuccess)
                {
                    webView21.CoreWebView2.WebMessageReceived += CoreWebView2_WebMessageReceived;
                }
            };

        }

        private void WireDragDrop(Control.ControlCollection ctls) //Enable dragdrop on all elements
        {
            foreach (Control ctl in ctls)
            {
                ctl.AllowDrop = true;
                ctl.DragEnter += Form1_DragEnter;
                ctl.DragDrop += Form1_DragDrop;
                WireDragDrop(ctl.Controls);
            }
        }

        private async void InitializeAsync()
        {
            var options = new CoreWebView2EnvironmentOptions("--allow-file-access-from-files");
            var env = await CoreWebView2Environment.CreateAsync(null, null, options);

            await webView21.EnsureCoreWebView2Async(env);
            webView2Initialized = true;

            // Optionally load a default gif:
            //ShowGif("E:/temptest/program/test gifs/8f71b48b80771d8290589f4f57711de9.gif");

            // add overlay JS after navigation completes
            webView21.CoreWebView2.NavigationCompleted += CoreWebView2_NavigationCompleted;
        }

        //private void CoreWebView2_NavigationCompleted(object sender, CoreWebView2NavigationCompletedEventArgs e)
        //{
        //        webView21.CoreWebView2.ExecuteScriptAsync(webviewScript.Script); //Inject custom painting JS after media loaded
        //}

        private void CoreWebView2_NavigationCompleted(object sender, CoreWebView2NavigationCompletedEventArgs e)
        {
            string mediaPath;

            if (mediaHistory.Count != 0)
            {
                // Use the current media from the queue
                mediaPath = new Uri(mediaHistory[mediaIndex].Path).AbsoluteUri;
            }
            else
            {
                // Default media for testing
                var defaultGifPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "8f7.gif");
                mediaPath = new Uri(defaultGifPath).AbsoluteUri;
            }

            // Call the JS function to load the media dynamically
            string script = $"window.showMedia('{mediaPath}');";
            webView21.CoreWebView2.ExecuteScriptAsync(script);
            //Console.WriteLine("index " + mediaIndex + " / count" + mediaHistory.Count);
        }


        private void CoreWebView2_WebMessageReceived(object sender, CoreWebView2WebMessageReceivedEventArgs e) //Recieved "something" from the JS webview
        {
            //Mask message
            try
            {
                var json = e.WebMessageAsJson;
                var document = JsonDocument.Parse(json);

                if (document.RootElement.TryGetProperty("maskData", out JsonElement element))
                {
                    string base64 = element.GetString().Replace("data:image/png;base64,", "");
                    byte[] bytes = Convert.FromBase64String(base64);

                    using (MemoryStream ms = new MemoryStream(bytes))
                    {
                        Bitmap newMask = new Bitmap(ms);

                        if (newMask.Width <= 0 || newMask.Height <= 0)
                        {
                            maskReady = false;
                            MessageBox.Show("Received mask has invalid dimensions.");
                            newMask.Dispose();
                            return;
                        }

                        // Dispose old mask if any
                        maskBitmap?.Dispose();
                        maskBitmap = newMask;
                        maskReady = true;

                        lblSel.Text = $"Mask received: {maskBitmap.Width} x {maskBitmap.Height}";
                    }
                }
            }
            catch (Exception ex)
            {
                maskReady = false;
                MessageBox.Show("Mask decode error: " + ex.Message);
            }

            //Color picker message
            try
            {
                var json = e.WebMessageAsJson;
                var document = JsonDocument.Parse(json);

                if (document.RootElement.TryGetProperty("pickedColor", out JsonElement colorElement))
                {
                    int r = colorElement.GetProperty("r").GetInt32();
                    int g = colorElement.GetProperty("g").GetInt32();
                    int b = colorElement.GetProperty("b").GetInt32();

                    Color picked = Color.FromArgb(r, g, b);
                    lblSel.Text = $"Picked Color: R={r}, G={g}, B={b}";
                    colorDialog1.Color = picked; // Optional: Store it for chroma key use
                    panelChromaColor.BackColor = colorDialog1.Color;
                }
            }
            catch (Exception ex)
            {
                maskReady = false;
                MessageBox.Show("Picker error: " + ex.Message);
            }

        }

        private static bool IsMasked(Bitmap mask, int x, int y) //Helper method, is the pixel masked
        {
            if (mask == null) return false;
            if (x < 0 || y < 0 || x >= mask.Width || y >= mask.Height) return false;
            return mask.GetPixel(x, y).A > 128;
        }

        private void ShowMedia() // Display the media at the current index in the "queue"
        {
            if (!webView2Initialized || webView21.CoreWebView2 == null)
            {
                MessageBox.Show("WebView2 not initialized!");
                return;
            }

            // Path to your local HTML (index.html) containing the custom JS
            var htmlPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "index.html");
            var htmlUri = new Uri(htmlPath).AbsoluteUri;

            webView21.CoreWebView2.Navigate(htmlUri);

            UpdateLabels();
        }


        //private void ShowMedia() //Display the media at the current index in the "queue"
        //{
        //    if (!webView2Initialized || webView21.CoreWebView2 == null)
        //    {
        //        MessageBox.Show("WebView2 not initialized!");
        //        return;
        //    }

        //    if(mediaHistory.Count != 0) //If media loaded
        //    {
        //        var uri = new Uri(mediaHistory[mediaIndex].Path); //Make a URL out of the file path
        //        string Url = uri.AbsoluteUri;
        //        Console.WriteLine("showmeda url " + Url);
        //        webView21.CoreWebView2.Navigate(Url);

        //        delay = GetDelay(); //Find the delay between frames //Not necessary?? should only be needed for encoding, not display? IDK, leaving in for backup
        //    }
        //    else
        //    {
        //        webView21.CoreWebView2.Navigate("about:blank"); //Show blank screen
        //    }


        //    UpdateLabels();
        //}

        //private Media LoadMediaWithRetry(string path, int maxRetries = 5, int delayMs = 100) //Will try and get media from the path and return it
        //{
        //    for (int i = 0; i < maxRetries; i++)
        //    {
        //        try
        //        {
        //            return Image.FromFile(path);
        //        }
        //        catch (OutOfMemoryException)
        //        {
        //            // Wait a bit and retry
        //            System.Threading.Thread.Sleep(delayMs);
        //            Console.WriteLine("load out of memory");
        //        }
        //    }

        //    // Last try, let exceptions bubble if fails
        //    return Image.FromFile(path);
        //}

        private void Form1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Copy;
            else
                e.Effect = DragDropEffects.None;
        }

        private async void Form1_DragDrop(object sender, DragEventArgs e) //handle file import
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            if (files != null && files.Length > 0)
            {
                await ImportMediaFileAsync(files[0]); // reuse helper
            }

        }

        //loadmedia, mediaload, importmedia,loadasync,loadmediaasync,asyncmedia
        private async Task ImportMediaFileAsync(string file)
        {
            updatedStartEnd = false;
            if (!webView2Initialized)
            {
                MessageBox.Show("WebView2 not initialized yet.");
                return;
            }

            Console.WriteLine("Importing new media, file = " + file);

            string ext = Path.GetExtension(file).ToLower();
            MediaItem media = new MediaItem { Path = file };

            if (ext == ".gif")
            {
                media.Type = MediaType.Gif;
                media.FrameDelay = GetDelay(media);
                media.Frames = await GetFrames(media);
            }
            else if (ext == ".png" || ext == ".jpg" || ext == ".jpeg")
            {
                media.Type = MediaType.Image;
                media.Frames = new Image[] { Image.FromFile(file) };
            }
            else if (ext == ".mp4" || ext == ".avi" || ext == ".mov" || ext == ".mkv" || ext == ".webm")
            {
                media.Type = MediaType.Video;
                // Do not extract frames immediately
            }
            else
            {
                MessageBox.Show("Unsupported file type.");
                return;
            }

            mediaHistory.Add(media);
            mediaIndex = mediaHistory.Count - 1;
            ShowMedia();
            UpdateLabels();
            UpdateStartEndFrame();
            Console.WriteLine("index " + mediaIndex + " / count" + mediaHistory.Count);
            GC.Collect(); // Force garbage collector
        }

        private int GetDelay(MediaItem media) //Find the frame delay, in ms, between frames of the MEDIA
        {
            int d = 100; //default delay



            if (media.Type == MediaType.Gif)
            {
                try
                {
                    var item = Image.FromFile(media.Path).GetPropertyItem(0x5100); //Where that info is stored
                    d = (item.Value[0] + item.Value[1] * 256) * 10; //Convert
                }
                catch { }

            }
            //redundant
            //else if (media.Type == MediaType.Video)
            //{
            //    ShellFile shellFile = ShellFile.FromFilePath(media.Path);
            //    float frameRate = (float)shellFile.Properties.System.Video.FrameRate.Value / 1000;
            //    Console.WriteLine("Framerate " + frameRate);
            //}

            return d;
        }

        private void UpdateLabels()
        {
            if (mediaIndex >= 0) //todo
            {
                MediaItem media = mediaHistory[mediaIndex];
                if (media.Type == MediaType.Gif)
                {
                    Image gif = Image.FromFile(media.Path);
                    lblSize.Text = $"{gif.Width} x {gif.Height}, {gif.GetFrameCount(FrameDimension.Time)} frames";
                }
                else if (media.Type == MediaType.Image)
                {
                    Image gif = Image.FromFile(media.Path);
                    lblSize.Text = $"{gif.Width} x {gif.Height}, Single Image";
                }
                else if (media.Type == MediaType.Video)
                {
                    int width = 0, height = 0, totalFrames = 0;
                    double fps = 0;


                    ShellFile shellFile = ShellFile.FromFilePath(media.Path);
                    fps = (double)(shellFile.Properties.System.Video.FrameRate.Value / 1000);
                   

                    float dur = (float)shellFile.Properties.System.Media.Duration.Value;
                    dur = dur / 10000000;

                    width = (int)(shellFile.Properties.System.Video.FrameWidth.Value ?? 0);
                    height = (int)(shellFile.Properties.System.Video.FrameHeight.Value ?? 0);

                    totalFrames = (int)Math.Round(fps * dur);



                    lblSize.Text = $"{width} x {height}, {totalFrames} frames, {fps:F2} fps, {dur:F2} sec";
                }

            }
            else
            {
                //lblSize.Text = "No GIF loaded";
                lblSize.Text = "No media loaded";
            }
            Console.WriteLine("index " + mediaIndex + " / count" + mediaHistory.Count);
        }


        //Image[] GetFrames(Image originalGif) //Extracts the frames from a gif.
        //{
        //    FrameDimension dimension = new FrameDimension(originalGif.FrameDimensionsList[0]); //Time dimension?
        //    int numberOfFrames = originalGif.GetFrameCount(dimension);

        //    Image[] frames = new Image[numberOfFrames];

        //    for (int i = 0; i < numberOfFrames; i++)
        //    {
        //        originalGif.SelectActiveFrame(dimension, i); //Select the frame we want to extract

        //        Bitmap bmp = new Bitmap(originalGif.Width, originalGif.Height, System.Drawing.Imaging.System.Drawing.Imaging.PixelFormat.Format32bppArgb); //Set up a bitmap to store it

        //        using (Graphics g = Graphics.FromImage(bmp))
        //        {
        //            g.Clear(Color.Transparent); // clear to transparent before drawing
        //            g.DrawImage(originalGif, 0, 0, originalGif.Width, originalGif.Height); //Write gif frame to bitmap
        //        }

        //        frames[i] = bmp;
        //    }

        //    return frames;
        //}

        async Task<Image[]> GetFrames(MediaItem media)
        {
            if ((media.Type == MediaType.Gif) || (media.Type == MediaType.Image))
            {
                Image originalGif = Image.FromFile(media.Path);
                FrameDimension dimension = new FrameDimension(originalGif.FrameDimensionsList[0]);
                int numberOfFrames = originalGif.GetFrameCount(dimension);

                Image[] frames = new Image[numberOfFrames];

                for (int i = 0; i < numberOfFrames; i++)
                {
                    originalGif.SelectActiveFrame(dimension, i);

                    Bitmap bmp = new Bitmap(originalGif.Width, originalGif.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                    using (Graphics g = Graphics.FromImage(bmp))
                    {
                        g.Clear(Color.Transparent);
                        g.DrawImage(originalGif, 0, 0, originalGif.Width, originalGif.Height);
                    }

                    frames[i] = bmp;
                }

                return frames;
            }
            else if (media.Type == MediaType.Video)
            {
                // Create temporary folder to extract video frames
                string videoDir = Path.GetDirectoryName(media.Path);
                string videoName = Path.GetFileNameWithoutExtension(media.Path);
                string tempDir = Path.Combine(videoDir, videoName + "_frames_temp");

                if (!Directory.Exists(tempDir))
                    Directory.CreateDirectory(tempDir);

                media.extractedFramePath = tempDir;

                //// Extract frames using FFmpeg
                //string outputPattern = Path.Combine(tempDir, "frame_%04d.png");
                //var conversion = FFmpeg.Conversions.New()
                //    .AddParameter($"-i \"{media.Path}\" \"{outputPattern}\"", ParameterPosition.PreInput);

                //await conversion.Start();

                //fix for starting at 0 / 1

                string outputPattern = Path.Combine(tempDir, "frame_%04d.png");

                var conversion = FFmpeg.Conversions.New()
                    .AddParameter(
                        $"-start_number 0 -i \"{media.Path}\" \"{outputPattern}\"",
                        ParameterPosition.PreInput);

                await conversion.Start();

                // Load frames into memory
                //string[] frameFiles = Directory.GetFiles(tempDir, "*.png").OrderBy(f => f).ToArray();
                // Image[] frames = new Image[frameFiles.Length];

                //for (int i = 0; i < frameFiles.Length; i++)
                //{
                //frames[i] = Image.FromFile(frameFiles[i]);
                //}

                //return frames;
            }

            return null;
        }



        // Class to deserialize JSON selection rectangle from JS
        public class SelectionRect
        {
            public int x { get; set; }
            public int y { get; set; }
            public int width { get; set; }
            public int height { get; set; }
        }


        private async void btnCensor_Click(object sender, EventArgs e) //Click to start processing
        {
            btnCensor.Enabled = false;

            //Console.WriteLine($"Processing {webView21.Source}");
            //DebugPrint($"Processing {webView21.Source}");

            // Request mask from JS
            try
            {
                await webView21.CoreWebView2.ExecuteScriptAsync("window.sendMaskToHost();");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to request mask: " + ex.Message);
                Console.WriteLine("Failed to request mask: " + ex.Message);
                btnCensor.Enabled = true;
                return;
            }

            // Wait for mask to be received (poll every 100ms for max 5s)
            int retries = 50;
            while (maskBitmap == null && retries-- > 0)
            {
                await Task.Delay(100);
            }

            if (maskBitmap == null)
            {
                MessageBox.Show("Failed to receive mask from WebView.");
                btnCensor.Enabled = true;
                return;
            }

            Console.WriteLine("Extracting frames");
            DebugPrint($"Extracting frames");

            Image[] processedFrames = new Image[0]; //Hold the processed frames

            //path = $"{webView21.Source}".Replace("file:///", "");
            Console.WriteLine("getting media from index " + mediaIndex + " / count" + mediaHistory.Count);
            MediaItem media = mediaHistory[mediaIndex];
            path = media.Path;
            Console.WriteLine("media path" + media.Path);

            if (media.Type == MediaType.Gif)
            {
                //Console.WriteLine($"{gifFrames.Length} frames extracted");
                //DebugPrint($"{gifFrames.Length} frames extracted");

                processedFrames = await Task.Run(() => ProcessFrames(media.Frames)); //Async method, go and process the frames in the background

                DebugPrint($"Encoding GIF...");

                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();

                path = AppendFileNumberIfExists(path);

                await Task.Run(() => MakeGif(processedFrames, media.FrameDelay, path)); //async? Make a gif from the frames

                stopwatch.Stop();
                TimeSpan elapsed = stopwatch.Elapsed;

                Console.WriteLine($"Complete. Encoded in {elapsed.TotalSeconds} s");
                DebugPrint($"Complete. Encoded in {elapsed.TotalSeconds} s");
            }
            else if (media.Type == MediaType.Image)
            {
                //Console.WriteLine($"{gifFrames.Length} frames extracted");
                //DebugPrint($"{gifFrames.Length} frames extracted");
                //media.Frames[0].Save("E:/temptest/program/test_diff_files/test_direct_output_pre.png", ImageFormat.Png); //Test

                processedFrames = await Task.Run(() => ProcessFrames(media.Frames)); //Async method, go and process the frames in the background


                //processedFrames[0].Save("E:/temptest/program/test_diff_files/test_direct_output_post.png", ImageFormat.Png); //Test

                processedFrames[0] = FixFormatting(processedFrames[0]); //Lets test this!

                DebugPrint($"saving image");
                path = AppendFileNumberIfExists(path);
                //Console.WriteLine($"System.Drawing.Imaging.PixelFormat: {gifFrames[0].System.Drawing.Imaging.PixelFormat}");
                //gifFrames[0].Save("E:/temptest/program/test gifs/test_direct_output.png", ImageFormat.Png); //Test

                //path = $"{webView21.Source}".Replace("file:///", ""); //Get path from the webview.... yuck?

                processedFrames[0].Save(path, ImageFormat.Png);

                //Console.WriteLine($"Complete.");
                DebugPrint($"Complete.");

            }
            else if (media.Type == MediaType.Video)
            {
                DebugPrint("Preparing video processing...");

                string videoDir = Path.GetDirectoryName(media.Path);
                string baseName = GetBaseVideoName(media.Path);

                // Determine output index
                //int currentIndex = GetOutputIndex(media.Path);      // 0 for original, 1 for first output, etc. ????
                //int nextIndex = currentIndex + 1;
                int latestProcessedIndex = GetLatestProcessedFramesIndex(media.Path);
                int nextIndex = latestProcessedIndex + 1;


                // Determine raw frames folder
                string rawFramesDir = GetRawFramesFolder(media.Path);
                Console.WriteLine("raw frames dir " + rawFramesDir);
               
                if (!Directory.Exists(rawFramesDir))
                {
                    Directory.CreateDirectory(rawFramesDir);
                    DebugPrint("Extracting video frames with FFmpeg...");

                    string pattern = Path.Combine(rawFramesDir, "frame_%04d.png");
                    var conversion = FFmpeg.Conversions.New()
                        .AddParameter($"-i \"{media.Path}\" \"{pattern}\"", ParameterPosition.PreInput);

                    await conversion.Start();
                }



                // Determine input frames (either raw or previous processed generation)
                string inputFramesDir = rawFramesDir; //Set input frames directory to the unprocessed frames folder

                if (checkReuseProcessed.Checked) //Then check if we need it
                   // if (checkReuseProcessed.Checked && currentIndex > 0) //Then check if we need it
                {

                    //string prevProcessed = GetProcessedFramesFolder(media.Path, currentIndex);

                    string prevProcessed = GetLatestProcessedFramesFolder(media.Path);
                    Console.WriteLine("prevprocessed " + prevProcessed);

                    if (!string.IsNullOrEmpty(prevProcessed) && Directory.Exists(prevProcessed))
                    {
                        DebugPrint("Found previously generated frames at " + prevProcessed);
                        inputFramesDir = prevProcessed;
                    }
                }
                else //Extract frames for the first time, to path/name_frames
                {
                    
                }

                // Determine output frames folder
                string processedDir = GetProcessedFramesFolder(media.Path, nextIndex);
                if (!Directory.Exists(processedDir))
                    Directory.CreateDirectory(processedDir);

                DebugPrint($"Processing frames → {processedDir}");

                // Load frames
                string[] frameFiles = Directory.GetFiles(inputFramesDir, "*.png")
                                               .OrderBy(f => f)
                                               .ToArray();
                int totalFrames = frameFiles.Length;

                bool useFrameRange = checkFrameRange.Checked;
                int minFrame = 1, maxFrame = totalFrames;
                if (useFrameRange)
                {
                    minFrame = Math.Max(1, (int)numMinFrame.Value);
                    maxFrame = Math.Min(totalFrames, (int)numMaxFrame.Value);
                }

                // Process frames
                for (int i = 0; i < totalFrames; i++)
                {
                    string srcPath = frameFiles[i];
                    string dstPath = Path.Combine(processedDir, $"frame_{i:0000}.png");

                    bool inRange = !useFrameRange || (i + 1 >= minFrame && i + 1 <= maxFrame);

                    if (!inRange)
                    {
                        // FAST PATH: just copy the file
                        File.Copy(srcPath, dstPath, overwrite: true);
                        DebugPrint($"{i + 1}/{totalFrames} copied (outside range)");
                        continue;
                    }

                    // SLOW PATH: actually process
                    using (Bitmap frame = new Bitmap(srcPath))
                    using (Image processed = ProcessSingleFrame(frame))
                    {
                        processed.Save(dstPath, ImageFormat.Png);
                    }

                    DebugPrint($"{i + 1}/{totalFrames} processed");
                }

                if (checkEncodeVid.Checked)
                {
                    DebugPrint("Encoding video...");

                    string outputVideo = AppendFileNumberIfExists(media.Path);

                    ShellFile shellFile = ShellFile.FromFilePath(media.Path);
                    double fps = (double)(shellFile.Properties.System.Video.FrameRate.Value / 1000);

                    string ext = Path.GetExtension(outputVideo).ToLowerInvariant();

                    // Create FFmpeg conversion
                    var conversionVideo = FFmpeg.Conversions.New();

                    string framePattern = Path.Combine(processedDir, "frame_%04d.png");

                    if (ext == ".webm")
                    {
                        // WebM encoding with audio preserved
                        conversionVideo
                            .AddParameter($"-framerate {fps} -i \"{framePattern}\" -i \"{media.Path}\"") // input frames + original audio
                            .AddParameter("-c:v libvpx-vp9 -pix_fmt yuv420p -crf 12 -b:v 0 -row-mt 1 -deadline good")
                            .AddParameter("-map 0:v:0 -map 1:a? -c:a copy"); // map frames as video, original audio if present
                    }
                    else
                    {
                        // MP4 encoding with audio preserved
                        conversionVideo
                            .AddParameter($"-framerate {fps} -i \"{framePattern}\" -i \"{media.Path}\"")
                            .AddParameter("-c:v libx264 -pix_fmt yuv420p -crf 12 -preset slow -profile:v high -level 4.1")
                            .AddParameter("-map 0:v:0 -map 1:a? -c:a copy"); // video from frames, audio from original
                    }

                    conversionVideo
                        .SetOutput(outputVideo)
                        .SetOverwriteOutput(true);

                    await conversionVideo.Start();

                    path = outputVideo;
                }

            }



            //}

            btnCensor.Enabled = true;

            Console.WriteLine("filepath " + path);
            DebugPrint($"Filepath " + path);




            if (chkDispProcessed.Checked) //If we should display the new image
            {
                //Add it to the list
                await ImportMediaFileAsync(path); // outputVideo is the path to the new MP4

                //mediaIndex = mediaHistory.Count - 1;
                //ShowMedia();
            }
            Console.WriteLine("index " + mediaIndex + " / count" + mediaHistory.Count);
            GC.Collect();
            updatedStartEnd = false;
        }

        private int GetLatestProcessedFramesIndex(string videoPath)
        {
            string dir = Path.GetDirectoryName(videoPath);
            string baseName = GetBaseVideoName(videoPath);

            string prefix = $"{baseName}_frames_processed(";
            int maxIndex = 0;

            foreach (string folder in Directory.GetDirectories(dir))
            {
                string name = Path.GetFileName(folder);

                if (!name.StartsWith(prefix) || !name.EndsWith(")"))
                    continue;

                string num = name.Substring(prefix.Length, name.Length - prefix.Length - 1);

                if (int.TryParse(num, out int index))
                    maxIndex = Math.Max(maxIndex, index);
            }

            return maxIndex;
        }

        private string GetLatestProcessedFramesFolder(string videoPath)
        {
            string dir = Path.GetDirectoryName(videoPath);
            string baseName = GetBaseVideoName(videoPath);

            if (!Directory.Exists(dir))
                return null;

            // Pattern: <base>_frames_processed(n)
            string prefix = $"{baseName}_frames_processed(";

            int maxIndex = -1;
            string latestFolder = null;

            foreach (string folder in Directory.GetDirectories(dir))
            {
                string name = Path.GetFileName(folder);

                if (!name.StartsWith(prefix) || !name.EndsWith(")"))
                    continue;

                string numberPart = name.Substring(
                    prefix.Length,
                    name.Length - prefix.Length - 1
                );

                if (int.TryParse(numberPart, out int index))
                {
                    if (index > maxIndex)
                    {
                        maxIndex = index;
                        latestFolder = folder;
                    }
                }
            }

            return latestFolder;
        }

        Image[] ProcessFrames(Image[] frames)
        {
            Image[] outputFrames = new Image[frames.Length];
            List<Bitmap> frameMasks = null;

            int totalFrames = frames.Length;

            // ----------------------------
            // MASK GENERATION (READ-ONLY)
            // ----------------------------
            if (checkChroma.Checked && maskBitmap != null)
            {
                frameMasks = new List<Bitmap>(frames.Length);

                Color chromaColor = colorDialog1.Color;
                int tolerance = int.Parse(txtChromaSens.Text);

                // Clone maskBitmap safely
                Bitmap maskRef = new Bitmap(
                    maskBitmap.Width,
                    maskBitmap.Height,
                    System.Drawing.Imaging.PixelFormat.Format32bppArgb);

                using (Graphics g = Graphics.FromImage(maskRef))
                {
                    g.DrawImage(maskBitmap, 0, 0, maskBitmap.Width, maskBitmap.Height);
                }

                for (int i = 0; i < frames.Length; i++)
                {
                    Bitmap srcBmp = (Bitmap)frames[i]; // READ ONLY
                    Bitmap mask = new Bitmap(srcBmp.Width, srcBmp.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

                    BitmapData bmpData = srcBmp.LockBits(
                        new Rectangle(0, 0, srcBmp.Width, srcBmp.Height),
                        ImageLockMode.ReadOnly,
                        System.Drawing.Imaging.PixelFormat.Format32bppArgb);

                    BitmapData maskData = mask.LockBits(
                        new Rectangle(0, 0, mask.Width, mask.Height),
                        ImageLockMode.WriteOnly,
                        System.Drawing.Imaging.PixelFormat.Format32bppArgb);

                    BitmapData refData = maskRef.LockBits(
                        new Rectangle(0, 0, maskRef.Width, maskRef.Height),
                        ImageLockMode.ReadOnly,
                        System.Drawing.Imaging.PixelFormat.Format32bppArgb);

                    unsafe
                    {
                        byte* bmpPtr = (byte*)bmpData.Scan0;
                        byte* maskPtr = (byte*)maskData.Scan0;
                        byte* refPtr = (byte*)refData.Scan0;

                        int bytesPerPixel = 4;
                        int stride = bmpData.Stride;

                        for (int y = 0; y < srcBmp.Height; y++)
                        {
                            for (int x = 0; x < srcBmp.Width; x++)
                            {
                                byte* srcPx = bmpPtr + y * stride + x * bytesPerPixel;
                                byte r = srcPx[2], g = srcPx[1], b = srcPx[0];

                                byte* refPx = refPtr + y * stride + x * bytesPerPixel;
                                bool inOriginalMask = refPx[3] >= 128;

                                byte* dstPx = maskPtr + y * stride + x * bytesPerPixel;

                                if (!inOriginalMask)
                                {
                                    dstPx[0] = dstPx[1] = dstPx[2] = dstPx[3] = 0;
                                    continue;
                                }

                                bool similar =
                                    Math.Abs(r - chromaColor.R) <= tolerance &&
                                    Math.Abs(g - chromaColor.G) <= tolerance &&
                                    Math.Abs(b - chromaColor.B) <= tolerance;

                                if (similar)
                                {
                                    dstPx[0] = 255;
                                    dstPx[1] = 0;
                                    dstPx[2] = 0;
                                    dstPx[3] = 255;
                                }
                                else
                                {
                                    dstPx[0] = dstPx[1] = dstPx[2] = dstPx[3] = 0;
                                }
                            }
                        }
                    }

                    srcBmp.UnlockBits(bmpData);
                    mask.UnlockBits(maskData);
                    maskRef.UnlockBits(refData);

                    frameMasks.Add(mask);
                }

                maskRef.Dispose();
            }
            else if (checkTracking.Checked && maskBitmap != null)
            {
                frameMasks = new List<Bitmap>(totalFrames);

                Rectangle trackedRegion = GetBoundingBoxOfMask(maskBitmap);
                Console.WriteLine($"Initial tracked region: {trackedRegion}");

                Bitmap prevFrame = (Bitmap)frames[0];
                Bitmap prevMask = new Bitmap(maskBitmap);

                frameMasks.Add(new Bitmap(prevMask)); // frame 0

                for (int i = 1; i < totalFrames; i++)
                {
                    Bitmap currFrame = (Bitmap)frames[i];

                    // Fast offset search around previous region
                    Point offset = FindBestMatchOffset(prevMask, currFrame, trackedRegion, searchRadius: 10, sampleStep: 2);

                    Console.WriteLine($"Frame {i}: offset = {offset.X},{offset.Y}");

                    Bitmap shiftedMask = ShiftMask(prevMask, offset, currFrame.Width, currFrame.Height);
                    frameMasks.Add(shiftedMask);

                    prevMask = shiftedMask;
                    prevFrame = currFrame;
                    trackedRegion = new Rectangle(trackedRegion.X + offset.X, trackedRegion.Y + offset.Y,
                                                  trackedRegion.Width, trackedRegion.Height);
                }
            }




            // ----------------------------
            // FRAME RANGE HANDLING
            // ----------------------------
            int minFrame = 0, maxFrame = frames.Length - 1;
            bool useFrameRange = checkFrameRange.Checked;
            if (useFrameRange)
            {
                minFrame = (int)numMinFrame.Value;
                maxFrame = (int)numMaxFrame.Value;

                // Shift to 0-based index, clamp to valid frame indices
                minFrame = Math.Max(0, minFrame - 1);
                maxFrame = Math.Min(frames.Length - 1, maxFrame - 1);
            }

            // ----------------------------
            // FRAME PROCESSING
            // ----------------------------
            for (int i = 0; i < frames.Length; i++)
            {
                // Skip frames outside range
                if (useFrameRange && (i < minFrame || i > maxFrame))
                {
                    outputFrames[i] = new Bitmap((Bitmap)frames[i]);
                    continue;
                }

                Bitmap src = new Bitmap((Bitmap)frames[i]);
                Bitmap mask = frameMasks != null ? frameMasks[i] : maskBitmap;

                if (radioPixel.Checked)
                    outputFrames[i] = Effects.Pixelate(src, mask, int.Parse(txtPxlSize.Text), float.Parse(txtAlpha.Text) / 100f);
                else if (radioBlur.Checked)
                    outputFrames[i] = Effects.Blur(src, mask, int.Parse(txtBlurRad.Text));
                else if (radioSolid.Checked)
                    outputFrames[i] = Effects.Fill(src, mask, colorDialog2.Color, float.Parse(txtAlpha.Text) / 100f);
                else if (radioStaticColor.Checked)
                    outputFrames[i] = Effects.FillWithStatic(src, mask, float.Parse(txtAlpha.Text) / 100f);
                else if (radioStaticMono.Checked)
                    outputFrames[i] = Effects.FillWithGrayscaleNoise(src, mask, float.Parse(txtAlpha.Text) / 100f);
                else if (radioRGBS.Checked)
                    outputFrames[i] = Effects.ApplyRGBShift(src, mask, int.Parse(txtRGBs.Text), int.Parse(txtRGBs.Text));
                else if (radioJitter.Checked)
                    outputFrames[i] = Effects.ApplyLineJitter(src, mask, int.Parse(txtJitter.Text), 1);
                else if (radioHSV.Checked)
                    outputFrames[i] = Effects.ApplyHslAdjust(src, mask, int.Parse(txtHue.Text), int.Parse(txtSat.Text), int.Parse(txtLum.Text));
                else
                    outputFrames[i] = src; // fallback
            }

            GC.Collect();
            return outputFrames;
        }


        Image ProcessSingleFrame(Image frame)
        {
            if (frame == null) throw new ArgumentNullException(nameof(frame));
            if (maskBitmap == null) throw new InvalidOperationException("maskBitmap must be set before processing.");

            Bitmap src = new Bitmap((Bitmap)frame);
            Bitmap mask;

            // ----------------------------
            // Generate mask for this frame
            // ----------------------------
            if (checkChroma.Checked)
            {
                mask = new Bitmap(src.Width, src.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

                Color chromaColor = colorDialog1.Color;
                int tolerance = int.Parse(txtChromaSens.Text);

                int minR = Math.Max(0, chromaColor.R - tolerance);
                int maxR = Math.Min(255, chromaColor.R + tolerance);
                int minG = Math.Max(0, chromaColor.G - tolerance);
                int maxG = Math.Min(255, chromaColor.G + tolerance);
                int minB = Math.Max(0, chromaColor.B - tolerance);
                int maxB = Math.Min(255, chromaColor.B + tolerance);

                Bitmap maskRef = new Bitmap(maskBitmap.Width, maskBitmap.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                using (Graphics g = Graphics.FromImage(maskRef))
                    g.DrawImage(maskBitmap, 0, 0, maskRef.Width, maskRef.Height);

                BitmapData srcData = src.LockBits(new Rectangle(0, 0, src.Width, src.Height),
                                                 ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                BitmapData maskData = mask.LockBits(new Rectangle(0, 0, mask.Width, mask.Height),
                                                   ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                BitmapData refData = maskRef.LockBits(new Rectangle(0, 0, maskRef.Width, maskRef.Height),
                                                     ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

                int bytesPerPixel = 4;
                int stride = srcData.Stride;

                unsafe
                {
                    byte* srcPtr = (byte*)srcData.Scan0;
                    byte* maskPtr = (byte*)maskData.Scan0;
                    byte* refPtr = (byte*)refData.Scan0;

                    // Sequential loop (safe)
                    for (int y = 0; y < src.Height; y++)
                    {
                        byte* rowSrc = srcPtr + y * stride;
                        byte* rowMask = maskPtr + y * stride;
                        byte* rowRef = refPtr + y * stride;

                        for (int x = 0; x < src.Width; x++)
                        {
                            byte* srcPx = rowSrc + x * bytesPerPixel;
                            byte* refPx = rowRef + x * bytesPerPixel;
                            byte* dstPx = rowMask + x * bytesPerPixel;

                            bool inOriginalMask = refPx[3] >= 128;

                            if (!inOriginalMask)
                            {
                                dstPx[0] = dstPx[1] = dstPx[2] = dstPx[3] = 0;
                                continue;
                            }

                            byte r = srcPx[2], g = srcPx[1], b = srcPx[0];

                            bool similar = (r >= minR && r <= maxR) &&
                                           (g >= minG && g <= maxG) &&
                                           (b >= minB && b <= maxB);

                            if (similar)
                            {
                                dstPx[0] = 255;  // B
                                dstPx[1] = 0;    // G
                                dstPx[2] = 0;    // R
                                dstPx[3] = 255;  // A
                            }
                            else
                            {
                                dstPx[0] = dstPx[1] = dstPx[2] = dstPx[3] = 0;
                            }
                        }
                    }
                }

                src.UnlockBits(srcData);
                mask.UnlockBits(maskData);
                maskRef.UnlockBits(refData);
                maskRef.Dispose();
            }
            else
            {
                // Normal mask usage
                mask = new Bitmap(maskBitmap.Width != src.Width || maskBitmap.Height != src.Height
                                  ? new Bitmap(maskBitmap, src.Width, src.Height)
                                  : maskBitmap);
            }

            // ----------------------------
            // Apply effect
            // ----------------------------
            Bitmap output;

            if (radioPixel.Checked)
                output = Effects.Pixelate(src, mask, int.Parse(txtPxlSize.Text), float.Parse(txtAlpha.Text) / 100f);
            else if (radioBlur.Checked)
                output = Effects.Blur(src, mask, int.Parse(txtBlurRad.Text));
            else if (radioSolid.Checked)
                output = Effects.Fill(src, mask, colorDialog2.Color, float.Parse(txtAlpha.Text) / 100f);
            else if (radioStaticColor.Checked)
                output = Effects.FillWithStatic(src, mask, float.Parse(txtAlpha.Text) / 100f);
            else if (radioStaticMono.Checked)
                output = Effects.FillWithGrayscaleNoise(src, mask, float.Parse(txtAlpha.Text) / 100f);
            else if (radioRGBS.Checked)
                output = Effects.ApplyRGBShift(src, mask, int.Parse(txtRGBs.Text), int.Parse(txtRGBs.Text));
            else if (radioJitter.Checked)
                output = Effects.ApplyLineJitter(src, mask, int.Parse(txtJitter.Text), 1);
            else if (radioHSV.Checked)
                output = Effects.ApplyHslAdjust(src, mask, int.Parse(txtHue.Text), int.Parse(txtSat.Text), int.Parse(txtLum.Text));
            else
                output = src; // fallback

            // Cleanup
            src.Dispose();
            mask.Dispose();

            return output;
        }







        private Image FixFormatting(Image inputImage) //Convert from other color profiles to a compatible one
        {
            var sourceMs = new MemoryStream();
            inputImage.Save(sourceMs, ImageFormat.Png);
            sourceMs.Position = 0;

            var magickImage = new MagickImage(sourceMs);


            //magickImage.TransformColorSpace(ColorProfile.AdobeRGB1998, ColorProfile.SRGB);
            //This was needed due to some wierd handling between adobe rgb, which made the colors look darker when converted.

            var prof = magickImage.GetColorProfile();

            if (prof != null) //Check if the image has a specific color profile or not
            {
                magickImage.TransformColorSpace(magickImage.GetColorProfile(), ColorProfile.SRGB); //Transform from the current color profile to SRGB. This should fix any wierdness with older RGB formats (see above)
            }


            magickImage.Strip();


            var outputMs = new MemoryStream();
            magickImage.Write(outputMs, MagickFormat.Bmp); // BMP for broad compatibility
            outputMs.Position = 0;

            return Image.FromStream(outputMs);
        }




        private void DebugPrint(string message)
        {
            if (!logDetails) return;

            if (InvokeRequired)
            {
                Invoke(new Action(() => DebugPrint(message)));
                return;
            }

            richTextBox1.AppendText(Environment.NewLine + message);
            richTextBox1.SelectionStart = richTextBox1.Text.Length;
            richTextBox1.ScrollToCaret();
        }


        async Task<string> SaveFramesAsVideoAsync(MediaItem media, Image[] processedFrames, string outputPath)
        {
            // 1. Create a temporary folder to store frame images
            string tempDir = Path.Combine(Path.GetTempPath(), "video_frames_" + Path.GetRandomFileName());
            Directory.CreateDirectory(tempDir);

            // 2. Save frames as PNG
            for (int i = 0; i < processedFrames.Length; i++)
            {
                string framePath = Path.Combine(tempDir, $"frame_{i:0000}.png");
                processedFrames[i].Save(framePath, ImageFormat.Png);
            }

            // 3. Determine framerate
            double fps = 30; // default

            if (media.Type == MediaType.Video)
            {
                var info = await FFmpeg.GetMediaInfo(media.Path);
                fps = info.VideoStreams.FirstOrDefault()?.Framerate ?? 30;
                Console.WriteLine("fps " + info.VideoStreams.FirstOrDefault()?.Framerate);
            }


            // 4. Build FFmpeg conversion command
            var conversion = FFmpeg.Conversions.New()
     .AddParameter($"-framerate {fps} -i \"{Path.Combine(tempDir, "frame_%04d.png")}\"")
     .AddParameter("-c:v libx264")        // use H.264 codec
     .AddParameter("-pix_fmt yuv420p")    // required by Windows Photos
     .AddParameter("-crf 18")             // optional: high quality
     .AddParameter("-preset veryfast")    // optional: encoding speed
     .SetOutput(outputPath)
     .SetOverwriteOutput(true);

            // 5. Run FFmpeg
            await conversion.Start();

            // 6. Cleanup temporary frames
            Directory.Delete(tempDir, true);

            return outputPath;
        }


        private void MakeGif(Image[] images, int delay, string filepath)
        {
            using (MagickImageCollection collection = new MagickImageCollection())
            {
                var magickFrames = new MagickImage[images.Length];

                Parallel.For(0, images.Length, i =>
                {
                    using (Bitmap bmp = new Bitmap(images[i]))
                    {
                        byte[] bitmapBytes;
                        using (var ms = new MemoryStream())
                        {
                            bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                            bitmapBytes = ms.ToArray();
                        }
                        magickFrames[i] = new MagickImage(bitmapBytes);
                        magickFrames[i].AnimationDelay = (uint)(delay / 10);
                        magickFrames[i].GifDisposeMethod = GifDisposeMethod.Previous;
                    }
                });

                foreach (var frame in magickFrames)
                {
                    collection.Add(frame);
                }

                collection.Write(filepath);
                Console.WriteLine("saving to " + filepath);
            }
        }



        private void btnPrev_Click(object sender, EventArgs e)
        {
            if (mediaIndex > 0)
            {
                mediaIndex--;
                ShowMedia();
            }
            else
            {
                Console.WriteLine("Start of queue reached");
                DebugPrint("Start of queue reached");
            }


            //Console.WriteLine("index " + mediaIndex + " / count" + mediaHistory.Count);
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            if (mediaIndex < mediaHistory.Count - 1)
            {
                mediaIndex++;
                ShowMedia();
            }
            else
            {
                Console.WriteLine("End of queue reached");
                DebugPrint("End of queue reached");
            }
            //Console.WriteLine("index " + mediaIndex + " / count" + mediaHistory.Count);
        }


        private string AppendFileNumberIfExists(string file)
        {
            if (!File.Exists(file))
                return file;

            string folderPath = Path.GetDirectoryName(file);
            string fileName = Path.GetFileNameWithoutExtension(file);
            string extension = Path.GetExtension(file);

            int fileNumber = 0;

            // Regex to detect "_output" optionally followed by "(n)" at the end
            Regex r = new Regex(@"(_output)(?:\((\d+)\))?$");
            Match m = r.Match(fileName);

            if (m.Success)
            {
                // _output already present
                if (m.Groups[2].Success) // number exists
                    fileNumber = int.Parse(m.Groups[2].Value);

                fileName = fileName.Substring(0, m.Index) + "_output"; // base filename with single "_output"
            }
            else
            {
                // _output not present yet
                fileName += "_output";
            }

            string newFile;
            do
            {
                fileNumber++;
                newFile = Path.Combine(folderPath, $"{fileName}({fileNumber}){extension}");
            } while (File.Exists(newFile));

            return newFile;
        }


        /// <summary>
        /// A function to add an incremented number at the end of a file name if a file already exists. 
        /// </summary>
        /// <param name="file">The file. This should be the complete path.</param>
        /// <param name="ext">This can be empty.</param>
        /// <returns>An incremented file name. </returns>
        //private string AppendFileNumberIfExists(string file, string ext)
        //{
        //    // This had a VB tidbit that helped to get this started. 
        //    // http://www.codeproject.com/Questions/212217/increment-filename-if-file-exists-using-csharp

        //    // If the file exists, then do stuff. Otherwise, we just return the original file name.
        //    if (File.Exists(file))
        //    {
        //        string folderPath = Path.GetDirectoryName(file); // The path to the file. No sense in dealing with this unecessarily. 
        //        string fileName = Path.GetFileNameWithoutExtension(file); // The file name with no extension. 
        //        string extension = string.Empty; // The file extension. 
        //                                         // This lets us pass in an empty string for the file extension if required. i.e. It just makes this function a bit more versatile. 
        //        if (ext == string.Empty)
        //        {
        //            extension = Path.GetExtension(file);
        //        }
        //        else
        //        {
        //            extension = ext;
        //        }

        //        // at this point, find out if the fileName ends in a number, then get that number.
        //        int fileNumber = 0; // This stores the number as a number for us. 
        //                            // need a regex here - \(([0-9]+)\)$
        //        Regex r = new Regex(@"\(([0-9]+)\)$"); // This matches the pattern we are using, i.e. ~(#).ext
        //        Match m = r.Match(fileName); // We pass in the file name with no extension.
        //        string addSpace = "_"; // We'll add a space when we don't have our pattern in order to pad the pattern.
        //        if (m.Success)
        //        {
        //            addSpace = string.Empty; // We have the pattern, so we don't add a space - it has already been added. 
        //            string s = m.Groups[1].Captures[0].Value; // This is the single capture that we are looking for. Stored as a string.
        //                                                      // set fileNumber to the new number.
        //            fileNumber = int.Parse(s); // Convert the number to an int.
        //                                       // remove the numbering from the string as we're constructing it again below.
        //            fileName = fileName.Replace("(" + s + ")", "");
        //        }

        //        // Start looping. 
        //        do
        //        {
        //            fileNumber += 1; // Increment the file number that we have above. 
        //            file = Path.Combine(folderPath, // Combine it all.
        //                                    String.Format("{0}{3}({1}){2}", // The pattern to combine.
        //                                                              fileName,         // The file name with no extension. 
        //                                                              fileNumber,       // The file number.
        //                                                              extension,        // The file extension.
        //                                                              addSpace));       // A space if needed to pad the initial ~(#).ext pattern.
        //        }
        //        while (File.Exists(file)); // As long as the file name exists, keep looping. 
        //    }
        //    return file;
        //}



        private void btnChromaCol_Click(object sender, EventArgs e)
        {
            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                Console.WriteLine("Color " + colorDialog1.Color.ToString());
            }
        }

        private async void btnPickColor_Click(object sender, EventArgs e)
        {
            await webView21.ExecuteScriptAsync("window.pickPixelColor()");
        }

        private void btnSolidColor_Click(object sender, EventArgs e)
        {
            if (colorDialog2.ShowDialog() == DialogResult.OK)
            {
                Console.WriteLine("Color " + colorDialog2.Color.ToString());
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            mediaIndex = -1;
            mediaHistory = new List<MediaItem>();

            ShowMedia(); //TEST
            Console.WriteLine("index " + mediaIndex + " / count" + mediaHistory.Count);
            GC.Collect();
        }

        private void checkChroma_CheckedChanged(object sender, EventArgs e)
        {
            if (checkChroma.Checked)
                checkTracking.Checked = false;
        }

        private void checkTracking_CheckedChanged(object sender, EventArgs e)
        {
            if (checkTracking.Checked)
                checkChroma.Checked = false;
        }


        Rectangle GetBoundingBoxOfMask(Bitmap mask)
        {
            int minX = mask.Width, minY = mask.Height, maxX = 0, maxY = 0;
            for (int y = 0; y < mask.Height; y++)
            {
                for (int x = 0; x < mask.Width; x++)
                {
                    if (mask.GetPixel(x, y).A >= 128)
                    {
                        minX = Math.Min(minX, x);
                        minY = Math.Min(minY, y);
                        maxX = Math.Max(maxX, x);
                        maxY = Math.Max(maxY, y);
                    }
                }
            }
            if (minX > maxX || minY > maxY) return Rectangle.Empty;
            return Rectangle.FromLTRB(minX, minY, maxX + 1, maxY + 1);
        }

        /// <summary>
        /// Finds the best translation offset of the previous mask to match the current frame within a search area.
        /// </summary>
        /// <param name="prevMask">The mask from the previous frame.</param>
        /// <param name="currFrame">The current frame to match against.</param>
        /// <param name="trackedRegion">The region of interest in the previous frame.</param>
        /// <param name="searchArea">The area in the current frame to search for the mask.</param>
        /// <returns>The offset (dx, dy) to apply to the previous mask.</returns>
        Point FindBestMatchOffset(Bitmap prevMask, Bitmap currFrame, Rectangle trackedRegion, int searchRadius = 10, int sampleStep = 2)
        {
            int bestOffsetX = 0;
            int bestOffsetY = 0;
            double bestScore = double.MaxValue;

            int maskWidth = prevMask.Width;
            int maskHeight = prevMask.Height;

            BitmapData prevData = prevMask.LockBits(new Rectangle(0, 0, maskWidth, maskHeight),
                ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            BitmapData currData = currFrame.LockBits(new Rectangle(0, 0, currFrame.Width, currFrame.Height),
                ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            unsafe
            {
                byte* prevPtr = (byte*)prevData.Scan0;
                int prevStride = prevData.Stride;

                byte* currPtr = (byte*)currData.Scan0;
                int currStride = currData.Stride;

                int bytesPerPixel = 4;

                // Only search small region around previous mask
                for (int dy = -searchRadius; dy <= searchRadius; dy++)
                {
                    for (int dx = -searchRadius; dx <= searchRadius; dx++)
                    {
                        double score = 0;

                        for (int y = 0; y < trackedRegion.Height; y += sampleStep)
                        {
                            for (int x = 0; x < trackedRegion.Width; x += sampleStep)
                            {
                                int px = trackedRegion.X + x;
                                int py = trackedRegion.Y + y;

                                byte* prevPx = prevPtr + py * prevStride + px * bytesPerPixel;
                                if (prevPx[3] < 128) continue; // skip transparent pixels

                                int cx = px + dx;
                                int cy = py + dy;

                                if (cx < 0 || cx >= currFrame.Width || cy < 0 || cy >= currFrame.Height) continue;

                                byte* currPx = currPtr + cy * currStride + cx * bytesPerPixel;

                                int dr = prevPx[2] - currPx[2];
                                int dg = prevPx[1] - currPx[1];
                                int db = prevPx[0] - currPx[0];

                                score += Math.Abs(dr) + Math.Abs(dg) + Math.Abs(db);
                            }
                        }

                        if (score < bestScore)
                        {
                            bestScore = score;
                            bestOffsetX = dx;
                            bestOffsetY = dy;

                            if (score == 0) break; // perfect match
                        }
                    }
                }
            }

            prevMask.UnlockBits(prevData);
            currFrame.UnlockBits(currData);

            return new Point(bestOffsetX, bestOffsetY);
        }


        Bitmap ShiftMask(Bitmap mask, Point offset, int width, int height)
        {
            Bitmap shifted = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            using (Graphics g = Graphics.FromImage(shifted))
            {
                g.Clear(Color.Transparent);
                g.DrawImage(mask, offset.X, offset.Y);
            }
            return shifted;
        }

        private void Form1_ResizeEnd(object sender, EventArgs e)
        {
            //ShowGif();
            //This seemed like a good idea, but ends up clearing the mask every time you resize the window...
        }


        //private Color HslToColor(float h, float s, float l)
        //{
        //    float c = (1f - Math.Abs(2f * l - 1f)) * s;
        //    float x = c * (1f - Math.Abs((h / 60f) % 2 - 1f));
        //    float m = l - c / 2f;

        //    float r = 0, g = 0, b = 0;

        //    if (h < 60) { r = c; g = x; }
        //    else if (h < 120) { r = x; g = c; }
        //    else if (h < 180) { g = c; b = x; }
        //    else if (h < 240) { g = x; b = c; }
        //    else if (h < 300) { r = x; b = c; }
        //    else { r = c; b = x; }

        //    return Color.FromArgb(
        //        (int)((r + m) * 255),
        //        (int)((g + m) * 255),
        //        (int)((b + m) * 255)
        //    );
        //}

        private static Color HslToColor(float h, float s, float l)
        {
            float c = (1f - Math.Abs(2f * l - 1f)) * s;
            float x = c * (1f - Math.Abs((h / 60f) % 2f - 1f));
            float m = l - c / 2f;

            float r = 0, g = 0, b = 0;

            if (h < 60) { r = c; g = x; }
            else if (h < 120) { r = x; g = c; }
            else if (h < 180) { g = c; b = x; }
            else if (h < 240) { g = x; b = c; }
            else if (h < 300) { r = x; b = c; }
            else { r = c; b = x; }

            return Color.FromArgb(
                255,
                (int)((r + m) * 255),
                (int)((g + m) * 255),
                (int)((b + m) * 255)
            );
        }

        private static void ApplyHslAdjustments(
    ref float h, ref float s, ref float l,
    int hueUI, int satUI, int lumUI)
        {
            // Hue
            h += (hueUI - 100) * 1.8f;
            if (h < 0) h += 360f;
            if (h >= 360) h -= 360f;

            // Saturation
            s *= satUI / 100f;

            // Luminance
            l *= lumUI / 100f;

            s = Clamp(s, 0f, 1f);
            l = Clamp(l, 0f, 1f);
        }

        private static void RgbToHsl(Color c, out float h, out float s, out float l)
        {
            float r = c.R / 255f;
            float g = c.G / 255f;
            float b = c.B / 255f;

            float max = Math.Max(r, Math.Max(g, b));
            float min = Math.Min(r, Math.Min(g, b));
            float delta = max - min;

            l = (max + min) * 0.5f;

            if (delta == 0)
            {
                h = 0;
                s = 0;
                return;
            }

            s = delta / (1f - Math.Abs(2f * l - 1f));

            if (max == r)
                h = 60f * (((g - b) / delta) % 6f);
            else if (max == g)
                h = 60f * (((b - r) / delta) + 2f);
            else
                h = 60f * (((r - g) / delta) + 4f);

            if (h < 0) h += 360f;
        }

        private void txtHue_TextChanged(object sender, EventArgs e)
        {
            UpdateChromaHslPreview();

            //if (txtHue.Text != "")
            //{
            //    Console.WriteLine(int.Parse(txtHue.Text));
            //    float hue = (int.Parse(txtHue.Text) - 100) * 1.8f;
            //    if (hue < 0) hue += 360f;

            //    Color preview = HslToColor(hue, 1f, 0.5f);
            //    txtHue.BackColor = preview;
            //}

        }

        private void UpdateChromaHslPreview()
        {
            if ((txtHue.Text != "") & (txtSat.Text != "") & (txtLum.Text != ""))
            {
                Console.WriteLine(int.Parse(txtHue.Text));
                //float hue = (int.Parse(txtHue.Text) - 100) * 1.8f;
                //if (hue < 0) hue += 360f;

                //Color preview = HslToColor(hue, 1f, 0.5f);
                //txtHue.BackColor = preview;

                Color baseColor = panelChromaColor.BackColor;

                RgbToHsl(baseColor, out float h, out float s, out float l);

                ApplyHslAdjustments(ref h, ref s, ref l, int.Parse(txtHue.Text), int.Parse(txtSat.Text), int.Parse(txtLum.Text));

                txtHue.BackColor = HslToColor(h, s, l);
            }

        }

        private void panelChromaColor_Paint(object sender, PaintEventArgs e)
        {
            UpdateChromaHslPreview();
        }

        private static float Clamp(float value, float min, float max)
        {
            if (value < min) return min;
            if (value > max) return max;
            return value;
        }

        private void txtSat_TextChanged(object sender, EventArgs e)
        {
            UpdateChromaHslPreview();
        }

        private void txtLum_TextChanged(object sender, EventArgs e)
        {
            UpdateChromaHslPreview();
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            txtHue.Text = trackBarHue.Value.ToString();

        }

        private void trackBarSat_Scroll(object sender, EventArgs e)
        {
            txtSat.Text = trackBarSat.Value.ToString();
        }

        private void trackBarLum_Scroll(object sender, EventArgs e)
        {
            txtLum.Text = trackBarLum.Value.ToString();
        }



        private async Task SendBitmapToJsAsync(Bitmap bmp)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                bmp.Save(ms, ImageFormat.Png);
                string base64 = Convert.ToBase64String(ms.ToArray());

                string script = "window.receiveBitmapFromHost('" + base64 + "');";
                await webView21.CoreWebView2.ExecuteScriptAsync(script);
            }
        }

        private async void btnmaskloadtest_Click(object sender, EventArgs e)
        {
            //if (maskBitmap != null)
            //{
            //    await SendBitmapToJsAsync(maskBitmap);
            //}
            LoadCachedMask();
        }

        private async void LoadCachedMask()
        {
            if (maskBitmap != null)
            {
                await SendBitmapToJsAsync(maskBitmap);
            }

        }

        private async void btnInv_Click(object sender, EventArgs e)
        {
            await webView21.CoreWebView2.ExecuteScriptAsync("window.sendMaskToHost();");

            await SendBitmapToJsAsync(InvertMask(maskBitmap));
        }



        public Bitmap InvertMask(Bitmap source)
        {
            Color maskColor = new Color();
            maskColor = Color.Red;
            // Ensure writable 32bpp ARGB
            Bitmap bmp = new Bitmap(source.Width, source.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceCopy;
                g.DrawImage(source, 0, 0, source.Width, source.Height);
            }

            Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
            BitmapData bmpData = bmp.LockBits(rect, ImageLockMode.ReadWrite, bmp.PixelFormat);

            int bytesPerPixel = 4; // 32bppArgb
            int byteCount = bmpData.Stride * bmp.Height;
            byte[] pixels = new byte[byteCount];
            System.Runtime.InteropServices.Marshal.Copy(bmpData.Scan0, pixels, 0, byteCount);

            for (int y = 0; y < bmp.Height; y++)
            {
                int rowStart = y * bmpData.Stride;
                for (int x = 0; x < bmp.Width; x++)
                {
                    int idx = rowStart + x * bytesPerPixel;

                    // Invert alpha only
                    pixels[idx + 3] = (byte)(255 - pixels[idx + 3]);

                    // If pixel is now visible (alpha > 0), set mask color
                    if (pixels[idx + 3] > 0)
                    {
                        pixels[idx + 2] = maskColor.R; // Red
                        pixels[idx + 1] = maskColor.G; // Green
                        pixels[idx + 0] = maskColor.B; // Blue
                    }
                }
            }

            System.Runtime.InteropServices.Marshal.Copy(pixels, 0, bmpData.Scan0, byteCount);
            bmp.UnlockBits(bmpData);

            return bmp;
        }

        private async void btnClearMask_Click(object sender, EventArgs e)
        {
            await webView21.CoreWebView2.ExecuteScriptAsync("window.sendMaskToHost();"); //get current mask

            Bitmap emptyBitmap = new Bitmap(maskBitmap.Width, maskBitmap.Height, maskBitmap.PixelFormat);

            await SendBitmapToJsAsync(emptyBitmap);
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void btnFirst_Click(object sender, EventArgs e)
        {
            mediaIndex = 0;
            ShowMedia();
        }

        private void btnLast_Click(object sender, EventArgs e)
        {
            mediaIndex = mediaHistory.Count - 1;
            ShowMedia();
        }

        private void checkFrameRange_CheckedChanged(object sender, EventArgs e)
        {
            if (checkFrameRange.Checked)
            {
                //txtMinFrame.Enabled = true;
                //txtMaxFrame.Enabled = true;
            }
            else
            {
                //txtMinFrame.Enabled = false;
                //txtMaxFrame.Enabled = false;
            }
            
        }

        //private async void btnShowRange_Click(object sender, EventArgs e)
        //{

        // }

        //private async void btnShowRange_Click(object sender, EventArgs e)
        //{
        //    pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
        //    pictureBox2.SizeMode = PictureBoxSizeMode.Zoom;
        //    if (mediaHistory.Count == 0)
        //        return;

        //    string videoPath = mediaHistory[mediaIndex].Path;
        //    if (!File.Exists(videoPath))
        //    {
        //        MessageBox.Show("Video not found.");
        //        return;
        //    }

        //    //if (!int.TryParse(txtMinFrame.Text, out int minFrame) ||
        //    //    !int.TryParse(txtMaxFrame.Text, out int maxFrame))
        //    //{
        //    //    MessageBox.Show("Invalid frame numbers.");
        //    //    return;
        //    //}
        //    int minFrame = (int)numMinFrame.Value;
        //    int maxFrame = (int)numMaxFrame.Value;



        //    minFrame = Math.Max(1, minFrame);
        //    maxFrame = Math.Max(1, maxFrame);

        //    Console.WriteLine($"min {minFrame} max {maxFrame}");

        //    int ffmpegMin = minFrame - 1; // FFmpeg counts frames from 0
        //    int ffmpegMax = maxFrame - 1;

        //    async Task<Image> ExtractFrameAsync(string video, int frameNumber)
        //    {
        //        var ms = new MemoryStream();

        //        var startInfo = new ProcessStartInfo
        //        {
        //            FileName = "ffmpeg", // ensure ffmpeg.exe is in PATH
        //            Arguments = $"-i \"{video}\" -vf \"select=eq(n\\,{frameNumber})\" -vframes 1 -f image2pipe -vcodec png -",
        //            RedirectStandardOutput = true,
        //            UseShellExecute = false,
        //            CreateNoWindow = true
        //        };

        //        var proc = new Process { StartInfo = startInfo };
        //        proc.Start();

        //        await proc.StandardOutput.BaseStream.CopyToAsync(ms);
        //        proc.WaitForExit();

        //        ms.Position = 0;
        //        return Image.FromStream(ms);
        //    }

        //    try
        //    {
        //        // Dispose previous images
        //        pictureBox1.Image?.Dispose();
        //        pictureBox2.Image?.Dispose();

        //        // Extract both frames concurrently
        //        var taskLower = ExtractFrameAsync(videoPath, ffmpegMin);
        //        var taskUpper = ExtractFrameAsync(videoPath, ffmpegMax);

        //        var results = await Task.WhenAll(taskLower, taskUpper);

        //        pictureBox1.Image = results[0];
        //        pictureBox2.Image = results[1];
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show("Error extracting frames: " + ex.Message);
        //    }
        //}

        private async void btnShowRange_Click(object sender, EventArgs e)
        {
            await UpdateStartEndFrame();
        }

        private async Task UpdateStartEndFrame()
        {
            // Get input video
            string videoPath = mediaHistory[mediaIndex].Path;
            if (!File.Exists(videoPath))
            {
                MessageBox.Show("Video not found.");
                return;
            }


            int minFrame = (int)numMinFrame.Value;
            int maxFrame = (int)numMaxFrame.Value;

            minFrame = Math.Max(1, minFrame);
            maxFrame = Math.Max(1, maxFrame);

            string tempDir = Path.Combine(Path.GetTempPath(), "VideoFramePreview");
            if (!Directory.Exists(tempDir)) Directory.CreateDirectory(tempDir);

            // Adjust for FFmpeg 0-based frame index
            int ffmpegMin = minFrame - 1;
            int ffmpegMax = maxFrame - 1;

            string minFramePath = Path.Combine(tempDir, $"frame_{minFrame}.png");
            string maxFramePath = Path.Combine(tempDir, $"frame_{maxFrame}.png");

            // Clear pictureboxes to release any locks
            if (pictureBox1.Image != null)
            {
                pictureBox1.Image.Dispose();
                pictureBox1.Image = null;
            }

            if (pictureBox2.Image != null)
            {
                pictureBox2.Image.Dispose();
                pictureBox2.Image = null;
            }
            // Delete existing files if they exist to avoid stale frames
            if (File.Exists(minFramePath)) File.Delete(minFramePath);
            if (File.Exists(maxFramePath)) File.Delete(maxFramePath);

            // Extract frames using FFmpeg
            var convMin = FFmpeg.Conversions.New()
                .AddParameter($"-i \"{videoPath}\" -vf \"select=eq(n\\,{ffmpegMin})\" -vframes 1 \"{minFramePath}\"", ParameterPosition.PreInput);
            await convMin.Start();

            var convMax = FFmpeg.Conversions.New()
                .AddParameter($"-i \"{videoPath}\" -vf \"select=eq(n\\,{ffmpegMax})\" -vframes 1 \"{maxFramePath}\"", ParameterPosition.PreInput);
            await convMax.Start();

            // Load into PictureBoxes (or any UI image control)
            pictureBox1.Image?.Dispose();
            pictureBox2.Image?.Dispose();

            pictureBox1.Image = Image.FromFile(minFramePath);
            pictureBox2.Image = Image.FromFile(maxFramePath);

            //workaround
            pictureBox1.Tag = minFramePath;
            pictureBox2.Tag = maxFramePath;

            //test. load first frame to webview

            //string mediaPath = new Uri(minFramePath).AbsoluteUri;

            // Call the JS function to load the media dynamically
            //string script = $"window.showMedia('{mediaPath}');";
            //await webView21.CoreWebView2.ExecuteScriptAsync(script);
            //Console.WriteLine("index " + mediaIndex + " / count" + mediaHistory.Count);

            //DebugPrint($"Displayed frames {minFrame} and {maxFrame}");
            updatedStartEnd = true;
        }

       

        private int ExtractProcessedIndex(string folderPath, string baseName)
        {
            string name = Path.GetFileName(folderPath);

            // base_frames_processed        → index 0
            // base_frames_processed(1)     → index 1
            // base_frames_processed_2      → index 2

            if (name.Equals($"{baseName}_frames_processed", StringComparison.OrdinalIgnoreCase))
                return 0;

            var match = Regex.Match(name, @"processed[\(_](\d+)\)?$");
            return match.Success ? int.Parse(match.Groups[1].Value) : -1;
        }

        private string GetNextProcessedFramesFolder(string videoPath)
        {
            string dir = Path.GetDirectoryName(videoPath);
            string baseName = GetBaseVideoName(videoPath);

            int outputIndex = GetOutputIndex(videoPath);

            if (outputIndex == 0)
            {
                return Path.Combine(dir, $"{baseName}_frames_processed");
            }

            return Path.Combine(dir, $"{baseName}_frames_processed_{outputIndex}");
        }


        //private string GetNextProcessedFramesFolder(string videoPath)
        //{
        //    string folder = Path.GetDirectoryName(videoPath);
        //    string baseName = GetBaseVideoName(videoPath);

        //    int i = 1;
        //    string path;

        //    do
        //    {
        //        path = Path.Combine(folder, $"{baseName}_frames_processed_{i}");
        //        i++;
        //    } while (Directory.Exists(path));

        //    Directory.CreateDirectory(path);
        //    return path;
        //}
        private int GetOutputIndex(string path)
        {
            string name = Path.GetFileNameWithoutExtension(path);

            var match = Regex.Match(name, @"_output\((\d+)\)$");
            if (match.Success)
                return int.Parse(match.Groups[1].Value);

            return 0; // original source
        }

        private string GetRawFramesFolder(string videoPath) //Returns the path for the frames folder for file at that path
        {
            string dir = Path.GetDirectoryName(videoPath);
            string baseName = GetBaseVideoName(videoPath);
            return Path.Combine(dir, $"{baseName}_frames");
        }

        private string GetProcessedFramesFolder(string videoPath, int index) //Returns the path for the processed frames folder for file at that path
        {
            string dir = Path.GetDirectoryName(videoPath);
            string baseName = GetBaseVideoName(videoPath);
            return Path.Combine(dir, $"{baseName}_frames_processed({index})");
        }


        //private string ResolveFrameSourceFolder(string videoPath)
        //{
        //    string folder = Path.GetDirectoryName(videoPath);
        //    string baseName = GetBaseVideoName(videoPath);

        //    string processedFrames = Path.Combine(folder, $"{baseName}_frames_processed");

        //    if (checkReuseProcessed.Checked && Directory.Exists(processedFrames))
        //        return processedFrames;

        //    // Default: raw extracted frames
        //    return Path.Combine(folder, $"{baseName}_frames");
        //}

        private string ResolveFrameSourceFolder(string videoPath)
        {
            string dir = Path.GetDirectoryName(videoPath);
            string baseName = GetBaseVideoName(videoPath);

            int outputIndex = GetOutputIndex(videoPath);

            if (outputIndex == 0)
            {
                // Original video → raw extracted frames
                return Path.Combine(dir, $"{baseName}_frames");
            }

            if (outputIndex == 1)
            {
                return Path.Combine(dir, $"{baseName}_frames_processed");
            }

            // output(2)+ → previous processed folder
            return Path.Combine(dir, $"{baseName}_frames_processed_{outputIndex - 1}");
        }


        private string GetBaseVideoName(string path)
        {
            string name = Path.GetFileNameWithoutExtension(path);
            return Regex.Replace(name, @"_output\(\d+\)$", "");
        }

        private void checkReuseProcessed_CheckedChanged(object sender, EventArgs e)
        {

        }

        private async void btnFrameStart_Click(object sender, EventArgs e)
        {
            if (!updatedStartEnd)
            {
                await UpdateStartEndFrame();
            }
            GetMaskWait(); //"Save" current mask

            //Console.WriteLine("pb location " + pictureBox1.Tag);

            string mediaPath = new Uri(pictureBox1.Tag.ToString()).AbsoluteUri;

            // Call the JS function to load the media dynamically
            string script = $"window.showMedia('{mediaPath}');";
            await webView21.CoreWebView2.ExecuteScriptAsync(script);
            LoadCachedMask();
            tabControl1.SelectedIndex = 0;
        }

        private async void btnFrameEnd_Click(object sender, EventArgs e)
        {
            if (!updatedStartEnd)
            {
                await UpdateStartEndFrame();
            }
            GetMaskWait(); //"Save" current mask

            //Console.WriteLine("pb location " + pictureBox1.Tag);

            string mediaPath = new Uri(pictureBox2.Tag.ToString()).AbsoluteUri;

            // Call the JS function to load the media dynamically
            string script = $"window.showMedia('{mediaPath}');";
            await webView21.CoreWebView2.ExecuteScriptAsync(script);
            LoadCachedMask();
            tabControl1.SelectedIndex = 0;
        }

        private async void GetMaskWait()
        {
            // Request mask from JS
            try
            {
                await webView21.CoreWebView2.ExecuteScriptAsync("window.sendMaskToHost();");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to request mask: " + ex.Message);
                Console.WriteLine("Failed to request mask: " + ex.Message);
                btnCensor.Enabled = true;
                return;
            }

            // Wait for mask to be received (poll every 100ms for max 5s)
            int retries = 50;
            while (maskBitmap == null && retries-- > 0)
            {
                await Task.Delay(100);
            }
        }

        private void numMinFrame_ValueChanged(object sender, EventArgs e)
        {
            updatedStartEnd = false;
        }

        private void numMaxFrame_ValueChanged(object sender, EventArgs e)
        {
            updatedStartEnd = false;
        }

        private void btnStep_Click(object sender, EventArgs e)
        {
            int min = (int)numMinFrame.Value;
            int max = (int)numMaxFrame.Value;

            int range = max - min;

            min = max + 1;
            max = max + range + 1;

            numMinFrame.Value = min;
            numMaxFrame.Value = max;
        }

        private void btnPurge_Click(object sender, EventArgs e)
        {
            MediaItem media = mediaHistory[mediaIndex]; 
            string mediaPath = media.Path;

            string dir = Path.GetDirectoryName(mediaPath);

            if (!Directory.Exists(dir))
                return;

            // Ensure all image/file handles are released
            GC.Collect();
            GC.WaitForPendingFinalizers();

            foreach (string folder in Directory.GetDirectories(dir))
            {
                string name = Path.GetFileName(folder);

                // Match any temp frame folders
                if (name.Contains("_frames") ||
                    name.Contains("_frames_processed"))
                {
                    try
                    {
                        Directory.Delete(folder, recursive: true);
                        DebugPrint($"Deleted temp folder: {folder}");
                    }
                    catch (Exception ex)
                    {
                        DebugPrint($"Failed to delete {folder}: {ex.Message}");
                    }
                }
            }
        }


        //private void ShowMedia(MediaItem media)
        //{
        //    if (!webView2Initialized) return;

        //    if (media.Type == MediaType.Video)
        //    {
        //        string url = new Uri(media.Path).AbsoluteUri;
        //        string html = $@"
        //    <video autoplay loop controls style='width:100%; height:100%;'>
        //        <source src='{url}'>
        //    </video>";
        //        webView21.NavigateToString(html);
        //    }
        //    else if (media.Type == MediaType.Gif || media.Type == MediaType.Image)
        //    {
        //        string url = new Uri(media.Path).AbsoluteUri;
        //        webView21.NavigateToString(url);
        //    }
        //}
    }
}








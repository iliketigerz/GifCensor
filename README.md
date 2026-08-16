# GifCensor
Desktop program that lets you easily mess with different censoring effects, for images, gifs and videos. I had a lot of fun messing with ezgif's censor function but wanted more features.

![Example gif](https://github.com/iliketigerz/GifCensor/blob/main/docs/BBB_apple_chroma_pixel.gif)

Effects
======================
* Pixelate
* Blur
* Solid color fill
* Noise fill
* RGB shift
* Glitch
* Hue shift

Masking tools
======================
* Rectangle select
* Paint select
* Fill
* Mask inversion
* Retrieve last mask
* Opacity
* Chroma key


Misc features
======================
* Effects can be applied to frames within a certain range, user selectable. Compatible with all modes.
* "Reuse processed frames" and "Encode video" settings - When applying lots of sequential edits (ie to a long video), you can stop the program from rencoding the video after every edit, and reuse the last edit's frames to save time and preserve quality. Just renable "Encode video" before the final edit is made (or just don't apply any effect and hit process)

Supported file formats (TBC)
======================

* Images: `.jpg`, `.png`, `.gif`
* Video: `.mp4`, `.webm`,

Quick tutorial
======================
Drag your file onto the application, anywhere except the large box in the middle of the screen. You can then set up your mask on the image, Left click to paint, right click to erase. Middle mouse to pan and use the scroll wheel to zoom

Once your mask is done, pick an effect from the lower left menu. Click "process" in the bottom right corner to apply the effect. 

The processed media will then be displayed in the window. It will appear in the same directory as the source file, but with "_output" appended to the file name. Subsequent edits will appear with "_output(n)". You can jump forwards and backwards through a list of your processed files with the buttons just above the process button, useful if you want to retry an edit if the settings weren't quite right or start again. You can clear this list with the "Clear images" button.
For a more detailed look, please read the [wiki](https://github.com/iliketigerz/GifCensor/wiki).

Limitations, things to look out for
======================
When gifs are processed each of the frames are held in memory. Large or very long gifs can eat a lot of RAM when processed.
When you process a video, a folder called "(filename)_frames" will be created to store all of the frames on disc. Processed frames (with effect applied) appear in a new folder called "(filename)_processed". If "reuse processed frames" is enabled, when you apply the next effect the frames from the corresponding _processed folder will be reused. You can delete these folders with the "purge temp files" button. This can however cause issues if you then go back in the history and apply an edit from scratch, it will just use the newest _processed_frames folder. This is to be fixed later, for now just purging the folders works.

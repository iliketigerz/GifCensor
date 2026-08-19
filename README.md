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
* Video: `.mp4`, `.webm`, `.mov`

Quick tutorial
======================
Drag your file onto the application. You can then set up your mask on the image, Left click to paint, right click to erase. Middle mouse to pan and use the scroll wheel to zoom

Once your mask is done, pick an effect from the lower left menu. Click "process" in the bottom right corner to apply the effect. 

The processed media will then be displayed in the window. It will appear in the same directory as the source file, but with "_output" appended to the file name. You can jump forwards and backwards through a list of your processed files with the buttons just above the process button.

For full information, please read the [wiki](https://github.com/iliketigerz/GifCensor/wiki)!

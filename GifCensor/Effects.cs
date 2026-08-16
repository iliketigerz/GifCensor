using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Drawing;
using System.Drawing.Imaging;
using Microsoft.Web.WebView2.Core;
using System.Drawing.Drawing2D;

namespace GifCensor
{
    class Effects
    {
        public static Bitmap ApplyRGBShift(Bitmap image, Bitmap mask, int maxOffsetX, int maxOffsetY)
        {
            Random rand = new Random();

            // Clone image to 32bppArgb bitmap and copy DPI to avoid scaling issues
            Bitmap shiftedImage = new Bitmap(image.Width, image.Height, PixelFormat.Format32bppArgb);
            shiftedImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);
            using (Graphics g = Graphics.FromImage(shiftedImage))
                g.DrawImage(image, 0, 0);

            // Clone mask similarly
            Bitmap maskClone = new Bitmap(mask.Width, mask.Height, PixelFormat.Format32bppArgb);
            maskClone.SetResolution(mask.HorizontalResolution, mask.VerticalResolution);
            using (Graphics g = Graphics.FromImage(maskClone))
                g.DrawImage(mask, 0, 0);

            Rectangle rect = new Rectangle(0, 0, image.Width, image.Height);

            BitmapData imgData = shiftedImage.LockBits(rect, ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            BitmapData maskData = maskClone.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

            int imgStride = imgData.Stride;
            int maskStride = maskData.Stride;

            unsafe
            {
                byte* imgPtr = (byte*)imgData.Scan0.ToPointer();
                byte* maskPtr = (byte*)maskData.Scan0.ToPointer();

                for (int y = 0; y < image.Height; y++)
                {
                    for (int x = 0; x < image.Width; x++)
                    {
                        byte* maskPixel = maskPtr + y * maskStride + x * 4;
                        if (maskPixel[3] < 128)  // Skip transparent in mask
                            continue;

                        byte* pixel = imgPtr + y * imgStride + x * 4;

                        if (pixel[3] < 10) // Skip mostly transparent pixels in image
                            continue;

                        int newX = x + rand.Next(-maxOffsetX, maxOffsetX + 1);
                        int newY = y + rand.Next(-maxOffsetY, maxOffsetY + 1);

                        if (newX < 0) newX = 0;
                        else if (newX >= image.Width) newX = image.Width - 1;

                        if (newY < 0) newY = 0;
                        else if (newY >= image.Height) newY = image.Height - 1;

                        byte* shiftedPixel = imgPtr + newY * imgStride + newX * 4;

                        // Shift RGB channels only, keep original alpha
                        pixel[0] = shiftedPixel[0]; // Blue
                        pixel[1] = shiftedPixel[1]; // Green
                        pixel[2] = shiftedPixel[2]; // Red
                    }
                }
            }

            shiftedImage.UnlockBits(imgData);
            maskClone.UnlockBits(maskData);
            maskClone.Dispose();

            return shiftedImage;
        }




        public static Bitmap Blur(Bitmap image, Bitmap mask, int radius)
        {
            if (radius < 1)
                return (Bitmap)image.Clone();

            int width = image.Width;
            int height = image.Height;

            // Force both to correct format and size
            Bitmap source = new Bitmap(width, height, PixelFormat.Format32bppArgb);
            using (Graphics g = Graphics.FromImage(source))
                g.DrawImageUnscaled(image, 0, 0);

            Bitmap maskClone = new Bitmap(width, height, PixelFormat.Format32bppArgb);
            using (Graphics g = Graphics.FromImage(maskClone))
                g.DrawImageUnscaled(mask, 0, 0);

            Bitmap result = new Bitmap(width, height, PixelFormat.Format32bppArgb);
            Rectangle rect = new Rectangle(0, 0, width, height);

            // Gaussian kernel generation
            double sigma = radius / 3.0;
            int kernelSize = 2 * radius + 1;
            double[] kernel = new double[kernelSize];
            double kernelSum = 0;
            for (int i = 0; i < kernelSize; i++)
            {
                int x = i - radius;
                kernel[i] = Math.Exp(-(x * x) / (2 * sigma * sigma));
                kernelSum += kernel[i];
            }
            for (int i = 0; i < kernelSize; i++)
                kernel[i] /= kernelSum;

            // Lock bits
            BitmapData srcData = source.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            BitmapData maskData = maskClone.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            BitmapData resData = result.LockBits(rect, ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);

            int stride = srcData.Stride;
            int maskStride = maskData.Stride;
            int resStride = resData.Stride;

            unsafe
            {
                byte* srcPtr = (byte*)srcData.Scan0;
                byte* maskPtr = (byte*)maskData.Scan0;
                byte* resPtr = (byte*)resData.Scan0;

                byte[] tempRow = new byte[width * 4];

                // Horizontal pass
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        byte* maskPixel = maskPtr + y * maskStride + x * 4;
                        if (maskPixel[3] < 128)
                        {
                            byte* orig = srcPtr + y * stride + x * 4;
                            int idx = x * 4;
                            tempRow[idx] = orig[0];
                            tempRow[idx + 1] = orig[1];
                            tempRow[idx + 2] = orig[2];
                            tempRow[idx + 3] = orig[3];
                            continue;
                        }

                        double b = 0, g = 0, r = 0, a = 0;

                        for (int k = -radius; k <= radius; k++)
                        {
                            int nx = x + k;
                            if (nx < 0) nx = 0;
                            else if (nx >= width) nx = width - 1;

                            byte* p = srcPtr + y * stride + nx * 4;
                            double w = kernel[k + radius];
                            b += p[0] * w;
                            g += p[1] * w;
                            r += p[2] * w;
                            a += p[3] * w;
                        }

                        int index = x * 4;
                        tempRow[index] = (byte)b;
                        tempRow[index + 1] = (byte)g;
                        tempRow[index + 2] = (byte)r;
                        tempRow[index + 3] = (byte)a;
                    }

                    byte* destRow = resPtr + y * resStride;
                    for (int i = 0; i < width * 4; i++)
                        destRow[i] = tempRow[i];
                }

                byte[] tempCol = new byte[height * 4];

                // Vertical pass
                for (int x = 0; x < width; x++)
                {
                    for (int y = 0; y < height; y++)
                    {
                        byte* maskPixel = maskPtr + y * maskStride + x * 4;
                        if (maskPixel[3] < 128)
                        {
                            byte* orig = srcPtr + y * stride + x * 4;
                            int idx = y * 4;
                            tempCol[idx] = orig[0];
                            tempCol[idx + 1] = orig[1];
                            tempCol[idx + 2] = orig[2];
                            tempCol[idx + 3] = orig[3];
                            continue;
                        }

                        double b = 0, g = 0, r = 0, a = 0;

                        for (int k = -radius; k <= radius; k++)
                        {
                            int ny = y + k;
                            if (ny < 0) ny = 0;
                            else if (ny >= height) ny = height - 1;

                            byte* p = resPtr + ny * resStride + x * 4;
                            double w = kernel[k + radius];
                            b += p[0] * w;
                            g += p[1] * w;
                            r += p[2] * w;
                            a += p[3] * w;
                        }

                        int index = y * 4;
                        tempCol[index] = (byte)b;
                        tempCol[index + 1] = (byte)g;
                        tempCol[index + 2] = (byte)r;
                        tempCol[index + 3] = (byte)a;
                    }

                    for (int y = 0; y < height; y++)
                    {
                        byte* dest = resPtr + y * resStride + x * 4;
                        int idx = y * 4;
                        dest[0] = tempCol[idx];
                        dest[1] = tempCol[idx + 1];
                        dest[2] = tempCol[idx + 2];
                        dest[3] = tempCol[idx + 3];
                    }
                }
            }

            source.UnlockBits(srcData);
            maskClone.UnlockBits(maskData);
            result.UnlockBits(resData);

            source.Dispose();
            maskClone.Dispose();

            return result;
        }






        //public static Bitmap Fill(Bitmap image, Bitmap mask, Color fillColor)
        //{
        //    int width = image.Width;
        //    int height = image.Height;

        //    // Clone the image to 32bppArgb (safe)
        //    Bitmap filled = new Bitmap(width, height, PixelFormat.Format32bppArgb);
        //    using (Graphics g = Graphics.FromImage(filled))
        //        g.DrawImage(image, 0, 0, width, height);

        //    // SAFELY clone the mask using exact size
        //    Bitmap maskClone = new Bitmap(mask.Width, mask.Height, PixelFormat.Format32bppArgb);
        //    using (Graphics g = Graphics.FromImage(maskClone))
        //        g.DrawImage(mask, 0, 0, mask.Width, mask.Height);

        //    Rectangle rect = new Rectangle(0, 0, Math.Min(width, mask.Width), Math.Min(height, mask.Height));

        //    BitmapData imgData = filled.LockBits(rect, ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
        //    BitmapData maskData = maskClone.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

        //    int imgStride = imgData.Stride;
        //    int maskStride = maskData.Stride;

        //    unsafe
        //    {
        //        byte* imgPtr = (byte*)imgData.Scan0;
        //        byte* maskPtr = (byte*)maskData.Scan0;

        //        byte r = fillColor.R;
        //        byte g = fillColor.G;
        //        byte b = fillColor.B;

        //        for (int y = 0; y < rect.Height; y++)
        //        {
        //            for (int x = 0; x < rect.Width; x++)
        //            {
        //                byte* maskPixel = maskPtr + y * maskStride + x * 4;
        //                if (maskPixel[3] < 128)
        //                    continue;

        //                byte* dest = imgPtr + y * imgStride + x * 4;

        //                dest[0] = b;
        //                dest[1] = g;
        //                dest[2] = r;
        //                dest[3] = 255;
        //            }
        //        }
        //    }

        //    filled.UnlockBits(imgData);
        //    maskClone.UnlockBits(maskData);
        //    maskClone.Dispose();

        //    return filled;
        //}

        public static Bitmap Fill(Bitmap image, Bitmap mask, Color fillColor, float alpha)
        {
            if (alpha <= 0f) return (Bitmap)image.Clone();
            if (alpha > 1f) alpha = 1f;

            int width = image.Width;
            int height = image.Height;

            Bitmap filled = new Bitmap(width, height, PixelFormat.Format32bppArgb);
            using (Graphics g = Graphics.FromImage(filled))
                g.DrawImage(image, 0, 0, width, height);

          // Always clone mask to a private 32bpp bitmap
Bitmap maskClone = new Bitmap(width, height, PixelFormat.Format32bppArgb);
using (Graphics g = Graphics.FromImage(maskClone))
{
    g.DrawImage(mask, 0, 0, width, height);
}


            if (maskClone != mask)
            {
                using (Graphics g = Graphics.FromImage(maskClone))
                    g.DrawImage(mask, 0, 0, mask.Width, mask.Height);
            }

            Rectangle rect = new Rectangle(0, 0, Math.Min(width, mask.Width), Math.Min(height, mask.Height));

            BitmapData imgData = filled.LockBits(rect, ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            BitmapData maskData = maskClone.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

            int imgStride = imgData.Stride;
            int maskStride = maskData.Stride;

            // Precompute integer alpha values
            int alphaInt = (int)(alpha * 256);
            int invAlphaInt = 256 - alphaInt;
            int rFi = fillColor.R;
            int gFi = fillColor.G;
            int bFi = fillColor.B;

            unsafe
            {
                byte* imgPtr = (byte*)imgData.Scan0;
                byte* maskPtr = (byte*)maskData.Scan0;

                // Parallelize over rows for large images
                Parallel.For(0, rect.Height, y =>
                {
                    byte* imgRow = imgPtr + y * imgStride;
                    byte* maskRow = maskPtr + y * maskStride;

                    for (int x = 0; x < rect.Width; x++)
                    {
                        byte* maskPixel = maskRow + x * 4;
                        if (maskPixel[3] < 128)
                            continue;

                        byte* dest = imgRow + x * 4;

                        dest[0] = (byte)((bFi * alphaInt + dest[0] * invAlphaInt) >> 8);
                        dest[1] = (byte)((gFi * alphaInt + dest[1] * invAlphaInt) >> 8);
                        dest[2] = (byte)((rFi * alphaInt + dest[2] * invAlphaInt) >> 8);
                        // Keep original alpha
                    }
                });
            }

            filled.UnlockBits(imgData);
            maskClone.UnlockBits(maskData);

            if (maskClone != mask) maskClone.Dispose();

            return filled;
        }

        public static Bitmap FillWithStatic(Bitmap image, Bitmap mask, float alpha)
        {
            if (alpha < 0f) alpha = 0f;
            if (alpha > 1f) alpha = 1f;

            Bitmap source = new Bitmap(image.Width, image.Height, PixelFormat.Format32bppArgb);
            using (Graphics g = Graphics.FromImage(source))
                g.DrawImage(image, 0, 0);

            Bitmap maskClone = new Bitmap(mask.Width, mask.Height, PixelFormat.Format32bppArgb);
            using (Graphics g = Graphics.FromImage(maskClone))
                g.DrawImage(mask, 0, 0);

            Rectangle rect = new Rectangle(0, 0, image.Width, image.Height);
            BitmapData imgData = source.LockBits(rect, ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            BitmapData maskData = maskClone.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

            int imgStride = imgData.Stride;
            int maskStride = maskData.Stride;
            Random rand = new Random();

            unsafe
            {
                byte* imgPtr = (byte*)imgData.Scan0;
                byte* maskPtr = (byte*)maskData.Scan0;

                for (int y = 0; y < image.Height; y++)
                {
                    for (int x = 0; x < image.Width; x++)
                    {
                        byte* maskPixel = maskPtr + y * maskStride + x * 4;
                        if (maskPixel[3] < 128)
                            continue;

                        byte* dest = imgPtr + y * imgStride + x * 4;

                        // Original pixel
                        byte bO = dest[0];
                        byte gO = dest[1];
                        byte rO = dest[2];

                        // Random overlay
                        byte bR = (byte)rand.Next(256);
                        byte gR = (byte)rand.Next(256);
                        byte rR = (byte)rand.Next(256);

                        // Alpha blend
                        dest[0] = (byte)(bR * alpha + bO * (1f - alpha));
                        dest[1] = (byte)(gR * alpha + gO * (1f - alpha));
                        dest[2] = (byte)(rR * alpha + rO * (1f - alpha));
                        // Keep original alpha
                        // dest[3] = dest[3];
                    }
                }
            }

            source.UnlockBits(imgData);
            maskClone.UnlockBits(maskData);
            maskClone.Dispose();

            return source;
        }

        //public static Bitmap FillWithStatic(Bitmap image, Bitmap mask)
        //{
        //    Bitmap source = new Bitmap(image.Width, image.Height, PixelFormat.Format32bppArgb);
        //    using (Graphics g = Graphics.FromImage(source))
        //        g.DrawImage(image, 0, 0);

        //    Bitmap maskClone = new Bitmap(mask.Width, mask.Height, PixelFormat.Format32bppArgb);
        //    using (Graphics g = Graphics.FromImage(maskClone))
        //        g.DrawImage(mask, 0, 0);

        //    Rectangle rect = new Rectangle(0, 0, image.Width, image.Height);
        //    BitmapData imgData = source.LockBits(rect, ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
        //    BitmapData maskData = maskClone.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

        //    int imgStride = imgData.Stride;
        //    int maskStride = maskData.Stride;
        //    Random rand = new Random();

        //    unsafe
        //    {
        //        byte* imgPtr = (byte*)imgData.Scan0;
        //        byte* maskPtr = (byte*)maskData.Scan0;

        //        for (int y = 0; y < image.Height; y++)
        //        {
        //            for (int x = 0; x < image.Width; x++)
        //            {
        //                byte* maskPixel = maskPtr + y * maskStride + x * 4;
        //                if (maskPixel[3] < 128)
        //                    continue;

        //                byte* dest = imgPtr + y * imgStride + x * 4;

        //                dest[0] = (byte)rand.Next(256);
        //                dest[1] = (byte)rand.Next(256);
        //                dest[2] = (byte)rand.Next(256);
        //                dest[3] = 255;
        //            }
        //        }
        //    }

        //    source.UnlockBits(imgData);
        //    maskClone.UnlockBits(maskData);
        //    maskClone.Dispose();

        //    return source;
        //}

        public static unsafe Bitmap ApplyLineJitter(Bitmap image, Bitmap mask, int lineOffsetRange, int stripeHeight)
        {
            // Clone image with 32bppArgb and copy DPI
            Bitmap corruptedImage = new Bitmap(image.Width, image.Height, PixelFormat.Format32bppArgb);
            corruptedImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);
            using (Graphics g = Graphics.FromImage(corruptedImage))
                g.DrawImage(image, 0, 0);

            // Clone mask similarly and copy DPI
            Bitmap maskClone = new Bitmap(mask.Width, mask.Height, PixelFormat.Format32bppArgb);
            maskClone.SetResolution(mask.HorizontalResolution, mask.VerticalResolution);
            using (Graphics g = Graphics.FromImage(maskClone))
                g.DrawImage(mask, 0, 0);

            Rectangle rect = new Rectangle(0, 0, image.Width, image.Height);

            BitmapData imgData = corruptedImage.LockBits(rect, ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            BitmapData maskData = maskClone.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

            int imgStride = imgData.Stride;
            int maskStride = maskData.Stride;

            byte* imgPtr = (byte*)imgData.Scan0.ToPointer();
            byte* maskPtr = (byte*)maskData.Scan0.ToPointer();

            Random rand = new Random();

            // Apply distortion in horizontal stripes
            for (int y = 0; y < image.Height; y += stripeHeight)
            {
                bool shouldApplyDistortion = false;

                for (int x = 0; x < image.Width; x++)
                {
                    if (IsMaskPixelVisible(maskPtr, maskStride, x, y))
                    {
                        shouldApplyDistortion = true;
                        break;
                    }
                }

                if (shouldApplyDistortion)
                {
                    int offset = rand.Next(-lineOffsetRange, lineOffsetRange + 1);

                    for (int x = 0; x < image.Width; x++)
                    {
                        if (IsMaskPixelVisible(maskPtr, maskStride, x, y))
                        {
                            byte* pixel = imgPtr + y * imgStride + x * 4;
                            int xOffset = x + offset;

                            if (xOffset >= 0 && xOffset < image.Width)
                            {
                                byte* shiftedPixel = imgPtr + y * imgStride + xOffset * 4;

                                // Swap all 4 channels including alpha
                                for (int c = 0; c < 4; c++)
                                {
                                    byte temp = pixel[c];
                                    pixel[c] = shiftedPixel[c];
                                    shiftedPixel[c] = temp;
                                }
                            }
                        }
                    }
                }
            }

            corruptedImage.UnlockBits(imgData);
            maskClone.UnlockBits(maskData);
            maskClone.Dispose();

            return corruptedImage;
        }

        // Helper function to check if a specific pixel is visible in the mask (based on alpha channel)
        unsafe private static bool IsMaskPixelVisible(byte* maskPtr, int maskStride, int x, int y)
        {
            byte* maskPixel = maskPtr + y * maskStride + x * 4;  // Assuming mask is in 32bppArgb format (4 bytes per pixel)
            return maskPixel[3] > 128;  // If alpha is greater than 128, consider it visible
        }

        public static Bitmap FillWithGrayscaleNoise(Bitmap image, Bitmap mask, float alpha)
        {
            if (alpha <= 0f) return (Bitmap)image.Clone();
            if (alpha > 1f) alpha = 1f;

            int width = image.Width;
            int height = image.Height;
            int alphaInt = (int)(alpha * 256); // scale 0..256 for integer math
            int invAlphaInt = 256 - alphaInt;

            Bitmap output = new Bitmap(width, height, PixelFormat.Format32bppArgb);
            using (Graphics g = Graphics.FromImage(output))
                g.DrawImage(image, 0, 0, width, height);

            Bitmap maskClone = new Bitmap(mask.Width, mask.Height, PixelFormat.Format32bppArgb);
            using (Graphics g = Graphics.FromImage(maskClone))
                g.DrawImage(mask, 0, 0, mask.Width, mask.Height);

            Rectangle rect = new Rectangle(0, 0, width, height);
            BitmapData outData = output.LockBits(rect, ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            BitmapData maskData = maskClone.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

            int outStride = outData.Stride;
            int maskStride = maskData.Stride;

            Random rand = new Random();

            unsafe
            {
                byte* outPtr = (byte*)outData.Scan0;
                byte* maskPtr = (byte*)maskData.Scan0;

                for (int y = 0; y < height; y++)
                {
                    byte* destRow = outPtr + y * outStride;
                    byte* maskRow = maskPtr + y * maskStride;

                    for (int x = 0; x < width; x++)
                    {
                        byte* maskPixel = maskRow + x * 4;
                        if (maskPixel[3] < 128)
                            continue;

                        byte* dest = destRow + x * 4;

                        // Generate grayscale noise as integer 0..255
                        int gray = rand.Next(256);

                        // Fast integer alpha blending
                        dest[0] = (byte)((gray * alphaInt + dest[0] * invAlphaInt) >> 8);
                        dest[1] = (byte)((gray * alphaInt + dest[1] * invAlphaInt) >> 8);
                        dest[2] = (byte)((gray * alphaInt + dest[2] * invAlphaInt) >> 8);
                        // Keep original alpha
                    }
                }
            }

            output.UnlockBits(outData);
            maskClone.UnlockBits(maskData);
            maskClone.Dispose();

            return output;
        }



        //public static Bitmap FillWithGrayscaleNoise(Bitmap image, Bitmap mask)
        //{
        //    // Convert image to 32bppArgb
        //    Bitmap source = new Bitmap(image.Width, image.Height, PixelFormat.Format32bppArgb);
        //    using (Graphics g = Graphics.FromImage(source))
        //    {
        //        g.DrawImage(image, 0, 0, image.Width, image.Height);
        //    }

        //    // Convert mask to 32bppArgb
        //    Bitmap maskClone = new Bitmap(mask.Width, mask.Height, PixelFormat.Format32bppArgb);
        //    using (Graphics g = Graphics.FromImage(maskClone))
        //    {
        //        g.DrawImage(mask, 0, 0, mask.Width, mask.Height);
        //    }

        //    Bitmap output = new Bitmap(image.Width, image.Height, PixelFormat.Format32bppArgb);
        //    using (Graphics g = Graphics.FromImage(output))
        //    {
        //        g.DrawImage(source, 0, 0, image.Width, image.Height);
        //    }

        //    Rectangle rect = new Rectangle(0, 0, image.Width, image.Height);

        //    BitmapData outData = output.LockBits(rect, ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
        //    BitmapData maskData = maskClone.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

        //    int outStride = outData.Stride;
        //    int maskStride = maskData.Stride;

        //    Random rand = new Random();

        //    unsafe
        //    {
        //        byte* outPtr = (byte*)outData.Scan0;
        //        byte* maskPtr = (byte*)maskData.Scan0;

        //        for (int y = 0; y < image.Height; y++)
        //        {
        //            for (int x = 0; x < image.Width; x++)
        //            {
        //                byte* maskPixel = maskPtr + y * maskStride + x * 4;
        //                if (maskPixel[3] < 128)
        //                    continue;

        //                byte* dest = outPtr + y * outStride + x * 4;

        //                byte gray = (byte)rand.Next(256);
        //                dest[0] = gray; // Blue
        //                dest[1] = gray; // Green
        //                dest[2] = gray; // Red
        //                dest[3] = 255;  // Fully opaque
        //            }
        //        }
        //    }

        //    output.UnlockBits(outData);
        //    maskClone.UnlockBits(maskData);

        //    source.Dispose();
        //    maskClone.Dispose();

        //    return output;
        //}


        public static Bitmap Pixelate(Bitmap image, Bitmap mask, int pixelateSize, float alpha)
{
    if (alpha < 0f) alpha = 0f;
    if (alpha > 1f) alpha = 1f;

    Bitmap source = new Bitmap(image.Width, image.Height, PixelFormat.Format32bppArgb);
    using (Graphics g = Graphics.FromImage(source))
        g.DrawImage(image, 0, 0, image.Width, image.Height);

    Bitmap maskClone = new Bitmap(mask.Width, mask.Height, PixelFormat.Format32bppArgb); //error, object reference not set
    using (Graphics g = Graphics.FromImage(maskClone))
        g.DrawImage(mask, 0, 0, mask.Width, mask.Height);

    Bitmap output = new Bitmap(image.Width, image.Height, PixelFormat.Format32bppArgb);
    Rectangle rect = new Rectangle(0, 0, image.Width, image.Height);

    BitmapData srcData = source.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
    BitmapData maskData = maskClone.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
    BitmapData outData = output.LockBits(rect, ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);

    int width = image.Width;
    int height = image.Height;

    int srcStride = srcData.Stride;
    int maskStride = maskData.Stride;
    int outStride = outData.Stride;

    unsafe
    {
        byte* srcPtr = (byte*)srcData.Scan0;
        byte* maskPtr = (byte*)maskData.Scan0;
        byte* outPtr = (byte*)outData.Scan0;

        for (int y = 0; y < height; y += pixelateSize)
        {
            for (int x = 0; x < width; x += pixelateSize)
            {
                int blockWidth = Math.Min(pixelateSize, width - x);
                int blockHeight = Math.Min(pixelateSize, height - y);

                // Check if any pixel in this block is masked
                bool blockMasked = false;
                for (int yy = 0; yy < blockHeight && !blockMasked; yy++)
                {
                    for (int xx = 0; xx < blockWidth; xx++)
                    {
                        int mx = x + xx;
                        int my = y + yy;
                        byte* m = maskPtr + my * maskStride + mx * 4;
                        if (m[3] >= 128)
                        {
                            blockMasked = true;
                            break;
                        }
                    }
                }

                if (!blockMasked)
                    continue;

                // Sum RGB + alpha
                long sumR = 0, sumG = 0, sumB = 0, sumA = 0;
                int count = 0;

                for (int yy = 0; yy < blockHeight; yy++)
                {
                    for (int xx = 0; xx < blockWidth; xx++)
                    {
                        int px = x + xx;
                        int py = y + yy;

                        byte* s = srcPtr + py * srcStride + px * 4;
                        byte a = s[3];
                        if (a == 0) continue;

                        float aF = a / 255f;
                        sumB += (long)(s[0] / aF);
                        sumG += (long)(s[1] / aF);
                        sumR += (long)(s[2] / aF);
                        sumA += a;
                        count++;
                    }
                }

                if (count == 0)
                    continue;

                byte avgA = (byte)(sumA / count);
                byte avgR = (byte)Math.Min(255, sumR / count);
                byte avgG = (byte)Math.Min(255, sumG / count);
                byte avgB = (byte)Math.Min(255, sumB / count);

                // Apply blended average color to the block
                for (int yy = 0; yy < blockHeight; yy++)
                {
                    for (int xx = 0; xx < blockWidth; xx++)
                    {
                        int px = x + xx;
                        int py = y + yy;

                        byte* d = outPtr + py * outStride + px * 4;
                        byte* s = srcPtr + py * srcStride + px * 4;

                        // Alpha blend
                        d[0] = (byte)Math.Min(255, Math.Max(0, avgB * alpha + s[0] * (1f - alpha) + 0.5f));
                        d[1] = (byte)Math.Min(255, Math.Max(0, avgG * alpha + s[1] * (1f - alpha) + 0.5f));
                        d[2] = (byte)Math.Min(255, Math.Max(0, avgR * alpha + s[2] * (1f - alpha) + 0.5f));
                        d[3] = s[3]; // preserve original alpha
                    }
                }
            }
        }

        // Copy untouched areas
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                byte* d = outPtr + y * outStride + x * 4;
                if (d[3] == 0)
                {
                    byte* s = srcPtr + y * srcStride + x * 4;
                    d[0] = s[0];
                    d[1] = s[1];
                    d[2] = s[2];
                    d[3] = s[3];
                }
            }
        }
    }

    source.UnlockBits(srcData);
    maskClone.UnlockBits(maskData);
    output.UnlockBits(outData);

    source.Dispose();
    maskClone.Dispose();

    return output;
}


        //    public static Bitmap ApplyHslAdjust(
        //Bitmap image,
        //Bitmap mask,
        //int hue,
        //int saturation,
        //int luminance)
        //    {
        //        Bitmap result = new Bitmap(image.Width, image.Height, PixelFormat.Format32bppArgb);
        //        using (Graphics g = Graphics.FromImage(result))
        //            g.DrawImage(image, 0, 0);

        //        Rectangle rect = new Rectangle(0, 0, image.Width, image.Height);


        //        BitmapData imgData = result.LockBits(rect, ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
        //        BitmapData maskData = mask.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

        //        int width = image.Width;
        //        int height = image.Height;
        //        int imgStride = imgData.Stride;
        //        int maskStride = maskData.Stride;

        //        float hueShift = (hue - 100) * 1.8f;
        //        float satFactor = saturation / 100f;
        //        float lumFactor = luminance / 100f;

        //        // LUTs
        //        float[] satLut = new float[256];
        //        float[] lumLut = new float[256];
        //        for (int i = 0; i < 256; i++)
        //        {
        //            float v = i / 255f;
        //            satLut[i] = Math.Min(1f, Math.Max(0f, v * satFactor));
        //            lumLut[i] = Math.Min(1f, Math.Max(0f, v * lumFactor));
        //        }

        //        unsafe
        //        {
        //            byte* imgPtr = (byte*)imgData.Scan0;
        //            byte* maskPtr = (byte*)maskData.Scan0;

        //            for (int y = 0; y < height; y++)
        //            {
        //                // Skip entire row if mask empty
        //                bool rowMasked = false;
        //                byte* maskRow = maskPtr + y * maskStride;
        //                for (int x = 0; x < width; x++)
        //                {
        //                    if (maskRow[x * 4 + 3] >= 128)
        //                    {
        //                        rowMasked = true;
        //                        break;
        //                    }
        //                }
        //                if (!rowMasked) continue;

        //                byte* imgRow = imgPtr + y * imgStride;

        //                for (int x = 0; x < width; x++)
        //                {
        //                    byte* maskPixel = maskRow + x * 4;
        //                    if (maskPixel[3] < 128)
        //                        continue;

        //                    byte* px = imgRow + x * 4;
        //                    byte a = px[3];
        //                    if (a < 10)
        //                        continue;

        //                    float r = px[2] / 255f;
        //                    float g = px[1] / 255f;
        //                    float b = px[0] / 255f;

        //                    RgbToHsl(r, g, b, out float h, out float s, out float l);

        //                    h += hueShift;
        //                    if (h >= 360f) h -= 360f;
        //                    else if (h < 0f) h += 360f;

        //                    s = satLut[(int)(s * 255)];
        //                    l = lumLut[(int)(l * 255)];

        //                    HslToRgb(h, s, l, out r, out g, out b);

        //                    px[2] = (byte)(r * 255);
        //                    px[1] = (byte)(g * 255);
        //                    px[0] = (byte)(b * 255);
        //                }
        //            }
        //        }

        //        result.UnlockBits(imgData);
        //        mask.UnlockBits(maskData);

        //        return result;
        //    }

        public static Bitmap ApplyHslAdjust(
    Bitmap image,
    Bitmap mask,
    int hue,
    int saturation,
    int luminance)
        {
            int width = image.Width;
            int height = image.Height;

            // Clone image to 32bppArgb
            Bitmap result = new Bitmap(width, height, PixelFormat.Format32bppArgb);
            using (Graphics g = Graphics.FromImage(result))
                g.DrawImage(image, 0, 0, width, height);

            // Clone mask safely to match image size
            Bitmap maskClone = new Bitmap(width, height, PixelFormat.Format32bppArgb);
            using (Graphics g = Graphics.FromImage(maskClone))
                g.DrawImage(mask, 0, 0, width, height);

            // Safe rectangle
            Rectangle rect = new Rectangle(0, 0, width, height);

            BitmapData imgData = result.LockBits(rect, ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            BitmapData maskData = maskClone.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

            float hueShift = (hue - 100) * 1.8f;
            float satFactor = saturation / 100f;
            float lumFactor = luminance / 100f;

            // LUTs
            float[] satLut = new float[256];
            float[] lumLut = new float[256];
            for (int i = 0; i < 256; i++)
            {
                float v = i / 255f;
                satLut[i] = Math.Min(1f, Math.Max(0f, v * satFactor));
                lumLut[i] = Math.Min(1f, Math.Max(0f, v * lumFactor));
            }

            int imgStride = imgData.Stride;
            int maskStride = maskData.Stride;

            unsafe
            {
                byte* imgPtr = (byte*)imgData.Scan0;
                byte* maskPtr = (byte*)maskData.Scan0;

                for (int y = 0; y < height; y++)
                {
                    byte* imgRow = imgPtr + y * imgStride;
                    byte* maskRow = maskPtr + y * maskStride;

                    for (int x = 0; x < width; x++)
                    {
                        byte* maskPixel = maskRow + x * 4;
                        if (maskPixel[3] < 128)
                            continue;

                        byte* px = imgRow + x * 4;
                        float r = px[2] / 255f;
                        float g = px[1] / 255f;
                        float b = px[0] / 255f;

                        RgbToHsl(r, g, b, out float h, out float s, out float l);

                        h += hueShift;
                        if (h >= 360f) h -= 360f;
                        else if (h < 0f) h += 360f;

                        s = satLut[(int)(s * 255)];
                        l = lumLut[(int)(l * 255)];

                        HslToRgb(h, s, l, out r, out g, out b);

                        px[2] = (byte)(r * 255);
                        px[1] = (byte)(g * 255);
                        px[0] = (byte)(b * 255);
                    }
                }
            }

            result.UnlockBits(imgData);
            maskClone.UnlockBits(maskData);
            maskClone.Dispose();

            return result;
        }


        private static float Clamp01(float v)
        {
            if (v < 0f) return 0f;
            if (v > 1f) return 1f;
            return v;
        }

        private static void RgbToHsl(float r, float g, float b, out float h, out float s, out float l)
        {
            float max = Math.Max(r, Math.Max(g, b));
            float min = Math.Min(r, Math.Min(g, b));
            l = (max + min) * 0.5f;

            if (max == min)
            {
                h = 0f;
                s = 0f;
                return;
            }

            float d = max - min;
            s = l > 0.5f ? d / (2f - max - min) : d / (max + min);

            if (max == r)
                h = (g - b) / d + (g < b ? 6f : 0f);
            else if (max == g)
                h = (b - r) / d + 2f;
            else
                h = (r - g) / d + 4f;

            h *= 60f;
        }

        private static void HslToRgb(float h, float s, float l, out float r, out float g, out float b)
        {
            if (s == 0f)
            {
                r = g = b = l;
                return;
            }

            float q = l < 0.5f ? l * (1f + s) : l + s - l * s;
            float p = 2f * l - q;

            r = HueToRgb(p, q, h + 120f);
            g = HueToRgb(p, q, h);
            b = HueToRgb(p, q, h - 120f);
        }

        private static float HueToRgb(float p, float q, float t)
        {
            if (t < 0f) t += 360f;
            if (t >= 360f) t -= 360f;

            if (t < 60f) return p + (q - p) * t / 60f;
            if (t < 180f) return q;
            if (t < 240f) return p + (q - p) * (240f - t) / 60f;
            return p;
        }



        //public static Bitmap Pixelate(Bitmap image, Bitmap mask, int pixelateSize)
        //{
        //    Bitmap pixelated = new Bitmap(image.Width, image.Height, PixelFormat.Format32bppArgb);

        //    using (Graphics graphics = Graphics.FromImage(pixelated))
        //    {
        //        graphics.DrawImage(image, 0, 0);
        //    }

        //    Rectangle rect = new Rectangle(0, 0, image.Width, image.Height);

        //    BitmapData srcData = image.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
        //    BitmapData maskData = mask.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
        //    BitmapData outData = pixelated.LockBits(rect, ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);

        //    int srcStride = srcData.Stride;
        //    int maskStride = maskData.Stride;
        //    int outStride = outData.Stride;

        //    unsafe
        //    {
        //        byte* srcPtr = (byte*)srcData.Scan0.ToPointer();
        //        byte* maskPtr = (byte*)maskData.Scan0.ToPointer();
        //        byte* outPtr = (byte*)outData.Scan0.ToPointer();

        //        for (int y = 0; y < image.Height; y += pixelateSize)
        //        {
        //            for (int x = 0; x < image.Width; x += pixelateSize)
        //            {
        //                // Check alpha of mask at top-left corner of block
        //                byte* topLeftMask = maskPtr + y * maskStride + x * 4;
        //                if (topLeftMask[3] < 128) continue;

        //                int blockWidth = Math.Min(pixelateSize, image.Width - x);
        //                int blockHeight = Math.Min(pixelateSize, image.Height - y);

        //                long r = 0, g = 0, b = 0;
        //                int count = 0;

        //                for (int yy = y; yy < y + blockHeight; yy++)
        //                {
        //                    for (int xx = x; xx < x + blockWidth; xx++)
        //                    {
        //                        byte* maskPixel = maskPtr + yy * maskStride + xx * 4;
        //                        if (maskPixel[3] < 128) continue;

        //                        byte* srcPixel = srcPtr + yy * srcStride + xx * 4;
        //                        if (srcPixel[3] < 10) continue; // Skip mostly transparent

        //                        r += srcPixel[2];
        //                        g += srcPixel[1];
        //                        b += srcPixel[0];
        //                        count++;
        //                    }
        //                }

        //                if (count == 0) continue;

        //                byte avgR = (byte)(r / count);
        //                byte avgG = (byte)(g / count);
        //                byte avgB = (byte)(b / count);

        //                for (int yy = y; yy < y + blockHeight; yy++)
        //                {
        //                    for (int xx = x; xx < x + blockWidth; xx++)
        //                    {
        //                        byte* maskPixel = maskPtr + yy * maskStride + xx * 4;
        //                        if (maskPixel[3] < 128) continue;

        //                        byte* outPixel = outPtr + yy * outStride + xx * 4;

        //                        outPixel[0] = avgB;
        //                        outPixel[1] = avgG;
        //                        outPixel[2] = avgR;
        //                        // Preserve original alpha
        //                    }
        //                }
        //            }
        //        }
        //    }

        //    image.UnlockBits(srcData);
        //    mask.UnlockBits(maskData);
        //    pixelated.UnlockBits(outData);

        //    return pixelated;
        //}
    }

    //old version?
    //public Bitmap ApplyLineJitter(Bitmap image, Bitmap mask, int maxJitter)
    //{
    //    Random rand = new Random();

    //    // Clone the image to preserve the original
    //    Bitmap jitteredImage = (Bitmap)image.Clone();
    //    Rectangle rect = new Rectangle(0, 0, image.Width, image.Height);
    //    BitmapData imgData = jitteredImage.LockBits(rect, ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
    //    BitmapData maskData = mask.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

    //    int imgStride = imgData.Stride;
    //    int maskStride = maskData.Stride;

    //    unsafe
    //    {
    //        byte* imgPtr = (byte*)imgData.Scan0.ToPointer();
    //        byte* maskPtr = (byte*)maskData.Scan0.ToPointer();

    //        // Loop through each row (y-coordinate)
    //        for (int y = 0; y < image.Height; y++)
    //        {
    //            // Check if the mask allows jittering for this row
    //            byte* maskPixel = maskPtr + y * maskStride;
    //            bool isRowVisible = false;

    //            // Check if any pixel in this row is non-transparent
    //            for (int x = 0; x < image.Width; x++)
    //            {
    //                if (maskPixel[3] >= 128)  // Check alpha channel for transparency
    //                {
    //                    isRowVisible = true;
    //                    break;  // Stop checking if a visible pixel is found
    //                }
    //                maskPixel += 4;  // Move to the next pixel in the row (RGBA)
    //            }

    //            if (isRowVisible)
    //            {
    //                // Apply jitter to this row if it is visible
    //                int offset = rand.Next(-maxJitter, maxJitter);

    //                for (int x = 0; x < image.Width; x++)
    //                {
    //                    byte* pixel = imgPtr + y * imgStride + x * 3;

    //                    // Apply jitter only to non-transparent areas in the mask
    //                    byte* maskPixelInRow = maskPtr + y * maskStride + x * 4;
    //                    if (maskPixelInRow[3] >= 128)  // Non-transparent pixel
    //                    {
    //                        int newX = Math.Min(Math.Max(x + offset, 0), image.Width - 1); // Ensure we don't go out of bounds
    //                        byte* jitteredPixel = imgPtr + y * imgStride + newX * 3;

    //                        // Swap pixels with jittered offset
    //                        pixel[0] = jitteredPixel[0];  // Blue
    //                        pixel[1] = jitteredPixel[1];  // Green
    //                        pixel[2] = jitteredPixel[2];  // Red
    //                    }
    //                }
    //            }
    //        }
    //    }

    //    jitteredImage.UnlockBits(imgData);
    //    mask.UnlockBits(maskData);
    //    return jitteredImage;
    //}
}

/**
The MIT License (MIT)

Copyright (c) 2012 Sagar Mohite

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. 
 * IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Threading;
using System.Diagnostics;

using System.Drawing;
using System.Drawing.Imaging;

using AForge;
using AForge.Imaging;
using AForge.Imaging.Filters;

using Compatibility;

namespace Block_Matching
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        public Bitmap img1 = new Bitmap(@"C:\Users\Sagar\Documents\Sag\Phase1\Block Matching\Block Matching\Images\sample (3).bmp", true);
        public Bitmap img2 = new Bitmap(@"C:\Users\Sagar\Documents\Sag\Phase1\Block Matching\Block Matching\Images\sample (4).bmp", true);
        public BitmapImage bi;
        public Bitmap grayImage;

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            bi = Compatibility.Compatibility.BitmapToBitmapImage(img1);
            image1.Source = bi;
            
        }

        private void grayscaleButton_Click(object sender, RoutedEventArgs e)
        {
            Bitmap temp = Compatibility.Compatibility.BitmapImageToBitmap((BitmapImage)image1.Source.Clone());
            // create grayscale filter (BT709)
            Grayscale filter = new Grayscale(0.2125, 0.7154, 0.0721);
            // apply the filter
            grayImage = filter.Apply(temp);
            bi = Compatibility.Compatibility.BitmapToBitmapImage(grayImage);
            image1.Source = bi;
        }

        private void initiateButton_Click(object sender, RoutedEventArgs e)
        {
            // create grayscale filter (BT709)
            Grayscale filter = new Grayscale(0.2125, 0.7154, 0.0721);
            // apply the filter
            img1 = filter.Apply(img1);
            img2 = filter.Apply(img2);

            Stopwatch st = new Stopwatch();
            st.Start();

            // collect reference points using corners detector (for example)
            SusanCornersDetector scd = new SusanCornersDetector(30, 18);
            List<IntPoint> points = scd.ProcessImage(img1);

            // create block matching algorithm's instance
            ExhaustiveBlockMatching bm = new ExhaustiveBlockMatching(12, 36);
            // process images searching for block matchings
            List<BlockMatch> matches = bm.ProcessImage(img1, points, img2);

            st.Stop();
            TimeSpan elapsed = st.Elapsed;
            timedisp.Text = "Elapsed time = " + elapsed.ToString();

            // draw displacement vectors
            BitmapData data = img1.LockBits(
                new System.Drawing.Rectangle(0, 0, img1.Width, img1.Height),
                ImageLockMode.ReadWrite, img1.PixelFormat);

            foreach (BlockMatch match in matches)
            {
                // highlight the original point in source image
                AForge.Imaging.Drawing.FillRectangle(data,
                    new System.Drawing.Rectangle(match.SourcePoint.X - 1, match.SourcePoint.Y - 1, 3, 3),
                    System.Drawing.Color.Yellow);
                // draw line to the point in search image
                AForge.Imaging.Drawing.Line(data, match.SourcePoint, match.MatchPoint, System.Drawing.Color.Red);

                // check similarity
                if (match.Similarity > 0.98f)
                {
                    // process block with high similarity
                }
            }

            img1.UnlockBits(data);
            bi = Compatibility.Compatibility.BitmapToBitmapImage(img1);
            image1.Source = bi;
        }

        private void prevButton_Click(object sender, RoutedEventArgs e)
        {
            bi = Compatibility.Compatibility.BitmapToBitmapImage(img1);
            image1.Source = bi;
        }

        private void nextButton_Click(object sender, RoutedEventArgs e)
        {
            bi = Compatibility.Compatibility.BitmapToBitmapImage(img2);
            image1.Source = bi;
        }

    }
}

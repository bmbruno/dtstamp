using System;
using System.Collections.Generic;
using System.IO;
using ExifLibrary;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.Processing;

namespace DTStamp
{
    class DTStamp
    {
        static void Main(string[] args)
        {
            // TODO: Add pretty welcome graphic and set up formatting

            string workingDirectory = Directory.GetCurrentDirectory();
            string outputDirectory = $"{workingDirectory}/output/";
            string fontPath = "impact.ttf";
            int fontSize = 96;
            List<string> validFileTypes = new List<string>() { ".jpg", ".jpeg", ".png" };            
            List<string> imageFiles = new List<string>();
            FontCollection fontCollection = new FontCollection();
            FontFamily fontFamily;
            Font font = null;

            // Set working directory to another path if provided via arguments
            if (args.Length == 1)
            {
                workingDirectory = args[0].Trim();

                // Verify directory exists
                if (!Directory.Exists(workingDirectory))
                {
                    Console.WriteLine("Provided directory doesn't exist.");
                    Environment.Exit(-1);
                }
            }

            // Prepare output directory
            if (Directory.Exists(outputDirectory))
            {
                Directory.Delete(outputDirectory, true);
                Directory.CreateDirectory(outputDirectory);
            }

            // Prep fonts
            try
            {
                fontFamily = fontCollection.Install(fontPath);
                font = fontFamily.CreateFont(fontSize, FontStyle.Regular);
            }
            catch (Exception exc)
            {
                Console.WriteLine($"[EXCEPTION] Could not install font '{fontPath}'. {exc.Message.ToString()}");
            }

            // Get all files in working directory
            string[] allFiles = Directory.GetFiles(workingDirectory);

            // Filter for valid file types
            foreach (string file in allFiles)
            {
                string extension = Path.GetExtension(file).ToLower();

                if (validFileTypes.Contains(extension))
                    imageFiles.Add(file);
            }

            // Ensure images exist in directory
            if (imageFiles.Count == 0)
            {
                Console.WriteLine($"No image files found at path {workingDirectory}.");
                Console.WriteLine($"Supported image types: JPG, JPEG, PNG");
                Environment.Exit(-1);
            }

            // Iterate over each list and timestamp images
            foreach (string file in imageFiles)
            {
                try
                {
                    // Load EXIF data
                    ImageFile exifData = ImageFile.FromFile(file);
                    ExifDateTime dateTime = exifData.Properties.Get<ExifDateTime>(ExifTag.DateTimeOriginal);
                    Console.WriteLine($"[IMAGE] {Path.GetFileName(file)} [EXIF DATETIME] {dateTime.ToString()}");

                    string text = $" {dateTime.Value.ToString("yyyy-MM-dd")}  {dateTime.Value.ToShortTimeString()}";

                    using (Image image = Image.Load(path: file))
                    {
                        image.Mutate(x => x.DrawText(text, font, Brushes.Solid(Color.Yellow), Pens.Solid(Color.Black, 3), new PointF(20, 20)));
                        image.SaveAsJpeg(outputDirectory + Path.GetFileName(file));
                    }

                }
                catch (Exception exc)
                {
                    // TODO: add detail: iamge filename
                    Console.WriteLine($"EXCEPTION: {exc.Message.ToString()}");
                }
            }
        }
    }
}

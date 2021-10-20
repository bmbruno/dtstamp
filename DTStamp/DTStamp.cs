using System;
using System.Collections.Generic;
using System.IO;
using ExifLibrary;
using ParamParser;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.Processing;
using System.Reflection;

namespace DTStamp
{
    class DTStamp
    {
        static void Main(string[] args)
        {
            Console.WriteLine($"#############################");
            Console.WriteLine($"     Welcome to DTStamp!");
            Console.WriteLine($"#############################");
            Console.WriteLine($"Version: {Assembly.GetEntryAssembly().GetName().Version}");
            Console.Write(Environment.NewLine);
            
            string workingDirectory = Directory.GetCurrentDirectory();
            string outputDirectory = string.Empty;
            string fontPath = "dtstamp.ttf";
            int fontSize = 96;
            List<string> validFileTypes = new List<string>() { ".jpg", ".jpeg", ".png" };            
            List<string> imageFiles = new List<string>();
            FontCollection fontCollection = new FontCollection();
            FontFamily fontFamily;
            Font font = null;
            Parser parser = new Parser(args);

            // Handle program parameters
            if (parser.HasParam("path"))
            {
                workingDirectory = parser.GetParam("path").Trim();

                // Verify directory exists
                if (!Directory.Exists(workingDirectory))
                {
                    Console.WriteLine("Provided directory doesn't exist.");
                    Environment.Exit(-1);
                }
            }

            if (parser.HasParam("size"))
            {
                if(!Int32.TryParse(parser.GetParam("size").Trim(), out fontSize))
                {
                    Console.WriteLine($"Could not parse 'size' param; defaulting fontSize to {fontSize}.");
                }
            }

            if (parser.Parameters.Count == 0)
            {
                Console.WriteLine("Available parameters:");
                Console.WriteLine(" -path : Absolute path to a folder containing images to be stamped; default is current directory of the executable.");
                Console.WriteLine($" -size : font size (pt) of datetime stamp on images; default is {fontSize}.");
                Console.Write(Environment.NewLine);
            }

            // Prepare output directory
            outputDirectory = $"{workingDirectory}\\output\\";

            if (Directory.Exists(outputDirectory))
                Directory.Delete(outputDirectory, true);

            Directory.CreateDirectory(outputDirectory);

            // Output information about this execition
            Console.WriteLine($"PATH:      {workingDirectory}");
            Console.WriteLine($"OUTPUT:    {outputDirectory}");
            Console.WriteLine($"FONT SIZE: {fontSize}");
            Console.Write(Environment.NewLine);

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
                string fileName = Path.GetFileName(file);

                try
                {
                    // Load EXIF data
                    ImageFile exifData = ImageFile.FromFile(file);
                    ExifDateTime dateTime = exifData.Properties.Get<ExifDateTime>(ExifTag.DateTimeOriginal);
                    Console.WriteLine($"[IMAGE] {fileName} [EXIF DATETIME] {dateTime.ToString()}");

                    string text = $"{dateTime.Value.ToString("yyyy-MM-dd")}  {dateTime.Value.ToShortTimeString()}";

                    using (Image image = Image.Load(path: file))
                    {
                        image.Mutate(x => x.DrawText(text, font, Brushes.Solid(Color.Yellow), Pens.Solid(Color.Black, (fontSize / 20)), new PointF(20, 20)));
                        image.SaveAsJpeg(outputDirectory + Path.GetFileName(file));
                    }

                }
                catch (Exception exc)
                {                    
                    Console.WriteLine($"EXCEPTION processing {fileName}: {exc.Message.ToString()}");
                }
            }
        }
    }
}

# About

Version: 1.0.1

DTStamp is a .NET 5 CLI utility for adding a date/time stamp to images in a single folder. Supported image types include JPG, JPEG, PNG.

This application is delivered as a single-file .NET 5 application. The `dtstamp.ttf` font file is required for the application to run and can be used to customized with the font of your choice (see **Configuration** below).

# Usage Instructions

There are two ways to use DTStamp:

## Option 1: place executable in images folder

1) Copy & paste the following files into your images folder:
    * dtstamp.exe
    * dtstamp.ttf
2) Run `dtstamp.exe`
3) Timestamped images will be available in a new folder named "output" inside the source folder

## Option 2: provide path to images folder

1) Run `dtstamp.exe` from any location with the following parameter: `-path <path_to_your_image_folder>`
    * Example: `dtstamp.exe -path 'C:\data\images'`
2) Timestamped images will be available in a new folder named "output" inside the source folder

# Configuration

## Parameters

Available parameters when running `dtstamp.exe`:

 * `-path`: absolute path to the image source folder; defaults to current directory of executable if not provided
 * `-size`: font size (pt) of date & time stamp on images
 * `-quality` : jpeg encoding quality (0 - 100)

Example of parameter usage:

```
dtstamp.exe -path 'c:\images' -size 128 -quality 55
```

## Custom fonts

The font file `dtstamp.ttf` represents the font used for timestamps on images. You can replace this with a font of your choice by naming a True Type Font file `dtstamp.ttf` and placing next to the executable (`dtstamp.exe`).

# Building

To publish as a single-file application using the .NET 5 CLI:

```
dotnet publish -r win-x64 -p:PublishSingleFile=true --self-contained false
```

# License

MIT license. See LICENSE file for additional detail.

﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Cairo;
//using Svg;

namespace IMGUI
{
    public partial class Texture
    {
        internal ImageSurface _surface;

        public int Width
        {
            get { return _surface.Width; }
        }
        
        public int Height
        {
            get { return _surface.Height; }
        }

        internal static Dictionary<string, Texture> _presets;

        public Texture(string filePath)
        {
            bool isPngFile = false;
            bool isSvgFile = false;

            //check if the file is a png or svg
            using (var fileStream = File.Open(filePath, FileMode.Open))
            using (var binaryReader = new BinaryReader(fileStream))
            {
                var headEightBytes = binaryReader.ReadBytes(8);
                isPngFile = headEightBytes.SequenceEqual(Utility.PngHeaderEightBytes);
            }
            if(isPngFile)
            {
                _surface = new ImageSurface(filePath);
            }
            else
            {
                //check if the file is a png or svg
                using (var fileStream = File.Open(filePath, FileMode.Open))
                {
                    using (var streamReader = new StreamReader(fileStream))
                    {
                        var firstLineText = streamReader.ReadLine();
                        if (firstLineText != null)
                        {
                            isSvgFile = firstLineText.StartsWith(Utility.SvgFileFirstLineTextPrefix);
                        }
                    }
                }

                //TODO Implement svg rendering (try to use nanosvg)
                if (isSvgFile)
                {
                    throw new NotImplementedException("SVG is not supported");
                    //using (var fileStream = File.Open(filePath, FileMode.Open))
                    //{
                    //    //convert svg file to png file in memory
                    //    var svgDoc = SvgDocument.Open<SvgDocument>(fileStream);
                    //    using (var stream = new MemoryStream())
                    //    {
                    //        using (var bitmap = svgDoc.Draw())
                    //        {
                    //            bitmap.Save("K:\\tmp\\t.png", ImageFormat.Png);
                    //        }
                    //        //_surface = CairoStreamReader.ImageSurfaceFromPng(stream);
                    //        _surface = new ImageSurface("K:\\tmp\\t.png");
                    //    }
                    //}
                }
            }

            if(!isSvgFile && !isPngFile)
            {
                throw new ArgumentException("Specified file is not a png file or a svg file");
            }
        }

        public Texture(byte[] data)
        {
            bool isPngFile = false;
            bool isSvgFile = false;

            //check if the file is a png or svgdata
            byte[] headEightBytes = new byte[8];
            Array.Copy(data, headEightBytes, 8);
            isPngFile = headEightBytes.SequenceEqual(Utility.PngHeaderEightBytes);
            if (isPngFile)
            {
                _surface = new ImageSurface(data);
            }
            else
            {
                throw new NotImplementedException("SVG is not supported");
            }

            if (!isSvgFile && !isPngFile)
            {
                throw new ArgumentException("Specified file is not a png file or a svg file");
            }
        }

        private Texture(ImageSurface imageSurface)
        {
            if (imageSurface == null)
            {
                throw new Exception("Texture creating failed");
            }

            _surface = imageSurface;
        }
        
        ~Texture()
        {
            _surface.Dispose();
        }
            
    }
}
﻿using Microsoft.Graphics.Canvas;
using Retouch_Photo.Models.Layers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Streams;

namespace Retouch_Photo.Models
{
    /// <summary>项目</summary>
    public class Project
    {
       public string Name;

        public int Width;
        public int Height;

        public int Tool;
        public int Index;

        public IEnumerable<Layer> Layers;


        public static Project CreateFromXDocument(ICanvasResourceCreator creator, XDocument xdoc)
        {
            return new Project()
            {
                Width = (int)xdoc.Descendants("Width").Single(),
                Height = (int)xdoc.Descendants("Height").Single(),

                Tool = (int)xdoc.Descendants("Tool").Single(),
                Index = (int)xdoc.Descendants("Index").Single(),

                Layers = from element in xdoc.Descendants("Layer") select Layer.CreateFromXElement(creator, element)
            };
        }
        
        public static Project CreateFromSize(ICanvasResourceCreator creator, BitmapSize pixels)
        {
            int width = (int)pixels.Width;
            int height = (int)pixels.Height;

            return new Project()
            {
                Width = width,
                Height = height,
                Layers = new List<Layer>()
            };
        }

        public static async Task<Project> CreateFromFileAsync(ICanvasResourceCreator creator, StorageFile file)
        {
            using (IRandomAccessStream stream = await file.OpenReadAsync())
            {
                try
                {
                    CanvasBitmap bitmap = await CanvasBitmap.LoadAsync(creator, stream, 96);

                    int width = (int)bitmap.SizeInPixels.Width;
                    int height = (int)bitmap.SizeInPixels.Height;

                    return new Project()
                    {
                        Width = width,
                        Height = height,
                        Layers = new List<Layer>()
                        {
                            ImageLayer.CreateFromBitmap(creator,bitmap,width,height)
                        }
                    };
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }

    }
}

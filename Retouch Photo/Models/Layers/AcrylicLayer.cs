﻿using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using Retouch_Photo.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Graphics.Effects;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace Retouch_Photo.Models.Layers
{
    public class AcrylicLayer : Layer
    {
         
        public static string Type = "Acrylic";
        protected AcrylicLayer() => base.Name = AcrylicLayer.Type;

        public float TintOpacity = 0.5f;
        public Color TintColor = Color.FromArgb(255, 255, 255, 255);
        public float BlurAmount = 12.0f;

        protected override ICanvasImage GetRender(ICanvasResourceCreator creator, IGraphicsEffectSource image, Matrix3x2 canvasToVirtualMatrix)
        {
            Matrix3x2 matrix = this.Transformer.Matrix * canvasToVirtualMatrix;
            Vector2 leftTop = this.Transformer.TransformLeftTop(matrix);
            Vector2 rightBottom = this.Transformer.TransformRightBottom(matrix);

            return new CropEffect
            {
                SourceRectangle = new Rect(leftTop.ToPoint(), rightBottom.ToPoint()),
                Source = new CompositeEffect
                {
                    Sources =
                    {
                        new GaussianBlurEffect
                        {
                             BlurAmount = this.BlurAmount,
                             Source = image
                        },
                        new OpacityEffect
                        {
                            Opacity = this.TintOpacity,
                            Source = new ColorSourceEffect
                            {
                                Color = this.TintColor
                             }
                         }
                    }
                }
            };
        }
        public override void ThumbnailDraw(ICanvasResourceCreator creator, CanvasDrawingSession ds, Size controlSize)
        {
            Rect rect = Layer.GetThumbnailSize(base.Transformer.Width, base.Transformer.Height, controlSize);

            ds.FillRectangle(rect, this.TintColor);
        }



        public static AcrylicLayer CreateFromRect(ICanvasResourceCreator creator, VectRect rect, Color color, float opacity = 0.5f)
        {
            return new AcrylicLayer
            {
                Transformer = Transformer.CreateFromRect(rect, disabledRadian: true),
                TintColor = color,
                TintOpacity = opacity
            };
        }

    }
}

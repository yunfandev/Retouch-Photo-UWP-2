﻿using FanKit.Transformers;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Brushes;
using Retouch_Photo2.Elements;
using System.Numerics;
using System.Xml.Linq;
using Windows.UI;

namespace Retouch_Photo2.Brushs.Models
{
    /// <summary>
    /// <see cref="IBrush"/>'s ColorBrush.
    /// </summary>
    public class ColorBrush : IBrush
    {
        //@Content
        public BrushType Type => BrushType.Color;

        public CanvasGradientStop[] Array { get => null; set { } }
        public Color Color { get; set; }
        public Transformer Destination { set { } }
        public Photocopier Photocopier { get => new Photocopier(); }
        public CanvasEdgeBehavior Extend { get => CanvasEdgeBehavior.Clamp; set { } }


        //@Construct
        /// <summary>
        /// Initializes a ColorBrush.
        /// </summary>
        public ColorBrush() { }
        /// <summary>
        /// Initializes a ColorBrush.
        /// </summary>
        /// <param name="color"> The color. </param>
        public ColorBrush(Color color) => this.Color = color;


        public ICanvasBrush GetICanvasBrush(ICanvasResourceCreator resourceCreator)
        {
            return new CanvasSolidColorBrush(resourceCreator, this.Color);
        }
        public ICanvasBrush GetICanvasBrush(ICanvasResourceCreator resourceCreator, Matrix3x2 matrix)
        {
            return new CanvasSolidColorBrush(resourceCreator, this.Color);
        }


        public BrushOperateMode ContainsOperateMode(Vector2 point, Matrix3x2 matrix)
        {
            return BrushOperateMode.None;
        }
        public void Controller(BrushOperateMode mode, Vector2 startingPoint, Vector2 point) { }

        public void Draw(CanvasDrawingSession drawingSession, Matrix3x2 matrix, Color accentColor) { }


        public IBrush Clone()
        {
            return new ColorBrush(this.Color);
        }

        public void SaveWith(XElement element)
        {
            element.Add(FanKit.Transformers.XML.SaveColor("Color", this.Color));
        }
        public void Load(XElement element)
        {
            if (element.Element("Color") is XElement color) this.Color = FanKit.Transformers.XML.LoadColor(color);
        }


        //@Interface
        public void CacheTransform() { }
        public void TransformMultiplies(Matrix3x2 matrix) { }
        public void TransformAdd(Vector2 vector) { }

    }
}
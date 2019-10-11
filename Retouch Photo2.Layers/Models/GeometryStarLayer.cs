﻿using FanKit.Transformers;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Geometry;
using Retouch_Photo2.Layers.Icons;
using System.Numerics;

namespace Retouch_Photo2.Layers.Models
{
    /// <summary>
    /// <see cref="IGeometryLayer"/>'s GeometryStarLayer .
    /// </summary>
    public class GeometryStarLayer : IGeometryLayer
    {
        //@Static
        public int Points = 5;
        public float InnerRadius = 0.38f;

        //@Construct
        public GeometryStarLayer()
        {
            base.Control.Icon = new GeometryStarIcon();
            base.Control.Text = "Star";
        }

        //@Override       
        public override string Type => "Star";

        public override CanvasGeometry CreateGeometry(ICanvasResourceCreator resourceCreator, Matrix3x2 canvasToVirtualMatrix)
        {
            Matrix3x2 oneMatrix = Transformer.FindHomography(GeometryUtil.OneTransformer, base.TransformManager.Destination);
            Matrix3x2 matrix = oneMatrix * canvasToVirtualMatrix;

            float rotation = GeometryUtil.StartingRotation;
            float angle = FanKit.Math.Pi / this.Points;

            Vector2[] points = new Vector2[this.Points * 2];
            for (int i = 0; i < this.Points; i++)
            {
                int index = i * 2;

                //Outer
                Vector2 outer = GeometryUtil.GetRotationVector(rotation);
                points[index] = Vector2.Transform(outer, matrix);
                rotation += angle;

                //Inner
                Vector2 inner = GeometryUtil.GetRotationVector(rotation);
                points[index + 1] = Vector2.Transform(inner * this.InnerRadius, matrix);
                rotation += angle;
            }

            return CanvasGeometry.CreatePolygon(resourceCreator, points);
        }

        public override ILayer Clone(ICanvasResourceCreator resourceCreator)
        {
            GeometryStarLayer StarLayer = new GeometryStarLayer
            {
                FillBrush = base.FillBrush,
                StrokeBrush = base.StrokeBrush,

                Points=this.Points,
                InnerRadius= this.InnerRadius,
            };

            LayerBase.CopyWith(resourceCreator, StarLayer, this);
            return StarLayer;
        }
    }
}
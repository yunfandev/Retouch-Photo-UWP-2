﻿using FanKit.Transformers;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Geometry;
using Retouch_Photo2.Layers.Icons;
using System.Collections.Generic;
using System.Numerics;
using Windows.ApplicationModel.Resources;

namespace Retouch_Photo2.Layers.Models
{
    /// <summary>
    /// <see cref="ILayer"/>'s GeometryEllipseLayer .
    /// </summary>
    public class GeometryEllipseLayer : LayerBase, ILayer
    {

        //@Override     
        public override LayerType Type => LayerType.GeometryEllipse;

        //@Construct
        /// <summary>
        /// Construct a ellipse-layer.
        /// </summary>
        /// <param name="element"> The source XElement. </param>
        public GeometryEllipseLayer()
        {
            base.Control = new LayerControl(this)
            {
                Icon = new GeometryEllipseIcon(),
                Text = this.ConstructStrings(),
            };
        }               


        public override ILayer Clone(ICanvasResourceCreator resourceCreator)
        {
            GeometryEllipseLayer ellipseLayer = new GeometryEllipseLayer();

            LayerBase.CopyWith(resourceCreator, ellipseLayer, this);
            return ellipseLayer;
        }


        public override CanvasGeometry CreateGeometry(ICanvasResourceCreator resourceCreator, Matrix3x2 canvasToVirtualMatrix)
        {
            Transformer transformer = base.TransformManager.Destination;

            return transformer.ToEllipse(resourceCreator, canvasToVirtualMatrix);
        }
        public override IEnumerable<IEnumerable<Node>> ConvertToCurves()
        {
            Transformer transformer = base.TransformManager.Destination;

            return TransformerGeometry.ConvertToCurvesFromEllipse(transformer);
        }


        //Strings
        private string ConstructStrings()
        {
            ResourceLoader resource = ResourceLoader.GetForCurrentView();

            return resource.GetString("/Layers/GeometryEllipse");
        }

    }
}
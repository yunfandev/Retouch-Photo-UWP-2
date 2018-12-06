﻿using Microsoft.Graphics.Canvas;
using Retouch_Photo.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI;

namespace Retouch_Photo.ViewModels.ToolViewModels
{
    public class ToolRectangularMarqueeViewModel : ToolViewModel
    {
        public override void Start(Vector2 point, DrawViewModel viewModel)
        {
            viewModel.MarqueeTool.Tool = MarqueeToolType.Rectangular;

            Vector2 v = viewModel.Transformer.InversionTransform(point);
            viewModel.MarqueeTool.Operator_Start(v);
        }
        public override void Delta(Vector2 point, DrawViewModel viewModel)
        {
            Vector2 v = viewModel.Transformer.InversionTransform(point);
            viewModel.MarqueeTool.Operator_Delta(v);
        }
        public override void Complete(Vector2 point, DrawViewModel viewModel)
        {
            Vector2 v = viewModel.Transformer.InversionTransform(point);
            viewModel.MarqueeTool.Operator_Complete(v);
        }



        public override void Render(CanvasDrawingSession ds, DrawViewModel viewModel)
        {
        }

    }
}



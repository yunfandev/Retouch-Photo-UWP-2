﻿using Microsoft.Graphics.Canvas;
using Retouch_Photo2.Tools.Controls;
using Retouch_Photo2.Tools.Pages;
using Retouch_Photo2.ViewModels;
using System.Numerics;

namespace Retouch_Photo2.Tools.Models
{
    public class PencilTool : Tool
    {
        //ViewModel
        DrawViewModel ViewModel => Retouch_Photo2.App.ViewModel;
        
        public PencilTool()
        {
            base.Type = ToolType.Pencil;
            base.Icon = new PencilControl();
            base.WorkIcon = new PencilControl();
            base.Page = new PencilPage();
        }

        public override void Start(Vector2 point) { }
        public override void Delta(Vector2 point) { }
        public override void Complete(Vector2 point) { }

        public override void Draw(CanvasDrawingSession ds) { }
    }
}
﻿using Microsoft.Graphics.Canvas;
using Retouch_Photo.Models;
using Retouch_Photo.Models.Layers;
using Retouch_Photo.Tools.Controls;
using Retouch_Photo.Tools.Pages;
using Retouch_Photo.ViewModels;
using System.Numerics;
using static Retouch_Photo.Library.HomographyController;

namespace Retouch_Photo.Tools.Models
{
    public class AcrylicTool : Tool
    {
        //ViewModel
        DrawViewModel ViewModel => Retouch_Photo.App.ViewModel;
        

        Vector2 point;
        Vector2 StartPoint;

        AcrylicLayer Layer;


        public AcrylicTool()
        {
            base.Type = ToolType.Acrylic;
            base.Icon = new AcrylicControl();
            base.WorkIcon = new AcrylicControl();
            base.Page = new AcrylicPage();
        }


        //@Override
        public override void ToolOnNavigatedTo()//当前页面成为活动页面
        {
        }
        public override void ToolOnNavigatedFrom()//当前页面不再成为活动页面
        {
        }


        public override void Start(Vector2 point)
        {
            this.point = point;
            this.StartPoint = Vector2.Transform(point, this.ViewModel.MatrixTransformer.InverseMatrix);
            VectRect rect = new VectRect(this.StartPoint, this.StartPoint, this.ViewModel.MarqueeMode);

            this.Layer = AcrylicLayer.CreateFromRect(this.ViewModel.CanvasDevice, rect, this.ViewModel.Color);
            this.ViewModel.InvalidateWithJumpedQueueLayer(this.Layer);
        }
        public override void Delta(Vector2 point)
        {
            Vector2 endPoint = Vector2.Transform(point, this.ViewModel.MatrixTransformer.InverseMatrix);
            VectRect rect = new VectRect(this.StartPoint, endPoint, this.ViewModel.MarqueeMode);

            this.Layer.Transformer = Transformer.CreateFromSize(rect.Width, rect.Height, new Vector2(rect.X, rect.Y));
            this.ViewModel.InvalidateWithJumpedQueueLayer(this.Layer);
        }
        public override void Complete(Vector2 point)
        {
            Vector2 endPoint = Vector2.Transform(point, this.ViewModel.MatrixTransformer.InverseMatrix);
            VectRect rect = new VectRect(this.StartPoint, endPoint, this.ViewModel.MarqueeMode);

            if (Transformer.OutNodeDistance(this.point, point))
            {
                AcrylicLayer layer = AcrylicLayer.CreateFromRect(this.ViewModel.CanvasDevice, rect, this.ViewModel.Color);
                this.ViewModel.RenderLayer.Insert(layer);
                this.ViewModel.CurrentLayer = layer;
            }

            this.Layer = null;
            this.ViewModel.Invalidate();
        }


        public override void Draw(CanvasDrawingSession ds)
        {
            if (this.Layer == null) return;
            Transformer.DrawBound(ds, this.Layer.Transformer, this.ViewModel.MatrixTransformer.Matrix);
        }

    }
}

﻿using FanKit.Transformers;
using Microsoft.Graphics.Canvas;
using Retouch_Photo2.Tools.Controls;
using Retouch_Photo2.Tools.Pages;
using Retouch_Photo2.ViewModels;
using Retouch_Photo2.ViewModels.Keyboards;
using Retouch_Photo2.ViewModels.Selections;
using Retouch_Photo2.ViewModels.Tips;
using System.Numerics;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Retouch_Photo2.Tools.Models
{
    /// <summary>
    /// <see cref="ITool"/>'s CursorTool.
    /// </summary>
    public partial class CursorTool : ITool
    {
        //@ViewModel
        ViewModel ViewModel => App.ViewModel;
        SelectionViewModel SelectionViewModel => App.SelectionViewModel;
        KeyboardViewModel KeyboardViewModel => App.KeyboardViewModel;
        MezzanineViewModel MezzanineViewModel => App.MezzanineViewModel;
        TipViewModel TipViewModel => App.TipViewModel;

        //Box
        bool _isBox;
        TransformerRect _boxCanvasRect;
        
        public ToolType Type=> ToolType.Cursor;
        public FrameworkElement Icon { get; }= new CursorControl();
        public FrameworkElement ShowIcon { get; }= new CursorControl();
        public Page Page => this._cursorPage;
        CursorPage _cursorPage { get; } = new CursorPage();

        public void Starting(Vector2 point)
        {
            this._isBox = false; //Box

            if (this.TipViewModel.TransformerTool.Starting(point)) return; //TransformerToolBase

            this._isBox = true; //Box
        }
        public void Started(Vector2 startingPoint, Vector2 point)
        {
            //Box
            if (this._isBox)
            {
                this.BoxDelta(startingPoint, point);//Box
                this.ViewModel.Invalidate(InvalidateMode.Thumbnail);//Invalidate
                return;
            }

            this.TipViewModel.TransformerTool.Started(startingPoint, point, false);//TransformerToolBase
        }
        public void Delta(Vector2 startingPoint, Vector2 point)
        {
            //Box
            if (this._isBox)
            {
                this.BoxDelta(startingPoint, point);//Box
                this.ViewModel.Invalidate();//Invalidate
                return;
            }

            this.TipViewModel.TransformerTool.Delta(startingPoint, point); //TransformerToolBase
        }
        public void Complete(Vector2 startingPoint, Vector2 point, bool isSingleStarted)
        {
            //Box
            if (this._isBox)
            {
                this._isBox = false;

                if (isSingleStarted)
                {
                    this.BoxComplete();//Box
                    this.SelectionViewModel.SetMode(this.ViewModel.Layers);//Selection
                    this.ViewModel.Invalidate(InvalidateMode.HD);//Invalidate
                    return;
                }
            }

            this.TipViewModel.TransformerTool.Complete(startingPoint, point, isSingleStarted); //TransformerToolBase
        }
        
        public void Draw(CanvasDrawingSession drawingSession)
        {
            //Box
            if (this._isBox)
            {
                this.BoxDraw(drawingSession);//Box
                return;
            }

            this.TipViewModel.TransformerTool.Draw(drawingSession);//TransformerToolBase
        }
        
        public void OnNavigatedTo() { }
        public void OnNavigatedFrom() { }
    }
}
﻿using FanKit.Transformers;
using Microsoft.Graphics.Canvas;
using Retouch_Photo2.Brushs;
using Retouch_Photo2.Layers;
using Retouch_Photo2.Tools.Icons;
using Retouch_Photo2.ViewModels;
using System.Numerics;
using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Retouch_Photo2.Tools.Models
{
    /// <summary>
    /// <see cref="ITool"/>'s TransparencyTool.
    /// </summary>
    public partial class TransparencyTool : ITool
    {

        //@ViewModel
        ViewModel ViewModel => App.ViewModel;
        ViewModel SelectionViewModel => App.SelectionViewModel;
        ViewModel MethodViewModel => App.MethodViewModel;
        SettingViewModel SettingViewModel => App.SettingViewModel;

        ListViewSelectionMode Mode => this.SelectionViewModel.SelectionMode;

        VectorBorderSnap Snap => this.ViewModel.VectorBorderSnap;
        bool IsSnap => this.SettingViewModel.IsSnap;


        //@Content
        public ToolType Type => ToolType.Transparency;
        public FrameworkElement Icon { get; } = new TransparencyIcon();
        public IToolButton Button { get; } = new ToolButton
        {
            CenterContent = new TransparencyIcon()
        };
        public FrameworkElement Page => this.TransparencyPage;
        TransparencyPage TransparencyPage = new TransparencyPage();


        //@Construct
        /// <summary>
        /// Initializes a TransparencyTool. 
        /// </summary>
        public TransparencyTool()
        {
            this.ConstructStrings();
        }


        BrushHandleMode HandleMode = BrushHandleMode.None;

        public void Started(Vector2 startingPoint, Vector2 point)
        {
            //Selection
            if (this.Mode == ListViewSelectionMode.None) return;

            //Snap
            if (this.IsSnap) this.ViewModel.VectorBorderSnapInitiate(this.SelectionViewModel.Transformer);

            this.TransparencyStarted(startingPoint, point);

            this.ViewModel.Invalidate(InvalidateMode.Thumbnail);//Invalidate
        }
        public void Delta(Vector2 startingPoint, Vector2 point)
        {
            //Selection
            if (this.Mode == ListViewSelectionMode.None) return;

            Matrix3x2 inverseMatrix = this.ViewModel.CanvasTransformer.GetInverseMatrix();
            Vector2 canvasStartingPoint = Vector2.Transform(startingPoint, inverseMatrix);
            Vector2 canvasPoint = Vector2.Transform(point, inverseMatrix);

            //Snap
            if (this.IsSnap) canvasPoint = this.Snap.Snap(canvasPoint);

            this.TransparencyDelta(canvasStartingPoint, canvasPoint);

            this.ViewModel.Invalidate();//Invalidate
        }
        public void Complete(Vector2 startingPoint, Vector2 point, bool isOutNodeDistance)
        {
            //Selection
            if (this.Mode == ListViewSelectionMode.None) return;

            Matrix3x2 inverseMatrix = this.ViewModel.CanvasTransformer.GetInverseMatrix();
            Vector2 canvasStartingPoint = Vector2.Transform(startingPoint, inverseMatrix);
            Vector2 canvasPoint = Vector2.Transform(point, inverseMatrix);

            //Snap
            if (this.IsSnap)
            {
                canvasPoint = this.Snap.Snap(canvasPoint);
                this.Snap.Default();
            }

            this.TransparencyComplete(canvasStartingPoint, canvasPoint);

            this.HandleMode = BrushHandleMode.None;
            this.ViewModel.Invalidate(InvalidateMode.HD);//Invalidate
        }
        public void Clicke(Vector2 point) => ToolBase.MoveTool.Clicke(point);

        public void Draw(CanvasDrawingSession drawingSession)
        {
            Matrix3x2 matrix = this.ViewModel.CanvasTransformer.GetMatrix();

            //@DrawBound
            switch (this.SelectionViewModel.SelectionMode)
            {
                case ListViewSelectionMode.None:
                    break;
                case ListViewSelectionMode.Single:
                    ILayer layer2 = this.SelectionViewModel.SelectionLayerage.Self;
                    drawingSession.DrawLayerBound(layer2, matrix, this.ViewModel.AccentColor);
                    break;
                case ListViewSelectionMode.Multiple:
                    foreach (Layerage layerage in this.ViewModel.SelectionLayerages)
                    {
                        ILayer layer = layerage.Self;
                        drawingSession.DrawLayerBound(layer, matrix, this.ViewModel.AccentColor);
                    }
                    break;
            }


            switch (this.SelectionViewModel.SelectionMode)
            {
                case ListViewSelectionMode.None:
                    break;
                case ListViewSelectionMode.Single:
                case ListViewSelectionMode.Multiple:
                    {
                        //Snapping
                        if (this.IsSnap) this.Snap.Draw(drawingSession, matrix);

                        this.Transparency.Draw(drawingSession, matrix, this.ViewModel.AccentColor);
                    }
                    break;
            }
        }


        public void OnNavigatedTo()
        {
            Layerage layerage = this.SelectionViewModel.GetFirstSelectedLayerage();
            if (layerage != null)
            {
                ILayer layer = layerage.Self;
                this.SelectionViewModel.SetStyle(layer.Style);
            }
        }
        public void OnNavigatedFrom() { }


        //Strings
        private void ConstructStrings()
        {
            ResourceLoader resource = ResourceLoader.GetForCurrentView();

            this.Button.Title = resource.GetString("/Tools/Transparency");
        }

    }


    /// <summary>
    /// Page of <see cref="TransparencyTool"/>.
    /// </summary>
    internal partial class TransparencyPage : Page
    {

        //@ViewModel
        ViewModel ViewModel => App.ViewModel;
        ViewModel SelectionViewModel => App.SelectionViewModel;
        ViewModel MethodViewModel => App.MethodViewModel;
        SettingViewModel SettingViewModel => App.SettingViewModel;

        ListViewSelectionMode Mode => this.SelectionViewModel.SelectionMode;


        //@Construct
        /// <summary>
        /// Initializes a TransparencyPage. 
        /// </summary>
        public TransparencyPage()
        {
            this.InitializeComponent();
            this.ConstructStrings();

            this.ConstructShowControl();

            //Type
            this.ConstructTransparencyType();
        }

    }

    /// <summary>
    /// Page of <see cref="TransparencyTool"/>.
    /// </summary>
    internal partial class TransparencyPage : Page
    {

        //Strings
        private void ConstructStrings()
        {
            ResourceLoader resource = ResourceLoader.GetForCurrentView();

            this.TypeTextBlock.Text = resource.GetString("/Tools/Brush_Type");
            this.ShowTextBlock.Text = resource.GetString("/Tools/Brush_Brush");
        }

        private void ConstructShowControl()
        {
            this.ShowControl.Tapped += (s, e) =>
            {
                this.TransparencyShow();
            };


            //@Focus
            // Before Flyout Showed, Don't let TextBox Got Focus.
            // After TextBox Gots focus, disable Shortcuts in SettingViewModel.
            if (this.StopsPicker.ColorPicker.HexPicker is TextBox textBox)
            {
                textBox.IsEnabled = false;
                this.StopsPicker.ColorFlyout.Opened += (s, e) => textBox.IsEnabled = true;
                this.StopsPicker.ColorFlyout.Closed += (s, e) => textBox.IsEnabled = false;
                textBox.GotFocus += (s, e) => this.SettingViewModel.KeyIsEnabled = false;
                textBox.LostFocus += (s, e) => this.SettingViewModel.KeyIsEnabled = true;
            }

            this.StopsPicker.StopsChanged += (s, array) =>
            {
                this.TransparencyStopsChanged(array);
            };

            this.StopsPicker.StopsChangeStarted += (s, array) =>
            {
                this.TransparencyStopsChangeStarted(array);
            };
            this.StopsPicker.StopsChangeDelta += (s, array) =>
            {
                this.TransparencyStopsChangeDelta(array);
            };
            this.StopsPicker.StopsChangeCompleted += (s, array) =>
            {
                this.TransparencyStopsChangeCompleted(array);
            };
        }

    }
}
﻿using FanKit.Transformers;
using Microsoft.Graphics.Canvas.Brushes;
using Retouch_Photo2.Brushs;
using Retouch_Photo2.Elements;
using Retouch_Photo2.Layers.Models;
using Retouch_Photo2.Tools.Models;
using Retouch_Photo2.ViewModels;
using System.Numerics;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Retouch_Photo2.Tools.Pages
{
    /// <summary>
    /// Page of <see cref = "BrushTool"/>.
    /// </summary>
    public sealed partial class BrushPage : Page, IToolPage
    {
        //@ViewModel
        ViewModel ViewModel => App.ViewModel;
        SelectionViewModel SelectionViewModel => App.SelectionViewModel;

        //@Content
        public FrameworkElement Self => this;
        public bool IsSelected { private get; set; }

        //@Converter
        private int FillOrStrokeToIndexConverter(FillOrStroke fillOrStroke) => (int)fillOrStroke;
        private int BrushTypeToIndexConverter(BrushType brushType) => (int)brushType;

        private bool IsOpenConverter(bool isOpen) => isOpen && this.IsSelected;

        bool _isOpened;

        //@Construct
        public BrushPage()
        {
            this.InitializeComponent();

            //FillOrStroke
            this.FillOrStrokeComboBox.SelectionChanged += (s, e) =>
            {
                FillOrStroke fillOrStroke = (FillOrStroke)this.FillOrStrokeComboBox.SelectedIndex;
                if (this.SelectionViewModel.FillOrStroke == fillOrStroke) return;

                this.SelectionViewModel.SetFillOrStroke(fillOrStroke);
                this.ViewModel.Invalidate();//Invalidate
            };


            //GradientBrushType
            this.StopsPicker.ComboBox.SetValue(ComboBox.SelectedIndexProperty, (int)GradientBrushType.Linear);//DependencyObject
            this.BrushTypeComboBox.SelectionChanged += (s, e) =>
            {
                BrushType brushType = (BrushType)this.BrushTypeComboBox.SelectedIndex;
                if (this.SelectionViewModel.BrushType == brushType) return;

                switch (brushType)
                {
                    case BrushType.None:
                        this.ToBrushTypeNone();
                        break;
                    case BrushType.Color:
                        this.ToBrushTypeColor();
                        break;
                    case BrushType.LinearGradient:
                        this.ToBrushTypeLinearGradient(isResetBrushArray: true);
                        this.StopsPicker.ComboBox.SetValue(ComboBox.SelectedIndexProperty, (int)GradientBrushType.Linear);//DependencyObject
                        break;
                    case BrushType.RadialGradient:
                        this.ToBrushTypeRadialGradient(isResetBrushArray: true);
                        this.StopsPicker.ComboBox.SetValue(ComboBox.SelectedIndexProperty, (int)GradientBrushType.Radial);//DependencyObject
                        break;
                    case BrushType.EllipticalGradient:
                        this.ToBrushTypeEllipticalGradient(isResetBrushArray: true);
                        this.StopsPicker.ComboBox.SetValue(ComboBox.SelectedIndexProperty, (int)GradientBrushType.Elliptical);//DependencyObject
                        break;
                    case BrushType.Image:
                        this.ToBrushTypeImage();
                        break;
                }
            };


            //BrushType   
            this.StopsPicker.ComboBox.SelectionChanged += (s, e) =>
            {
                if (this._isOpened == false) return;
                GradientBrushType gradientBrushType = (GradientBrushType)this.StopsPicker.ComboBox.SelectedIndex;

                switch (gradientBrushType)
                {
                    case GradientBrushType.Linear:
                        this.ToBrushTypeLinearGradient(isResetBrushArray: false);
                        this.BrushTypeComboBox.SetValue(ComboBox.SelectedIndexProperty, (int)BrushType.LinearGradient);//DependencyObject
                        break;
                    case GradientBrushType.Radial:
                        this.ToBrushTypeRadialGradient(isResetBrushArray: false);
                        this.BrushTypeComboBox.SetValue(ComboBox.SelectedIndexProperty, (int)BrushType.RadialGradient);//DependencyObject
                        break;
                    case GradientBrushType.Elliptical:
                        this.ToBrushTypeEllipticalGradient(isResetBrushArray: false);
                        this.BrushTypeComboBox.SetValue(ComboBox.SelectedIndexProperty, (int)BrushType.EllipticalGradient);//DependencyObject
                        break;
                }
                this.EaseStoryboard.Begin();//Storyboard
            };


            //Show
            {
                this.StopsFlyout.Opened += (s, e) => this._isOpened = true;
                this.StopsFlyout.Closed += (s, e) => this._isOpened = false;
                this.ShowControl.Tapped += async (s, e) =>
                {
                    switch (this.SelectionViewModel.BrushType)
                    {
                        case BrushType.None: break;

                        case BrushType.Color:
                            {
                                switch (this.SelectionViewModel.FillOrStroke)
                                {
                                    case FillOrStroke.Fill:
                                        this.ColorPicker.Color = this.SelectionViewModel.FillColor;
                                        break;
                                    case FillOrStroke.Stroke:
                                        this.ColorPicker.Color = this.SelectionViewModel.StrokeColor;
                                        break;
                                }
                                this.ColorFlyout.ShowAt(this);//Flyout
                            }
                            break;

                        case BrushType.LinearGradient:
                        case BrushType.RadialGradient:
                        case BrushType.EllipticalGradient:
                            {
                                CanvasGradientStop[] array = this.SelectionViewModel.BrushArray;
                                this.StopsPicker.SetArray(array);
                                this.StopsFlyout.ShowAt(this);//Flyout
                            }
                            break;

                        case BrushType.Image:
                            {
                                Transformer transformer = this.SelectionViewModel.Transformer;

                                //imageRe
                                ImageRe imageRe = await FileUtil.CreateFromLocationIdAsync(this.ViewModel.CanvasDevice, PickerLocationId.PicturesLibrary);
                                if (imageRe == null) return;

                                //Images
                                ImageRe.DuplicateChecking(imageRe);

                                //Transformer
                                Transformer transformerSource = new Transformer(imageRe.Width, imageRe.Height, Vector2.Zero);

                                //Selection
                                this.SelectionViewModel.SetValue((layer) =>
                                {
                                    switch (this.SelectionViewModel.FillOrStroke)
                                    {
                                        case FillOrStroke.Fill:
                                            layer.StyleManager.FillBrush.Source = transformerSource;
                                            layer.StyleManager.FillBrush.ImageDestination = transformer;
                                            layer.StyleManager.FillBrush.ImageStr = imageRe.ToImageStr();
                                            break;
                                        case FillOrStroke.Stroke:
                                            layer.StyleManager.StrokeBrush.Source = transformerSource;
                                            layer.StyleManager.StrokeBrush.ImageDestination = transformer;
                                            layer.StyleManager.StrokeBrush.ImageStr = imageRe.ToImageStr();
                                            break;
                                    }
                                });

                                this.SelectionViewModel.BrushImageDestination = transformer;//Selection
                                this.ViewModel.Invalidate();//Invalidate
                            }
                            break;
                    }
                };

                this.ColorPicker.ColorChange += (s, value) =>
                {
                    this.SelectionViewModel.Color = value;

                    //Brush
                    this.SelectionViewModel.BrushType = BrushType.Color;

                    //Selection
                    this.SelectionViewModel.FillColor = value;
                    this.SelectionViewModel.SetValue((layer) =>
                    { 
                        //FillOrStroke
                        switch (this.SelectionViewModel.FillOrStroke)
                        {
                            case FillOrStroke.Fill:
                                layer.StyleManager.FillBrush.Color = value;
                                break;
                            case FillOrStroke.Stroke:
                                layer.StyleManager.StrokeBrush.Color = value;
                                break;
                        }
                    });

                    this.ViewModel.Invalidate();//Invalidate
                };

                this.StopsPicker.StopsChanged += (s, array) =>
                {
                    //Selection
                    this.SelectionViewModel.BrushArray = (CanvasGradientStop[])array.Clone();

                    this.SelectionViewModel.SetValue((layer) =>
                    {
                        //FillOrStroke
                        switch (this.SelectionViewModel.FillOrStroke)
                        {
                            case FillOrStroke.Fill:
                                layer.StyleManager.FillBrush.Array = (CanvasGradientStop[])array.Clone();
                                break;
                            case FillOrStroke.Stroke:
                                layer.StyleManager.StrokeBrush.Array = (CanvasGradientStop[])array.Clone();
                                break;
                        }
                    });

                    this.ViewModel.Invalidate();//Invalidate
                };
            }
        }

        public void OnNavigatedTo() { }
        public void OnNavigatedFrom() { }
    }
}
﻿using FanKit.Transformers;
using Microsoft.Graphics.Canvas;
using Retouch_Photo2.Elements;
using Retouch_Photo2.Layers;
using Retouch_Photo2.Layers.Models;
using Retouch_Photo2.Tools.Icons;
using Retouch_Photo2.ViewModels;
using System;
using System.Numerics;
using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Retouch_Photo2.Tools.Models
{
    /// <summary>
    /// <see cref="ITool"/>'s ImageTool.
    /// </summary>
    public partial class ImageTool : Page, ITool
    {
        //@ViewModel
        ViewModel ViewModel => App.ViewModel;
        SelectionViewModel SelectionViewModel => App.SelectionViewModel;
        TipViewModel TipViewModel => App.TipViewModel;
        

        //@Construct
        public ImageTool()
        {
            this.InitializeComponent();
            this.ConstructStrings();
            this.ClearButton.Click += (s, e) => this.SelectionViewModel.Photocopier = new Photocopier();//Photocopier

            //Select
            this.SelectButton.Click += (s, e) => Retouch_Photo2.DrawPage.FrameNavigatePhotosPage?.Invoke(PhotosPageMode.SelectImage);
            Retouch_Photo2.PhotosPage.SelectCallBack += (photo) =>
            {
                if (photo == null) return;

                this.SelectionViewModel.Photocopier = photo.ToPhotocopier();//Photo
            };

            //Replace
            this.ReplaceButton.Click += (s, e) => Retouch_Photo2.DrawPage.FrameNavigatePhotosPage?.Invoke(PhotosPageMode.ReplaceImage);
            Retouch_Photo2.PhotosPage.ReplaceCallBack += (photo) =>
            {
                if (photo == null) return;
                Photocopier photocopier = photo.ToPhotocopier();

                //Transformer
                Transformer transformerSource = new Transformer(photo.Width, photo.Height, Vector2.Zero);

                //Selection
                this.SelectionViewModel.SetValue((layer) =>
                {
                    if (layer.Type == LayerType.Image)
                    {
                        ImageLayer imageLayer = (ImageLayer)layer;
                        imageLayer.Photocopier = photocopier;
                        imageLayer.Transform = new Transform
                        {
                            Source = transformerSource,
                            Destination = layer.Transform.Destination,
                        };

                        this.SelectionViewModel.StyleLayer = layer;
                    }
                });
            };
        }


        /// <summary> Tip. </summary>
        public void TipSelect() => this.EaseStoryboard.Begin();//Storyboard
        
        private Transformer CreateTransformer(Vector2 startingPoint, Vector2 point, float sizeWidth, float sizeHeight)
        {
            Matrix3x2 inverseMatrix = this.ViewModel.CanvasTransformer.GetInverseMatrix();
            Transformer canvasTransformer = Transformer.CreateWithAspectRatio(startingPoint, point, sizeWidth, sizeHeight);
            return canvasTransformer * inverseMatrix;
        }


        public void OnNavigatedTo() { }
        public void OnNavigatedFrom() { }        

    }
    
    /// <summary>
    /// <see cref="ITool"/>'s ImageTool.
    /// </summary>
    public partial class ImageTool : Page, ITool
    {
        //Strings
        private void ConstructStrings()
        {
            ResourceLoader resource = ResourceLoader.GetForCurrentView();

            this._button.ToolTip.Content = resource.GetString("/Tools/Image");

            this.SelectTextBlock.Text = resource.GetString("/Tools/Image_Select");
            this.ReplaceTextBlock.Text = resource.GetString("/Tools/Image_Replace");
            this.ClearTextBlock.Text = resource.GetString("/Tools/Image_Clear");
            this.ConvertTextBlock.Text = resource.GetString("/ToolElements/Convert");
        }


        //@Content
        public ToolType Type => ToolType.Image;
        public FrameworkElement Icon => this._icon;
        public bool IsSelected { get => this._button.IsSelected; set => this._button.IsSelected = value; }

        public FrameworkElement Button => this._button;
        public FrameworkElement Page => this;

        readonly FrameworkElement _icon = new ImageIcon();
        readonly ToolButton _button = new ToolButton(new ImageIcon());


        float _sizeWidth;
        float _sizeHeight;

        public void Started(Vector2 startingPoint, Vector2 point)
        {
            Photocopier photocopier = this.SelectionViewModel.Photocopier;
            if (photocopier.FolderRelativeId == null) { this.TipSelect(); return; }

            Photo photo = Photo.FindFirstPhoto(photocopier);
            if (photo == null)
            {
                this.TipSelect();
                return;
            }


            //Transformer
            this._sizeWidth = photo.Width;
            this._sizeHeight = photo.Height;
            Transformer transformerSource = new Transformer(photo.Width, photo.Height, Vector2.Zero);
            Transformer transformerDestination = this.CreateTransformer(startingPoint, point, photo.Width, photo.Height);

            //Mezzanine         
            this.ViewModel.MezzanineLayer = new ImageLayer(transformerSource, photocopier)
            {
                SelectMode = SelectMode.Selected,
                Transform = new Transform(transformerSource, transformerDestination),
                Style = this.SelectionViewModel.GeometryStyle
            };
            this.ViewModel.Layers.MezzanineOnFirstSelectedLayer(this.ViewModel.MezzanineLayer);

            this.SelectionViewModel.Transformer = transformerDestination;//Selection

            this.ViewModel.Invalidate(InvalidateMode.Thumbnail);//Invalidate
        }
        public void Delta(Vector2 startingPoint, Vector2 point)
        {
            //ILayer
            ILayer mezzanineLayer = this.ViewModel.MezzanineLayer;
            if (mezzanineLayer == null) return;

            Transformer transformerDestination = this.CreateTransformer(startingPoint, point, this._sizeWidth, this._sizeHeight);
            mezzanineLayer.Transform.Destination = transformerDestination;
                       
            //Selection
            this.SelectionViewModel.Transformer = transformerDestination;//Selection

            this.ViewModel.Invalidate();//Invalidate
        }
        public void Complete(Vector2 startingPoint, Vector2 point, bool isOutNodeDistance)
        {
            if (this.ViewModel.MezzanineLayer == null) return;

            if (isOutNodeDistance)
            {
                Transformer transformerDestination = this.CreateTransformer(startingPoint, point, this._sizeWidth, this._sizeHeight);
                this.ViewModel.MezzanineLayer.Transform.Destination = transformerDestination;
                this.SelectionViewModel.Transformer = transformerDestination;//Selection

                foreach (ILayer child in this.ViewModel.Layers.RootLayers)
                {
                    child.SelectMode = SelectMode.UnSelected;
                }
                this.ViewModel.MezzanineLayer.SelectMode = SelectMode.Selected;
                this.ViewModel.MezzanineLayer = null;

                this.ViewModel.Layers.ArrangeLayersControlsWithClearAndAdd();
            }
            else this.ViewModel.Layers.RemoveMezzanineLayer(this.ViewModel.MezzanineLayer);//Mezzanine

            this.SelectionViewModel.SetMode(this.ViewModel.Layers);//Selection

            this.ViewModel.Invalidate(InvalidateMode.HD);//Invalidate
        }
        public void Clicke(Vector2 point) => this.TipViewModel.MoveTool.Clicke(point);
        

        public void Draw(CanvasDrawingSession drawingSession) { }
               
    }
}
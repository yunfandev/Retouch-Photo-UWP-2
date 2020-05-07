﻿using FanKit.Transformers;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.UI.Xaml;
using Retouch_Photo2.Layers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;
using Windows.Storage;
using Windows.UI.Xaml;

namespace Retouch_Photo2.ViewModels
{
    /// <summary> 
    /// Retouch_Photo2's the only <see cref = "ViewModel" />.
    /// </summary>
    public partial class ViewModel : INotifyPropertyChanged
    {

        /// <summary>
        /// Load from a project.
        /// </summary>
        /// <param name="project"> The project. </param>
        public void LoadFromProject(Project project)
        {
            if (project == null) return;

            //Name
            this.Name = project.Name;

            //Width Height
            this.CanvasTransformer.Width = project.Width;
            this.CanvasTransformer.Height = project.Height;
            
            //Layers
            this.Layers.RootLayers.Clear();
            if (project.Layers != null)
            {
                foreach (ILayer layer in project.Layers)
                {
                    if (layer != null)
                    {
                        this.Layers.RootLayers.Add(layer);
                    }
                }
            }

            //Arrange
            this.Layers.ArrangeLayersControlsWithClearAndAdd();
            this.Layers.ArrangeLayersParents();
            this.Layers.ArrangeChildrenExpand();
        }


        /// <summary> Retouch_Photo2's the current project name. </summary>
        public string Name = null;


        /// <summary> Retouch_Photo2's the only ILayers. </summary>
        public LayerCollection Layers { get; } = new LayerCollection();
        /// <summary> Retouch_Photo2's the only Mezzanine Layer. </summary>
        public ILayer MezzanineLayer = null;


        #region Invalidate


        /// <summary> Retouch_Photo2's the only <see cref = "Microsoft.Graphics.Canvas.UI.Xaml.CanvasControl" />. </summary>
        public CanvasControl CanvasDevice { get; } = new CanvasControl();


        /// <summary>
        /// Indicates that the contents of the CanvasControl need to be redrawn.
        /// </summary>
        /// <param name="mode"> The invalidate mode </param>
        public void Invalidate(InvalidateMode mode = InvalidateMode.None)
        {
            switch (mode)
            {
                case InvalidateMode.Thumbnail:
                    this.CanvasDevice.DpiScale = 0.5f;
                    break;
                case InvalidateMode.HD:
                    this.CanvasDevice.DpiScale = 1.0f;
                    break;
            }

            this.CanvasDevice.Invalidate();//Invalidate
        }
        

        #endregion


        //@Notify 
        /// <summary> Multicast event for property change notifications. </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        /// <summary>
        /// Notifies listeners that a property value has changed.
        /// </summary>
        /// <param name="propertyName"> Name of the property used to notify listeners. </param>
        protected void OnPropertyChanged(string propertyName) => this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
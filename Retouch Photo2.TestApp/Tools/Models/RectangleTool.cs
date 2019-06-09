﻿using Retouch_Photo2.Layers;
using Retouch_Photo2.Layers.Models;
using Retouch_Photo2.Library;
using Retouch_Photo2.TestApp.Tools.Controls;
using Retouch_Photo2.TestApp.ViewModels;

namespace Retouch_Photo2.TestApp.Tools.Models
{
    /// <summary>
    /// <see cref="ICreateTool"/>'s RectangleTool.
    /// </summary>
    public class RectangleTool : ICreateTool
    {
        //ViewModel
        ViewModel ViewModel => Retouch_Photo2.TestApp.App.ViewModel;

        //@Override
        public override Layer CreateLayer(Transformer transformer) => new RectangleLayer
        {
            IsChecked=true,
            TransformerMatrix=new TransformerMatrix(transformer)
        };

        //@Construct
        public RectangleTool()
        {
            base.Type = ToolType.Rectangle;
            base.Icon = new RectangleControl();
            base.ShowIcon = new RectangleControl();
            base.Page = null;
        }      
    }
}
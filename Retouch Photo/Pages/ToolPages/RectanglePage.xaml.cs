﻿using Microsoft.Graphics.Canvas.Brushes;
using Retouch_Photo.Models;
using Retouch_Photo.Models.Layers;
using Retouch_Photo.ViewModels;
using Windows.UI;
using Windows.UI.Xaml.Input;

namespace Retouch_Photo.Pages.ToolPages
{
    public sealed partial class RectanglePage : ToolPage
    {
        //ViewModel
        DrawViewModel ViewModel => Retouch_Photo.App.ViewModel;

        public RectanglePage()
        {
            this.InitializeComponent();
        }

        //@Override
        public override void ToolOnNavigatedTo()//当前页面成为活动页面
        {
            this.ColorPicker.Color = this.ViewModel.Color;
        }
        public override void ToolOnNavigatedFrom()//当前页面不再成为活动页面
        {
        }
        

        private void ColorButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            this.ColorFlyout.ShowAt(this.ColorButton);
            this.ColorPicker.Color = this.ViewModel.Color;
        }
        private void ColorPicker_ColorChange(object sender, Color value)
        {
            this.ViewModel.Color = value;

            Layer layer = this.ViewModel.CurrentLayer;
            if (layer != null)
            {
                layer.ColorChanged(value);
                layer.Invalidate();
                this.ViewModel.Invalidate();
            }
        }

    }
}
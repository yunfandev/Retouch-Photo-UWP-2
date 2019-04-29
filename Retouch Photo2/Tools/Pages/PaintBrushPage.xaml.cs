﻿using Retouch_Photo2.Brushs;
using Retouch_Photo2.Tools;
using Retouch_Photo2.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace Retouch_Photo2.Tools.Pages
{
    public sealed partial class PaintBrushPage : ToolPage
    {
        //ViewModel
        public DrawViewModel ViewModel => Retouch_Photo2.App.ViewModel;


        public PaintBrushPage()
        {
            this.InitializeComponent();
        }

        //@Override
        public override void ToolOnNavigatedTo()//当前页面成为活动页面
        {
            if (this.ViewModel.CurrentGeometryLayer == null) return;
            BrushType type = this.ViewModel.CurrentGeometryLayer.FillBrush.Type;
            this.BrushControl.ChangeSelectedIndex((int)type);
        }

        //Communication
        public override void Communication(int magicNumbers) => this.BrushControl.ChangeSelectedIndex(magicNumbers);
    }
}
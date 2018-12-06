﻿using Retouch_Photo.Library;
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

namespace Retouch_Photo.Pages.ToolPages.ToolMarqueePage
{
    public sealed partial class ToolMarqueeModeControl : UserControl
    {

        #region DependencyProperty

        public MarqueeMode Mode
        {
            get { return (MarqueeMode)GetValue(ModeProperty); }
            set { SetValue(ModeProperty, value); }
        }
        public static readonly DependencyProperty ModeProperty = DependencyProperty.Register(nameof(Mode), typeof(MarqueeMode), typeof(ToolMarqueeModeControl), new PropertyMetadata(MarqueeMode.None, (sender, e) =>
        {
            ToolMarqueeModeControl con = (ToolMarqueeModeControl)sender;

            if (e.NewValue is MarqueeMode value)
            {
                con.Segmente(value);
            }
        }));

        #endregion

        //Delegate
        public delegate void ModeChangedHandler(MarqueeMode mode);
        public event ModeChangedHandler ModeChanged = null;

        public ToolMarqueeModeControl()
        {           
            this.InitializeComponent(); 
        }

        private void NoneSegmented_Tapped(object sender, TappedRoutedEventArgs e) => this.Mode = MarqueeMode.None;
        private void SquareSegmented_Tapped(object sender, TappedRoutedEventArgs e) => this.Mode = MarqueeMode.Square;
        private void CenterSegmented_Tapped(object sender, TappedRoutedEventArgs e) => this.Mode = MarqueeMode.Center;
        private void SquareAndCenterSegmented_Tapped(object sender, TappedRoutedEventArgs e) => this.Mode = MarqueeMode.SquareAndCenter;

        private void Segmente(MarqueeMode mode)
        {
            this.SegmenteColor(this.NoneSegmented, mode == MarqueeMode.None);
            this.SegmenteColor(this.SquareSegmented, mode == MarqueeMode.Square);
            this.SegmenteColor(this.CenterSegmented, mode == MarqueeMode.Center);
            this.SegmenteColor(this.SquareAndCenterSegmented, mode == MarqueeMode.SquareAndCenter);

            this.ModeChanged?.Invoke(mode);
        }
         

        private void SegmenteColor(ContentPresenter control, bool IsChecked)
        {
            control.Background = IsChecked ? this.AccentColor : this.UnAccentColor;
            control.Foreground = IsChecked ? this.CheckColor : this.UnCheckColor;
        }



    }
}

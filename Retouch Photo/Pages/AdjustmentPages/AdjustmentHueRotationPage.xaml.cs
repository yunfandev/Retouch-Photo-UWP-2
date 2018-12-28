﻿using System;
using System.Collections.Generic;
using Retouch_Photo.Models.Adjustments;
using Retouch_Photo.ViewModels;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;

namespace Retouch_Photo.Pages.AdjustmentPages
{
    public sealed partial class AdjustmentHueRotationPage : Page
    {

        //ViewModel
        DrawViewModel ViewModel => App.ViewModel;

        #region DependencyProperty

        public HueRotationAdjustment HueRotationAdjustment
        {
            get { return (HueRotationAdjustment)GetValue(MyPropertyProperty); }
            set { SetValue(MyPropertyProperty, value); }
        }
        public static readonly DependencyProperty MyPropertyProperty = DependencyProperty.Register(nameof(HueRotationAdjustment), typeof(HueRotationAdjustment), typeof(HueRotationAdjustment), new PropertyMetadata(null, (sender, e) =>
        {
            AdjustmentHueRotationPage con = (AdjustmentHueRotationPage)sender;

            if (e.NewValue is HueRotationAdjustment adjustment)
            {
                con.HueRotationSlider.Value = adjustment.Angle * 180/Math.PI;
            }
        }));

        #endregion


        public AdjustmentHueRotationPage()
        {
            this.InitializeComponent();
        }

        private void HueRotationSlider_ValueChangeDelta(object sender, RangeBaseValueChangedEventArgs e)
        {
            this.HueRotationAdjustment.Angle = (float)(e.NewValue * Math.PI / 180);
            this.ViewModel.Invalidate();
        }
    }
}


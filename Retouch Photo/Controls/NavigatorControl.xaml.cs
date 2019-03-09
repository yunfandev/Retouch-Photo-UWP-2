﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.ApplicationModel.Resources;
using Retouch_Photo.ViewModels;
using Retouch_Photo.Library;

namespace Retouch_Photo.Controls
{
    public sealed partial class NavigatorControl : UserControl
    {

        //ViewModel
        DrawViewModel ViewModel => Retouch_Photo.App.ViewModel;


        private ApplicationViewTitleBar TitleBar = ApplicationView.GetForCurrentView().TitleBar;
        public ElementTheme TitleRequestedTheme
        {
            set
            {
                this.ThemeIcon.Glyph = value == ElementTheme.Light ? "\uE706" : "\uEC46";
   
                this.TitleBar.ButtonInactiveBackgroundColor =
                this.TitleBar.ButtonBackgroundColor =
                this.TitleBar.InactiveBackgroundColor =
                this.TitleBar.BackgroundColor =
                value == ElementTheme.Light ? Color.FromArgb(255, 243, 243, 245) : Color.FromArgb(255, 32, 32, 33);
            }
        }
        public ElementTheme AppRequestedTheme
        {
            set
            {
                if (Window.Current.Content is FrameworkElement frameworkElement)
                {
                    frameworkElement.RequestedTheme = value;
                }
            }
        }


        public NavigatorControl()
        {
            this.InitializeComponent();

            //Navigator
            this.FiftyPercent.Tapped += (sender, e) => this.Navigator((m) => m.Fit(0.5f));
            this.HundredPercent.Tapped += (sender, e) => this.Navigator((m) => m.Fit(1f));
            this.TwoHundredPercent.Tapped += (sender, e) => this.Navigator((m) => m.Fit(2f));
            this.AutoPercent.Tapped += (sender, e) => this.Navigator((m) => m.Fit());
          

            //Theme
            this.ThemeSwitch.Loaded += (sender, e) =>
            {
                if (Window.Current.Content is FrameworkElement frameworkElement)
                {
                    this.ThemeSwitch.IsOn = (frameworkElement.RequestedTheme != ElementTheme.Light);
                    this.TitleRequestedTheme = frameworkElement.RequestedTheme;
                }
            };

            this.ThemeSwitch.Toggled += (sender, e) =>
            {
                this.TitleRequestedTheme =
                this.AppRequestedTheme =
                (this.ThemeSwitch.IsOn ? ElementTheme.Dark : ElementTheme.Light);
            };


            //Ruler
            this.RulerSwitch.Toggled += (sender, e) =>
            {
                this.ViewModel.RenderLayer.IsRuler = this.RulerSwitch.IsOn;
                this.ViewModel.Invalidate();
            };

            
        }

        private void Navigator(Action<MatrixTransformer> action)
        {
            action(this.ViewModel.MatrixTransformer);

            App.ViewModel.Invalidate();
        }
    }
}
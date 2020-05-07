﻿using Retouch_Photo2.ViewModels;
using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml.Controls;

namespace Retouch_Photo2.Tools.Elements
{
    /// <summary>
    /// Control of <see cref = "KeyboardViewModel.IsSquare" /> and <see cref = "KeyboardViewModel.IsCenter" />.
    /// </summary>
    public sealed partial class MoreCreateControl : UserControl
    {
        //@ViewModel
        ViewModel ViewModel => App.ViewModel;
        KeyboardViewModel KeyboardViewModel => App.KeyboardViewModel;

        //@Construct
        public MoreCreateControl()
        {
            this.InitializeComponent();
            this.ConstructStrings();
        }     
        
        //Strings
        private void ConstructStrings()
        {
            ResourceLoader resource = ResourceLoader.GetForCurrentView();

            this.SquareTextBlock.Text = resource.GetString("/ToolElements/MoreCreate_Square");

            this.CenterTextBlock.Text = resource.GetString("/ToolElements/MoreCreate_Center");
        }

    }
}
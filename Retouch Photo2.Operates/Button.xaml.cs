﻿using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace Retouch_Photo2.Operates
{
    public sealed partial class Button : UserControl
    {
        //@Delegate
        public event TappedEventHandler ButtonTapped;

        public UIElement EnabledIcon { get => this.EnabledViewbox.Child; set => this.EnabledViewbox.Child = value; }
        public UIElement DisabledIcon { get => this.DisabledViewbox.Child; set => this.DisabledViewbox.Child = value; }

        public bool ButtonIsEnabled
        {
            set
            {
                this.RootButton.IsEnabled = value;

                this.EnabledViewbox.Visibility = value ? Visibility.Visible : Visibility.Collapsed;
                this.DisabledViewbox.Visibility = value ? Visibility.Collapsed : Visibility.Visible;
            }
        }

        public Button()
        {
            this.InitializeComponent();
            this.RootButton.Tapped += (s, e) => this.ButtonTapped?.Invoke(s, e);
        }
    }
}

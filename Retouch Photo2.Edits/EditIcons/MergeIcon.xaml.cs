﻿using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Retouch_Photo2.Edits.EditIcons
{
    /// <summary>
    /// Icon of Merge.
    /// </summary>
    public sealed partial class MergeIcon : UserControl
    {

        //@VisualState
        bool _vsIsEnabled => this.IsEnabled;
        /// <summary> 
        /// Represents the visual appearance of UI elements in a specific state.
        /// </summary>
        public VisualState VisualState
        {
            get
            {
                if (this.IsEnabled) return this.Normal;
                else return this.Disabled;
            }
            set => VisualStateManager.GoToState(this, value.Name, false);
        }

        //@Construct
        /// <summary>
        /// Initializes a MergeIcon. 
        /// </summary>
        public MergeIcon()
        {
            this.InitializeComponent();
            this.Loaded += (s, e) => this.VisualState = this.VisualState;//State
            this.IsEnabledChanged += (s, e) =>
            {
                if (e.NewValue != e.OldValue)
                {
                    this.VisualState = this.VisualState;//State
                }
            };
        }
    }
}
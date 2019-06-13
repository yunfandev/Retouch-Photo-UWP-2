﻿using Retouch_Photo2.Layers;
using Retouch_Photo2.TestApp.ViewModels;
using Windows.UI.Xaml.Controls;

namespace Retouch_Photo2.TestApp.Controls
{
    /// <summary> 
    /// Retouch_Photo2's the only <see cref = "LayersControl" />. 
    /// </summary>
    public sealed partial class LayerControl : UserControl
    {
        //ViewModel
        ViewModel ViewModel => Retouch_Photo2.TestApp.App.ViewModel;
 
        //@Converter
        private double OpacityToValueConverter(float opacity) => opacity * 100.0d;
        private float ValueToOpacityConverter(double value) => (float)value / 100.0f;

        private double BoolToOpacityConverter(bool isChecked) => isChecked ? 1.0 : 0.4;
         
        private bool SelectionModeToVoolConverter(ListViewSelectionMode selectionMode) => (selectionMode == ListViewSelectionMode.None) ? false : true;
 

        //@Construct
        public LayerControl()
        {
            this.InitializeComponent();

            this.OpacitySlider.ValueChanged += (s, e) =>
            {
                float opacity = this.ValueToOpacityConverter(e.NewValue);
                                       
                //Selection
                this.ViewModel.SelectionOpacity = opacity;
                this.ViewModel.SelectionSetValue((layer) =>
                {
                    layer.Opacity = opacity;
                });

                this.ViewModel.Invalidate();//Invalidate
            };
            this.BlendControl.TypeChanged += (type) =>
            {
                //Selection
                this.ViewModel.SelectionBlendType = type;
                this.ViewModel.SelectionSetValue((layer) =>
                {
                    layer.BlendType = type;
                });

                this.ViewModel.Invalidate();//Invalidate
            };            

            this.VisualButton.Tapped += (s, e) =>
            {
                bool value = !this.ViewModel.SelectionIsVisual;

                //Selection
                this.ViewModel.SelectionIsVisual = value;
                this.ViewModel.SelectionSetValue((layer) => 
                {
                    layer.IsVisual = value;
                });

                this.ViewModel.Invalidate();//Invalidate
            };
            this.RemoveButton.Tapped += (s, e) =>
            {
                switch (this.ViewModel.SelectionMode)
                {
                    case ListViewSelectionMode.None:
                        break;
                    case ListViewSelectionMode.Single:
                        {
                            this.ViewModel.Layers.Remove(this.ViewModel.SelectionLayer);

                            this.ViewModel.SetSelectionModeNone();//Selection
                            this.ViewModel.Invalidate();//Invalidate
                        }
                        break;
                    case ListViewSelectionMode.Multiple:
                        {
                            foreach (Layer layer in this.ViewModel.SelectionLayers)
                            {
                                this.ViewModel.Layers.Remove(this.ViewModel.SelectionLayer);
                            }

                            this.ViewModel.SetSelectionModeNone();//Selection
                            this.ViewModel.Invalidate();//Invalidate
                        }
                        break;
                }
            };
        }


    }
}

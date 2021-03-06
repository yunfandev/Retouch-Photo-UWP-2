﻿using Retouch_Photo2.Effects.Icons;
using Retouch_Photo2.ViewModels;
using Windows.ApplicationModel.Resources;
using Retouch_Photo2.Historys;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Retouch_Photo2.Layers;

namespace Retouch_Photo2.Effects.Models
{
    /// <summary>
    /// Page of <see cref = "Effect.Morphology_IsOn"/>.
    /// </summary>
    public sealed partial class MorphologyEffectPage : Page, IEffectPage
    {

        //@ViewModel
        ViewModel ViewModel => App.ViewModel;
        ViewModel SelectionViewModel => App.SelectionViewModel;
        ViewModel MethodViewModel => App.MethodViewModel;
        

        //@Content
        private int Size
        {
            set
            {
                this.SizePicker.Value = value;
                this.SizeSlider.Value = value;
            }
        }


        //@Construct
        /// <summary>
        /// Initializes a MorphologyEffectPage. 
        /// </summary>
        public MorphologyEffectPage()
        {
            this.InitializeComponent();
            this.ConstructString();

            this.ConstructIsOn();

            this.ConstructSize1();
            this.ConstructSize2();
        }
    }

    /// <summary>
    /// Page of <see cref = "Effect.Morphology_IsOn"/>.
    /// </summary>
    public sealed partial class MorphologyEffectPage : Page, IEffectPage
    {
        //String
        private void ConstructString()
        {
            ResourceLoader resource = ResourceLoader.GetForCurrentView();

            this.Button.Text = resource.GetString("/Effects/Morphology");

            this.SizeTextBlock.Text = resource.GetString("/Effects/Morphology_Size");
        }

        //@Content
        /// <summary> Gets the type. </summary>
        public EffectType Type => EffectType.Morphology;
        /// <summary> Gets the page. </summary>
        public FrameworkElement Page => this;
        /// <summary> Gets the button. </summary>
        public EffectButton Button { get; } = new EffectButton
        {
            Icon = new MorphologyIcon()
        };
        
        public void Reset()
        {
            this.Size = 1;

            this.MethodViewModel.EffectChanged<int>
            (
                set: (effect) => effect.Morphology_Size = 1,

                historyTitle: "Set effect morphology effect",
                getHistory: (effect) => effect.Morphology_Size,
                setHistory: (effect, previous) => effect.Morphology_Size = previous
            );
        }
        public void FollowButton(Effect effect)
        {
            this.Button.IsOn = effect.Morphology_IsOn;
        }
        public void FollowPage(Effect effect)
        {
            this.SizeSlider.Value = effect.Morphology_Size;
        }
    }

    /// <summary>
    /// Page of <see cref = "Effect.Morphology_IsOn"/>.
    /// </summary>
    public sealed partial class MorphologyEffectPage : Page, IEffectPage
    {

        //IsOn
        private void ConstructIsOn()
        {
            this.Button.Toggled += (isOn) => this.MethodViewModel.EffectChanged<bool>
            (
                set: (effect) => effect.Morphology_IsOn = isOn,

                historyTitle: "Set effect morphology is on",
                getHistory: (effect) => effect.Morphology_IsOn,
                setHistory: (effect, previous) => effect.Morphology_IsOn = previous
            );            
        }


        //Size
        private void ConstructSize1()
        {
            this.SizePicker.Unit = null;
            this.SizePicker.Minimum = -100;
            this.SizePicker.Maximum = 100;
            this.SizePicker.ValueChanged += (s, value) =>
            {
                int size = value;
                this.Size = size;

                this.MethodViewModel.EffectChanged<int>
                (
                    set: (effect) => effect.Morphology_Size = size,

                    historyTitle: "Set effect morphology size",
                    getHistory: (effect) => effect.Morphology_Size,
                    setHistory: (effect, previous) => effect.Morphology_Size = previous
                );
            };
        }

        private void ConstructSize2()
        {
            this.SizeSlider.Minimum = -100.0d;
            this.SizeSlider.Maximum = 100.0d;
            this.SizeSlider.ValueChangeStarted += (s, value) => this.MethodViewModel.EffectChangeStarted(cache: (effect) => effect.CacheMorphology());
            this.SizeSlider.ValueChangeDelta += (s, value) =>
            {
                int size = (int)value;
                this.Size = size;

                this.MethodViewModel.EffectChangeDelta(set: (effect) => effect.Morphology_Size = size);
            };
            this.SizeSlider.ValueChangeCompleted += (s, value) =>
            {
                int size = (int)value;
                this.Size = size;

                this.MethodViewModel.EffectChangeCompleted<int>
                (
                    set: (effect) => effect.Morphology_Size = size,

                    historyTitle: "Set effect morphology size",
                    getHistory: (effect) => effect.StartingMorphology_Size,
                    setHistory: (effect, previous) => effect.Morphology_Size = previous
                );
            };
        }

    }
}
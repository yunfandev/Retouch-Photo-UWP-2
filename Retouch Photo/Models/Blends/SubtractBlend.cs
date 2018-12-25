﻿using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using Retouch_Photo.Controls.BlendControl;
using Windows.UI.Xaml;

namespace Retouch_Photo.Models.Blends
{
    public class SubtractBlend : Blend
    {
        public SubtractBlend()
        {
            base.Type = BlendType.Subtract;
        }

        protected override FrameworkElement GetIcon() => new BlendSubtractControl();
        protected override ICanvasImage GetRender(ICanvasImage background, ICanvasImage foreground)
        {
            return new BlendEffect
            {
                Background = background,
                Foreground = foreground,
                Mode = BlendEffectMode.Subtract
            };
        }
    }
}

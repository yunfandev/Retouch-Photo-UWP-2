﻿using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using Retouch_Photo.Controls.BlendControl;
using Windows.UI.Xaml;

namespace Retouch_Photo.Models.Blends
{
    public class DarkerColorBlend : Blend
    {
        public DarkerColorBlend()
        {
            base.Type = BlendType.DarkerColor;
        }

        protected override FrameworkElement GetIcon() => new BlendDarkerColorControl();
        protected override ICanvasImage GetRender(ICanvasImage background, ICanvasImage foreground)
        {
            return new BlendEffect
            {
                Background = background,
                Foreground = foreground,
                Mode = BlendEffectMode.DarkerColor
            };
        }
    }
}

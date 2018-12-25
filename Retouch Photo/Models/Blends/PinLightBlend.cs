﻿using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using Retouch_Photo.Controls.BlendControl;
using Windows.UI.Xaml;

namespace Retouch_Photo.Models.Blends
{
    public class PinLightBlend : Blend
    {
        public PinLightBlend()
        {
            base.Type = BlendType.PinLight;
        }

        protected override FrameworkElement GetIcon() => new BlendPinLightControl();
        protected override ICanvasImage GetRender(ICanvasImage background, ICanvasImage foreground)
        {
            return new BlendEffect
            {
                Background = background,
                Foreground = foreground,
                Mode = BlendEffectMode.PinLight
            };
        }
    }
}

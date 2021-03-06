﻿using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using System.Numerics;
using System.Xml.Linq;
using Windows.UI.Xaml;

namespace Retouch_Photo2.Adjustments.Models
{
    /// <summary>
    /// <see cref="IAdjustment"/>'s BrightnessAdjustment.
    /// </summary>
    public class BrightnessAdjustment : IAdjustment
    {
        //@Static
        //@Generic
        public static string GenericText = "Brightness";
        public static IAdjustmentPage GenericPage;// = new BrightnessPage();

        //@Content
        public AdjustmentType Type => AdjustmentType.Brightness;
        public Visibility PageVisibility => Visibility.Visible;
        public IAdjustmentPage Page { get; } = BrightnessAdjustment.GenericPage;
        public string Text => BrightnessAdjustment.GenericText;


        /// <summary> Interval 1.0->0.5 . </summary>
        public float WhiteLight = 1.0f;
        public float StartingWhiteLight { get; private set; }
        public void CacheWhiteLight() => this.StartingWhiteLight = this.WhiteLight;

        /// <summary> Interval 1.0->0.5 . </summary>
        public float WhiteDark = 1.0f;
        public float StartingWhiteDark { get; private set; }
        public void CacheWhiteDark() => this.StartingWhiteDark = this.WhiteDark;

        /// <summary> Interval 0.0->0.5 . </summary>
        public float BlackLight = 0.0f;
        public float StartingBlackLight { get; private set; }
        public void CacheBlackLight() => this.StartingBlackLight = this.BlackLight;

        /// <summary> Interval 0.0->0.5 . </summary>
        public float BlackDark = 0.0f;
        public float StartingBlackDark { get; private set; }
        public void CacheBlackDark() => this.StartingBlackDark = this.BlackDark;
        

        public IAdjustment Clone()
        {
            return new BrightnessAdjustment
            {
                WhiteLight = this.WhiteLight,
                WhiteDark = this.WhiteDark,
                BlackLight = this.BlackLight,
                BlackDark = this.BlackDark,
            };
        }


        public void SaveWith(XElement element)
        {
            element.Add(new XAttribute("WhiteLight", this.WhiteLight));
            element.Add(new XAttribute("WhiteDark", this.WhiteDark));
            element.Add(new XAttribute("BlackLight", this.BlackLight));
            element.Add(new XAttribute("BlackDark", this.BlackDark));
        }
        public void Load(XElement element)
        {
            this.WhiteLight = (float)element.Attribute("WhiteLight");
            this.WhiteDark = (float)element.Attribute("WhiteDark");
            this.BlackLight = (float)element.Attribute("BlackLight");
            this.BlackDark = (float)element.Attribute("BlackDark");
        }


        public ICanvasImage GetRender(ICanvasImage image)
        {
            return new BrightnessEffect
            {
                WhitePoint = new Vector2(this.WhiteLight, this.WhiteDark),
                BlackPoint = new Vector2(this.BlackDark, this.BlackLight),
                Source = image
            };
        }

    }
}
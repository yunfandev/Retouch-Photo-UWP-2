﻿using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using Retouch_Photo2.Layers;
using Retouch_Photo2.Library;
using Retouch_Photo2.Transformers;
using Retouch_Photo2.TestApp.ViewModels;
using System.Numerics;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Microsoft.Graphics.Canvas.Text;
using System;

namespace Retouch_Photo2.TestApp.Controls
{
    /// <summary> 
    /// Retouch_Photo2's the only <see cref = "MainCanvasControl" />. 
    /// </summary>
    public sealed partial class MainCanvasControl : UserControl
    {
        //ViewModel
        ViewModel ViewModel => Retouch_Photo2.TestApp.App.ViewModel;

        //Single
        bool isSingleStarted;
        Vector2 singleStartingPoint;
        //Right
        Vector2 rightStartPoint;
        //Double
        Vector2 doubleStartCenter;
        Vector2 doubleStartPosition;
        float doubleStartScale;
        float doubleStartSpace;

        //IsRuler
        float ControlWidth => this.ViewModel.CanvasTransformer.ControlWidth;
        float ControlHeight => this.ViewModel.CanvasTransformer.ControlHeight;
        float Scale => this.ViewModel.CanvasTransformer.Scale;
        readonly float RulerSpace = 20;
        readonly CanvasTextFormat RulerTextFormat = new CanvasTextFormat()
        {
            FontSize = 12,
            HorizontalAlignment = CanvasHorizontalAlignment.Center,
            VerticalAlignment = CanvasVerticalAlignment.Center
        };


        #region DependencyProperty


        /// <summary> Gets or sets <see cref = "MainCanvasControl" />'s accent color. </summary>
        public Color AccentColor
        {
            get { return (Color)GetValue(AccentColorProperty); }
            set { SetValue(AccentColorProperty, value); }
        }
        /// <summary> Identifies the <see cref = "MainCanvasControl.AccentColor" /> dependency property. </summary>
        public static readonly DependencyProperty AccentColorProperty = DependencyProperty.Register(nameof(AccentColor), typeof(Color), typeof(MainCanvasControl), new PropertyMetadata(Colors.DodgerBlue, (sender, e) =>
        {
            MainCanvasControl con = (MainCanvasControl)sender;

            if (e.NewValue is Color value)
            {
                con.CanvasControl.Invalidate();
            }
        }));


        /// <summary> Gets or sets <see cref = "MainCanvasControl" />'s shadow color. </summary>
        public Color ShadowColor
        {
            get { return (Color)GetValue(ShadowColorProperty); }
            set { SetValue(ShadowColorProperty, value); }
        }
        /// <summary> Identifies the <see cref = "MainCanvasControl.ShadowColor" /> dependency property. </summary>
        public static readonly DependencyProperty ShadowColorProperty = DependencyProperty.Register(nameof(ShadowColor), typeof(Color), typeof(MainCanvasControl), new PropertyMetadata(Colors.Black, (sender, e) =>
        {
            MainCanvasControl con = (MainCanvasControl)sender;

            if (e.NewValue is Color value)
            {
                con.CanvasControl.Invalidate();
            }
        }));


        /// <summary> Sets or Gets the on state of the ruler on the <see cref = "MainCanvasControl" />. </summary>
        public bool IsRuler
        {
            get { return (bool)GetValue(IsRulerProperty); }
            set { SetValue(IsRulerProperty, value); }
        }
        /// <summary> Identifies the <see cref = "MainCanvasControl.IsRuler" /> dependency property. </summary>
        public static readonly DependencyProperty IsRulerProperty = DependencyProperty.Register(nameof(IsRuler), typeof(bool), typeof(MainCanvasControl), new PropertyMetadata(false, (sender, e) =>
        {
            MainCanvasControl con = (MainCanvasControl)sender;

            if (e.NewValue is bool value)
            {
                con.CanvasControl.Invalidate();
            }
        }));



        #endregion


        //@Construct
        public MainCanvasControl()
        {
            this.InitializeComponent();
            this.SizeChanged += (s, e) => 
            {
                if (e.NewSize == e.PreviousSize) return;
                this.ViewModel.CanvasTransformer.Size = e.NewSize;
            };

            //Draw
            this.ViewModel.InvalidateAction = (mode) =>
            {
                switch (mode)
                {
                    case InvalidateMode.Thumbnail:
                        this.CanvasControl.DpiScale = 0.5f;
                        break;
                    case InvalidateMode.HD:
                        this.CanvasControl.DpiScale = 1.0f;
                        break;
                }

                this.CanvasControl.Invalidate();
            };
            this.CanvasControl.Draw += (sender, args) =>
            {
                //Blank Image
                ICanvasImage previousImage = new ColorSourceEffect { Color = Colors.White };


                //Render & Mezzanine
                {
                    //Render
                    Matrix3x2 canvasToVirtualMatrix = this.ViewModel.CanvasTransformer.GetMatrix(MatrixTransformerMode.CanvasToVirtual);

                    void aaa() =>
                      previousImage = Layer.Render(this.ViewModel.CanvasDevice, this.ViewModel.Mezzanine.Layer, previousImage, canvasToVirtualMatrix);

                    void bbb(int i) =>
                        previousImage = Layer.Render(this.ViewModel.CanvasDevice, this.ViewModel.Layers[i], previousImage, canvasToVirtualMatrix);


                    //Mezzanine 
                    if (this.ViewModel.Mezzanine.Layer != null)
                    {
                        if (this.ViewModel.Layers.Count == 0) aaa();
                        else
                        {
                            for (int i = this.ViewModel.Layers.Count - 1; i >= 0; i--)
                            {
                                if (this.ViewModel.Mezzanine.Index == i) aaa();

                                bbb(i);
                            }
                        }
                    }
                    else
                    {
                        for (int i = this.ViewModel.Layers.Count - 1; i >= 0; i--)
                        {
                            bbb(i);
                        }
                    }
                }


                //Crop & Final
                {
                    //Crop : Get the border from MatrixTransformer
                    float width = this.ViewModel.CanvasTransformer.Width * this.ViewModel.CanvasScale;
                    float height = this.ViewModel.CanvasTransformer.Height * this.ViewModel.CanvasScale;
                    ICanvasImage cropRect = new CropEffect
                    {
                        Source = previousImage,
                        SourceRectangle = new Rect(-width / 2, -height / 2, width, height),
                    };


                    //Final : Draw to Canvas
                    ICanvasImage finalCanvas = new Transform2DEffect
                    {
                        Source = cropRect,
                        TransformMatrix = this.ViewModel.CanvasTransformer.GetMatrix(MatrixTransformerMode.VirtualToControl)
                    };
                    ICanvasImage shadow = new ShadowEffect
                    {
                        Source = finalCanvas,
                        ShadowColor = this.ShadowColor,
                        BlurAmount = 4.0f
                    };
                    args.DrawingSession.DrawImage(shadow, 5.0f, 5.0f);
                    args.DrawingSession.DrawImage(finalCanvas);
                }


                //Selection & Mezzanine
                {
                    if (this.ViewModel.Mezzanine.Layer == null)
                    {
                        //Selection
                        switch (this.ViewModel.SelectionMode)
                        {
                            case ListViewSelectionMode.None:
                                break;
                            case ListViewSelectionMode.Single:
                            case ListViewSelectionMode.Multiple:
                                {
                                    Transformer transformer = this.ViewModel.GetSelectionTransformer();
                                    Matrix3x2 matrix = this.ViewModel.CanvasTransformer.GetMatrix();
                                    args.DrawingSession.DrawBoundNodes(transformer, matrix, this.AccentColor);
                                }
                                break;
                        }
                    }
                    else
                    {
                        //Mezzanine 
                        Matrix3x2 matrix = this.ViewModel.CanvasTransformer.GetMatrix();
                        args.DrawingSession.DrawBound(this.ViewModel.Mezzanine.Layer.TransformerMatrix.Destination, matrix, this.AccentColor);
                    }
                }


                //Tool
                this.ViewModel.Tool.Draw(args.DrawingSession);


                //IsRuler
                if (this.IsRuler)
                {
                    Vector2 position = this.ViewModel.CanvasTransformer.GetMatrix().Translation;

                    //line
                    args.DrawingSession.FillRectangle(0, 0, this.ControlWidth, this.RulerSpace, Windows.UI.Color.FromArgb(64, 127, 127, 127));//Horizontal
                    args.DrawingSession.FillRectangle(0, 0, this.RulerSpace, this.ControlHeight, Windows.UI.Color.FromArgb(64, 127, 127, 127));//Vertical
                    args.DrawingSession.DrawLine(0, this.RulerSpace, this.ControlWidth, this.RulerSpace, Windows.UI.Colors.Gray);//Horizontal
                    args.DrawingSession.DrawLine(this.RulerSpace, 0, this.RulerSpace, this.ControlHeight, Windows.UI.Colors.Gray);//Vertical

                    //space
                    float space = (10 * this.Scale);
                    while (space < 10) space *= 5;
                    while (space > 100) space /= 5;
                    float spaceFive = space * 5;

                    //Horizontal
                    for (float X = position.X; X < this.ControlWidth; X += space) args.DrawingSession.DrawLine(X, 10, X, this.RulerSpace, Windows.UI.Colors.Gray);
                    for (float X = position.X; X > this.RulerSpace; X -= space) args.DrawingSession.DrawLine(X, 10, X, this.RulerSpace, Windows.UI.Colors.Gray);
                    //Vertical
                    for (float Y = position.Y; Y < this.ControlHeight; Y += space) args.DrawingSession.DrawLine(10, Y, this.RulerSpace, Y, Windows.UI.Colors.Gray);
                    for (float Y = position.Y; Y > this.RulerSpace; Y -= space) args.DrawingSession.DrawLine(10, Y, this.RulerSpace, Y, Windows.UI.Colors.Gray);

                    //Horizontal
                    for (float X = position.X; X < this.ControlWidth; X += spaceFive) args.DrawingSession.DrawLine(X, 10, X, this.RulerSpace, Windows.UI.Colors.Gray);
                    for (float X = position.X; X > this.RulerSpace; X -= spaceFive) args.DrawingSession.DrawLine(X, 10, X, this.RulerSpace, Windows.UI.Colors.Gray);
                    //Vertical
                    for (float Y = position.Y; Y < this.ControlHeight; Y += spaceFive) args.DrawingSession.DrawLine(10, Y, this.RulerSpace, Y, Windows.UI.Colors.Gray);
                    for (float Y = position.Y; Y > this.RulerSpace; Y -= spaceFive) args.DrawingSession.DrawLine(10, Y, this.RulerSpace, Y, Windows.UI.Colors.Gray);

                    //Horizontal
                    for (float X = position.X; X < this.ControlWidth; X += spaceFive) args.DrawingSession.DrawText(((int)(Math.Round((X - position.X) / this.Scale))).ToString(), X, 10, Windows.UI.Colors.Gray, RulerTextFormat);
                    for (float X = position.X; X > this.RulerSpace; X -= spaceFive) args.DrawingSession.DrawText(((int)(Math.Round((X - position.X) / this.Scale))).ToString(), X, 10, Windows.UI.Colors.Gray, RulerTextFormat);
                    //Vertical
                    for (float Y = position.Y; Y < this.ControlHeight; Y += spaceFive) args.DrawingSession.DrawText(((int)(Math.Round((Y - position.Y) / this.Scale))).ToString(), 10, Y, Windows.UI.Colors.Gray, RulerTextFormat);
                    for (float Y = position.Y; Y > this.RulerSpace; Y -= spaceFive) args.DrawingSession.DrawText(((int)(Math.Round((Y - position.Y) / this.Scale))).ToString(), 10, Y, Windows.UI.Colors.Gray, RulerTextFormat);
                }
            };


            //Single
            this.CanvasOperator.Single_Start += (point) =>
            {
                this.isSingleStarted = false;
                this.singleStartingPoint = point;

               this.ViewModel.Tool.Starting(point);//Starting
            };
            this.CanvasOperator.Single_Delta += (point) =>
            {
                //Delta
                if (this.isSingleStarted)
                {
                    this.ViewModel.Tool.Delta(this.singleStartingPoint, point);//Delta
                    return;
                }

                //Started
                if ((this.singleStartingPoint - point).LengthSquared() > 400.0f)
                {
                    this.isSingleStarted = true;

                    this.ViewModel.Tool.Started(this.singleStartingPoint, point);//Started
                }
            };
            this.CanvasOperator.Single_Complete += (point) => this.ViewModel.Tool.Complete(this.singleStartingPoint, point, this.isSingleStarted);//Started


            //Right
            this.CanvasOperator.Right_Start += (point) =>
            {
                this.rightStartPoint = point;
                this.ViewModel.ViewTool.Started(this.rightStartPoint, point);//Started
            };
            this.CanvasOperator.Right_Delta += (point) => this.ViewModel.ViewTool.Delta(this.rightStartPoint, point);//Delta
            this.CanvasOperator.Right_Complete += (point) => this.ViewModel.ViewTool.Complete(this.rightStartPoint, point, this.isSingleStarted);//Started


            //Double
            this.CanvasOperator.Double_Start += (center, space) =>
            {
                this.doubleStartCenter = (center - this.ViewModel.CanvasTransformer.Position) / this.ViewModel.CanvasScale + this.ViewModel.CanvasTransformer.ControlCenter;
                this.doubleStartPosition = this.ViewModel.CanvasTransformer.Position;

                this.doubleStartSpace = space;
                this.doubleStartScale = this.ViewModel.CanvasScale;

                this.ViewModel.Invalidate(InvalidateMode.Thumbnail);
            };
            this.CanvasOperator.Double_Delta += (center, space) =>
            {
                this.ViewModel.CanvasScale = this.doubleStartScale / this.doubleStartSpace * space;
                this.ViewModel.CanvasTransformer.Position = center - (this.doubleStartCenter - this.ViewModel.CanvasTransformer.ControlCenter) * this.ViewModel.CanvasScale;
                this.ViewModel.CanvasTransformer.ReloadMatrix();

                this.ViewModel.Invalidate();
            };
            this.CanvasOperator.Double_Complete += (center, space) => this.ViewModel.Invalidate(InvalidateMode.HD);


            //Wheel
            this.CanvasOperator.Wheel_Changed += (point, space) =>
            {
                if (space > 0)
                {
                    if (this.ViewModel.CanvasScale < 10f)
                    {
                        this.ViewModel.CanvasScale *= 1.1f;
                        this.ViewModel.CanvasTransformer.Position = point + (this.ViewModel.CanvasTransformer.Position - point) * 1.1f;
                    }
                }
                else
                {
                    if (this.ViewModel.CanvasScale > 0.1f)
                    {
                        this.ViewModel.CanvasScale /= 1.1f;
                        this.ViewModel.CanvasTransformer.Position = point + (this.ViewModel.CanvasTransformer.Position - point) / 1.1f;
                    }
                }

                this.ViewModel.CanvasTransformer.ReloadMatrix();
                this.ViewModel.Invalidate();
            };
        }
    }
}
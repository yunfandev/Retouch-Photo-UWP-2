﻿using Microsoft.Graphics.Canvas;
using System.Numerics;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Retouch_Photo2.Tools
{
    /// <summary> 
    /// Operation Tools Class. 
    /// </summary>
    public abstract class Tool
    {

        /// <summary> <see cref = "Tool" />'s type. </summary>
        public ToolType Type { get; set; }
        /// <summary> <see cref = "Tool" />'s icon. </summary>
        public FrameworkElement Icon { get; set; }
        /// <summary> <see cref = "Tool" />'s icon2. </summary>
        public FrameworkElement ShowIcon { get; set; }
        /// <summary> <see cref = "Tool" />'s page. </summary>
        public Page Page { get; set; }


        //@Abstract
        /// <summary>
        /// Occurs the first time an action processor is created.
        /// </summary>
        /// <param name="point"> The pointer. </param>
        public abstract void Starting(Vector2 point);

        /// <summary>
        /// Occurs when the operation begins. 
        /// </summary>
        /// <param name="startingPoint"> The starting pointer. </param>
        /// <param name="point"> The pointer. </param>
        public abstract void Started(Vector2 startingPoint, Vector2 point);

        /// <summary>
        /// Occurs when the input device changes position during operation.
        /// </summary>
        /// <param name="startingPoint"> The starting pointer. </param>
        /// <param name="point"> The pointer. </param>
        public abstract void Delta(Vector2 startingPoint, Vector2 point);

        /// <summary>
        /// Occurs when the operation completes.
        /// </summary>
        /// <param name="startingPoint"> The starting pointer. </param>
        /// <param name="point"> The pointer. </param>
        /// <param name="isSingleStarted"> Whether the <see cref = "Tool.Started" /> method was triggered. </param>
        public abstract void Complete(Vector2 startingPoint, Vector2 point, bool isSingleStarted);

        /// <summary>
        /// Occurs when the canvas is drawn.
        /// </summary>
        /// <param name="ds"> Drawing session. </param>
        public abstract void Draw(CanvasDrawingSession ds);


        //@Virtual
        /// <summary> The current tool becomes the active tool. </summary>
        public virtual void ToolOnNavigatedTo() { }
        /// <summary> The current page does not become an active page. </summary>
        public virtual void ToolOnNavigatedFrom() { }
    }
}
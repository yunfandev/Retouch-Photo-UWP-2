﻿using System.Numerics;
using System.ComponentModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Input.Inking;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Controls;
using Windows.Foundation;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.UI.Xaml;
using Microsoft.Graphics.Canvas.Geometry;
using Retouch_Photo.Library;
using Retouch_Photo.Models;
using Retouch_Photo.Controls;
using Retouch_Photo.Pages.ToolPages;
using Retouch_Photo.Controls.ToolControls;
using Microsoft.Graphics.Canvas.Effects;
using Newtonsoft.Json;
using System;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Input;
using Retouch_Photo.ViewModels.ToolViewModels;
using Microsoft.Graphics.Canvas.UI;
using Windows.System;
using Windows.UI.Core;
using Retouch_Photo.Models.Tools;
using static Retouch_Photo.Library.TransformController;

namespace Retouch_Photo.ViewModels
{

    class CanvasControlManger
    {
        readonly CanvasControl CanvasControl;

        public CanvasControlManger(CanvasControl sender)=>  this.CanvasControl = sender;

        /// <summary> 获取或设置应用于此控件的 dpi 的缩放因子。 </summary>
        public float DpiScale
        {
            get => this.CanvasControl.DpiScale;
            set => this.CanvasControl.DpiScale=value;
        }

        /// <summary> 指示需要重新绘制载化管控制的内容。在不久之后引发的 "绘制" 事件中调用 "无效" 结果。 </summary>
        public void Invalidate()=> this.CanvasControl.Invalidate();
    }

    public class DrawViewModel : INotifyPropertyChanged
    {
        public Vector2 AAA;
        public Vector2 BBB;
        public Vector2 CCC;
        public Vector2 DDD;

        public Vector2 OOO;
        public int XXX;
        public int YYY;

        KeyViewModel KeyViewModel = new KeyViewModel();

        public DrawViewModel()
        {
            Window.Current.CoreWindow.KeyUp += (s, e) =>
            {
                this.KeyViewModel.KeyUp(s, e);
                this.KeyViewModel.KeyUpAndDown(s, e);
            };
            Window.Current.CoreWindow.KeyDown += (s, e) =>
            {
                this.KeyViewModel.KeyDown(s, e);
                this.KeyViewModel.KeyUpAndDown(s, e);
            };
        }

        #region CanvasControl



        /// <summary> 画布管理 </summary>
        CanvasControlManger CanvasControl;
        /// <summary> 画布设备 </summary>
        public CanvasDevice CanvasDevice { get; } = new CanvasDevice();
        /// <summary> 初始化 </summary>
        public void InitializeCanvasControl(CanvasControl sender) => this.CanvasControl=new CanvasControlManger(sender);

        /// <summary>
        ///  Indicates that the contents of the CanvasControl need to be redrawn. 
        ///  Calling Invalidate results in the Draw event being raised shortly afterward.
        /// </summary>
        /// <param name="isThumbnail"> draw thumbnails? </param>
        public void Invalidate(bool? isThumbnail = null)
        {
            if (this.CanvasControl == null) return;

            this.RenderLayer.RenderTarget = this.RenderLayer.GetRender
            (
                this.CanvasDevice,
                this.MatrixTransformer.CanvasToVirtualMatrix,
                this.MatrixTransformer.Width,
                this.MatrixTransformer.Height,
                this.MatrixTransformer.Scale
            );

            if (isThumbnail == true) this.CanvasControl.DpiScale = 0.5f;
            else if (isThumbnail == false) this.CanvasControl.DpiScale = 1.0f;

            this.CanvasControl.Invalidate();


            /*
                     Dpi标准为=96

                     我的Surface Book：
                     CanvasControl.Dpi = 240;
                     CanvasControl.DpiScale = 1;
                     CanvasControl.ConvertPixelsToDips(240) = 96;
                     CanvasControl.ConvertDipsToPixels(96, CanvasDpiRounding.Round) = 240;

                     修改DpiScale为=0.4后：
                     CanvasControl.Dpi = 96;
                     CanvasControl.DpiScale = 0.4;
                     CanvasControl.ConvertPixelsToDips(240) = 240;
                     CanvasControl.ConvertDipsToPixels(96, CanvasDpiRounding.Round) = 96;

                     可见
                     CanvasControl.DpiScale = 96 / CanvasControl.Dpi;
                     可以使DPI为标准的96，避免了位图的像素被缩放的问题
                   （比如，在高分辨率的设备上，100 * 100的位图可能占用更多比如240 * 240的像素）

                     在绘制之前，将DpiScale设为比1.0低的数，可以节省性能
                     （注：如果数字太小或太大会崩溃）
                     CanvasBitmap类在初始化时，它的DPI会和参数里的CanvasControl的DPI保持一致，请将它手动设为96.0f
                        */
        }

        public void InvalidateWithJumpedQueueLayer(Layer jumpedQueueLayer, bool? isThumbnail = null)
        {
            this.RenderLayer.RenderTarget = this.RenderLayer.GetRenderWithJumpedQueueLayer
            (
                this.CanvasDevice,
                jumpedQueueLayer,
                this.MatrixTransformer.CanvasToVirtualMatrix,
                this.MatrixTransformer.Width,
                this.MatrixTransformer.Height,
                this.MatrixTransformer.Scale
            );

            if (isThumbnail == true) this.CanvasControl.DpiScale = 0.5f;
            else if (isThumbnail == false) this.CanvasControl.DpiScale = 1.0f;

            this.CanvasControl.Invalidate();
        }
        

        #endregion


        /// <summary>重新加载ViewModel，可以多次调用</summary>
        /// <param name="project">Project类型</param>
        public void LoadFromProject(Project project)
        {
            if (project == null) return;

            this.MatrixTransformer.LoadFromProject(project);

            this.RenderLayer.LoadFromProject(this.CanvasDevice, project);
            this.RenderLayer.Layers.CollectionChanged += (ssender, e) => this.Invalidate();

            int index = (project.Tool >= Tool.ToolList.Count) || (project.Tool < 0) ? 0 : project.Tool;
            this.Tool = Tool.ToolList[index];
            ToolControl.SetIndex(index);
        }
               

        /// <summary>矩阵变换</summary>
        public MatrixTransformer MatrixTransformer = new MatrixTransformer();


        /// <summary>控件选定索引</summary>      
        public int SelectedIndex
        {
            get => this.RenderLayer.Index;
            set
            {
                this.RenderLayer.Index = value;
                OnPropertyChanged(nameof(SelectedIndex));
            }
        }
        /// <summary>当前图层</summary>     
        public Layer CurrentLayer
        {
            get
            {
                if (this.RenderLayer.Layers.Count == 0 || this.SelectedIndex == -1) return null;

                if (this.SelectedIndex >= 0 && this.SelectedIndex < this.RenderLayer.Layers.Count()) return this.RenderLayer.Layers[this.SelectedIndex];

                return null;
            }
            set
            {
                if (value == null ||
                    this.RenderLayer.Layers == null ||
                    this.RenderLayer.Layers.Count == 0)
                {
                    this.SelectedIndex = -1;
                    OnPropertyChanged(nameof(CurrentLayer));
                    return;
                }

                if (this.RenderLayer.Layers.Contains(value))
                {
                    this.SelectedIndex = this.RenderLayer.Layers.IndexOf(value);
                    OnPropertyChanged(nameof(CurrentLayer));
                    return;
                }

                this.SelectedIndex = -1;
                OnPropertyChanged(nameof(CurrentLayer));
                return;
            }
        }
        /// <summary>渲染图层</summary>
        public RenderLayer RenderLayer = new RenderLayer();
        /// <summary>标尺线</summary>   
        public bool IsRuler
        {
            get => this.RenderLayer.IsRuler;
            set
            {
                this.RenderLayer.IsRuler = value;
                OnPropertyChanged(nameof(IsRuler));
            }
        }


        #region Index & Tool
                

        /// <summary>颜色</summary>    
        private Color color = Color.FromArgb(255, 214, 214, 214);
        public Color Color
        {
            get => this.color;
            set
            {
                this.color = value;
                OnPropertyChanged(nameof(Color));
            }
        }

        /// <summary>变换</summary>    
        private Transformer transformer;
        public Transformer Transformer
        {
            get => this.transformer;
            set
            {
                this.transformer = value;
                OnPropertyChanged(nameof(Transformer));
            }
        }

        /// <summary>工具</summary>    
        private Tool tool = new NullTool();
        public Tool Tool
        {
            get => this.tool;
            set
            {
                this.tool.ViewModel.ToolOnNavigatedFrom();//当前页面不再成为活动页面
                value.ViewModel.ToolOnNavigatedTo();//当前页面成为活动页面

                this.tool = value;
                OnPropertyChanged(nameof(Tool));
            }
        }


        #endregion


        #region KeyBoard



        private bool keyShift;
        public bool KeyShift
        {
            get =>this.keyShift;
            set
            {
                this.keyShift = value;
                OnPropertyChanged(nameof(KeyShift));
            }
        }

        private bool keyCtrl;
        public bool KeyCtrl
        {
            get => this.keyCtrl;
            set
            {
                this.keyCtrl = value;
                OnPropertyChanged(nameof(KeyCtrl));
            }
        }

        private bool keyAlt;
        public bool KeyAlt
        {
            get => this.keyAlt;
            set
            {
                this.keyAlt = value;
                OnPropertyChanged(nameof(KeyAlt));
            }
        } 
        

        #endregion



        /// <summary>选框模式</summary>
        public MarqueeMode MarqueeMode
        {
            get => marqueeMode;
            set
            {
                if (marqueeMode == value) return;
                marqueeMode = value;
                OnPropertyChanged(nameof(MarqueeMode));
            }
        }
        private MarqueeMode marqueeMode = MarqueeMode.None;

        /// <summary> 文本 </summary>      
        public string Text
        {
            get => text;
            set
            {
                text = value;
                OnPropertyChanged(nameof(Text));
            }
        }
        private string text;



        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name) => this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}

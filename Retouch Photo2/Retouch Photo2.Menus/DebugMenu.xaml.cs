﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Retouch_Photo2.Elements;
using Retouch_Photo2.ViewModels;
using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Retouch_Photo2.Layers;

namespace Retouch_Photo2.Menus.Models
{
    public sealed partial class DebugMenu : UserControl, IMenu
    {
        //@ViewModel
        ViewModel ViewModel => App.ViewModel;
        ViewModel SelectionViewModel => App.SelectionViewModel;
        ViewModel MethodViewModel => App.MethodViewModel;

        //@Construct
        public DebugMenu()
        {
            this.InitializeComponent();
            this.ConstructStrings();
            this.ConstructMenu();

            this.Button.Click += (s, e) =>
            {
                this.ItemsControl.ItemsSource = sadasd(this.ViewModel.LayerageCollection.RootLayerages, 0);
            };
            this.Button2.Click += (s, e) =>
            {
                this.ItemsControl.ItemsSource = asdsadasdsdsssss();
            };




            this.Re.Click += (s, e) =>
            {
                List<string> sadas = new List<string>();

                if (this.SelectionViewModel.SelectionLayerage != null)
                {
                    if (this.SelectionViewModel.SelectionLayerage.Id != null)
                    {
                        ILayer layer2 = this.SelectionViewModel.SelectionLayerage.Self;
                        string sssss = $"   id:{layer2.Id} {this.SelectionViewModel.SelectionLayerage.Parents == null}";

                        sadas.Add(sssss);
                    }
                }

                this.ItemsControl.ItemsSource = sadas;

            };
            this.Coo.Click += (s, e) =>
            {
                List<string> sadas = new List<string>();

                if (this.SelectionViewModel.SelectionLayerages != null)
                {
                    foreach (var item in this.SelectionViewModel.SelectionLayerages)
                    {
                        ILayer layer2 = item.Self;
                        string sssss = $"   id:{layer2.Id} {item.Parents == null}";

                        sadas.Add(sssss);
                    }
                }

                this.ItemsControl.ItemsSource = sadas;
            };
        }
        IEnumerable<Layerage> previous;

        private IEnumerable<string> asdsadasdsdsssss()
        {
            foreach (var layer in LayerBase.Instances)
            {
                ILayer layer2 = layer;
                yield return $"   {layer2.Id}";
            }
        }

        private IEnumerable<string> sadasd(IList<Layerage> layers, int depht)
        {

            foreach (var layer in layers)
            {
                ILayer layer2 = layer.Self;
                string sadas = $"de:{depht}  id:{layer2.Id} {layer2.IsSelected}";

                if (layer.Parents != null)
                {
                    sadas += $"  pa:{layer.Parents.Id}";
                }
                foreach (var sss in layer.Children)
                {
                    sadas += $"  {sss.Id}";
                }
                yield return sadas;
                foreach (var child in sadasd(layer.Children, depht + 1))
                {
                    yield return child;
                }
            }
        }
    }

    public sealed partial class DebugMenu : UserControl, IMenu
    {
        //Strings
        private void ConstructStrings()
        {
            ResourceLoader resource = ResourceLoader.GetForCurrentView();

            this._button.ToolTip.Content =
            this._Expander.Title =
            this._Expander.CurrentTitle = "Debug"; //resource.GetString("/Menus/Debug");
        }

        //Menu
        public MenuType Type => MenuType.Debug;
        public IExpander Expander => this._Expander;
        MenuButton _button = new MenuButton
        {
            CenterContent = "?"
        };

        public void ConstructMenu()
        {
            this._Expander.Layout = this;
            this._Expander.Button = this._button;
            this._Expander.Initialize();
        }
    }
}
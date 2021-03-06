﻿using FanKit.Transformers;
using Microsoft.Graphics.Canvas;
using Retouch_Photo2.Elements;
using Retouch_Photo2.Historys;
using Retouch_Photo2.Layers;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources;
using Windows.Graphics.Imaging;
using Windows.UI.Xaml.Controls;

namespace Retouch_Photo2
{
    /// <summary> 
    /// Represents a page used to draw vector graphics.
    /// </summary>
    public sealed partial class DrawPage : Page
    {

        //Strings
        private void ConstructStrings()
        {
            ResourceLoader resource = ResourceLoader.GetForCurrentView();

            this.DrawLayout.RightPhotosToolTip.Content = resource.GetString("/$DrawPage/Page_Photos");
            this.DrawLayout.RightWidthToolTip.Content = resource.GetString("/$DrawPage/Page_Width");

            this.SetupDialog.Title = resource.GetString("/$DrawPage/SetupDialog_Title");
            this.SetupDialog.CloseButton.Content = resource.GetString("/$DrawPage/SetupDialog_Close");
            this.SetupDialog.PrimaryButton.Content = resource.GetString("/$DrawPage/SetupDialog_Primary");
            this.SetupSizePicker.WidthText = resource.GetString("/$DrawPage/SetupSizePicker_Width");
            this.SetupSizePicker.HeightText = resource.GetString("/$DrawPage/SetupSizePicker_Height");
            this.SetupLayersTextBlock.Text = resource.GetString("/$DrawPage/SetupDialog_Layers");
            this.SetupResizeButton.Content = resource.GetString("/$DrawPage/SetupDialog_Resize");
            this.SetupAnchorButton.Content = resource.GetString("/$DrawPage/SetupDialog_Anchor");
            
            this.ExportDialog.Title = resource.GetString("/$DrawPage/ExportDialog_Title");
            this.ExportDialog.CloseButton.Content = resource.GetString("/$DrawPage/ExportDialog_Close");
            this.ExportDialog.PrimaryButton.Content = resource.GetString("/$DrawPage/ExportDialog_Primary");
            this.ExportSizePicker.WidthText = resource.GetString("/$DrawPage/ExportSizePicker_Width");
            this.ExportSizePicker.HeightText = resource.GetString("/$DrawPage/ExportSizePicker_Height");
            this.ExportQualityTextBlock.Text = resource.GetString("/$DrawPage/ExportDialog_Quality");
            this.ExportFileFormatTextBlock.Text = resource.GetString("/$DrawPage/ExportDialog_FileFormat"); 

            this.RenameDialog.Title = resource.GetString("/$DrawPage/RenameDialog_Title");
            this.RenameTextBox.PlaceholderText = resource.GetString("/$DrawPage/RenameDialog_PlaceholderText");
            this.RenameDialog.CloseButton.Content = resource.GetString("/$DrawPage/RenameDialog_Close");
            this.RenameDialog.PrimaryButton.Content = resource.GetString("/$DrawPage/RenameDialog_Primary");
        }


        //Export
        public float ExportQuality
        {
            get => this.exportQuality;
            set
            {
                this.ExportQualityPicker.Value = (int)(value * 100.0f);
                this.ExportQualitySlider.Value = value;
                this.exportQuality = value;
            }
        }
        public float exportQuality = 1.0f;

        private void ConstructExportDialog()
        {
            this.DPIComboBox.DPI = DPI.DPI144;
            this.FileFormatComboBox.FileFormat = CanvasBitmapFileFormat.Jpeg;


            this.ExportQualityPicker.Unit = "%";
            this.ExportQualityPicker.Minimum = 0;
            this.ExportQualityPicker.Maximum = 100;
            this.ExportQualityPicker.ValueChanged += (s, value) => this.ExportQualitySlider.Value = value / 100.0f;


            this.ExportQualitySlider.Minimum = 0.0d;
            this.ExportQualitySlider.Maximum = 1.0d;
            this.ExportQualitySlider.ValueChangeStarted += (s, value) => this.ExportQualityPicker.Value = (int)(value * 100.0f);
            this.ExportQualitySlider.ValueChangeDelta += (s, value) => this.ExportQualityPicker.Value = (int)(value * 100.0f);
            this.ExportQualitySlider.ValueChangeCompleted += (s, value) => this.ExportQualityPicker.Value = (int)(value * 100.0f);


            this.ExportDialog.CloseButton.Click += (sender, args) => this.ExportDialog.Hide();


            this.ExportDialog.PrimaryButton.Click += async (_, __) =>
            {
                this.ExportDialog.Hide();

                this.LoadingControl.State = LoadingState.Saving;
                this.LoadingControl.IsActive = true;

                bool isSuccesful = await this.Export();

                this.LoadingControl.State = isSuccesful ? LoadingState.SaveSuccess : LoadingState.SaveFailed;
                await Task.Delay(400);

                this.LoadingControl.State = LoadingState.None;
                this.LoadingControl.IsActive = false;
            };
        }
        private void ShowExportDialog()
        {
            BitmapSize size = new BitmapSize
            {
                Width = (uint)this.ViewModel.CanvasTransformer.Width,
                Height = (uint)this.ViewModel.CanvasTransformer.Height,
            };
            this.ExportSizePicker.Size = size;
            this.ExportQuality = ExportQuality;
            this.ExportDialog.Show();
        }


        //Setup
        private void ConstructSetupDialog()
        {
            this.SetupResizeButton.IsEnabled = false;
            this.SetupAnchorButton.IsEnabled = true;
            this.SetupIndicatorControl.Mode = IndicatorMode.None;

            this.SetupResizeButton.Click += (sender, args) =>
            {
                this.SetupResizeButton.IsEnabled = false;
                this.SetupAnchorButton.IsEnabled = true;
                this.SetupIndicatorControl.Mode = IndicatorMode.None;
            };
            this.SetupAnchorButton.Click += (sender, args) =>
            {
                this.SetupResizeButton.IsEnabled = true;
                this.SetupAnchorButton.IsEnabled = false;
                this.SetupIndicatorControl.Mode = IndicatorMode.LeftTop;
            };


            this.SetupDialog.CloseButton.Click += (sender, args) => this.SetupDialog.Hide();

            this.SetupDialog.PrimaryButton.Click += (_, __) =>
            {
                this.SetupDialog.Hide();

                BitmapSize size = this.SetupSizePicker.Size;
                IndicatorMode mode = this.SetupIndicatorControl.Mode;

                if (mode== IndicatorMode.None)
                    this.MethodViewModel.MethodSetup(size);
                else
                    this.MethodViewModel.MethodSetup(size, mode);
            };
        }
        private void ShowSetupDialog()
        {
            BitmapSize size = new BitmapSize
            {
                Width = (uint)this.ViewModel.CanvasTransformer.Width,
                Height = (uint)this.ViewModel.CanvasTransformer.Height,
            };
            this.SetupSizePicker.Size = size;

            this.SetupDialog.Show();
        }


        //Rename
        private void ConstructRenameDialog()
        {
            //@Focus
            // Before Flyout Showed, Don't let TextBox Got Focus.
            // After TextBox Gots focus, disable Shortcuts in SettingViewModel.
            if (this.RenameTextBox is TextBox textBox)
            {
                //textBox.IsEnabled = false;
                //this.ColorFlyout.Opened += (s, e) => textBox.IsEnabled = true;
                //this.ColorFlyout.Closed += (s, e) => textBox.IsEnabled = false;
                textBox.GotFocus += (s, e) => this.SettingViewModel.KeyIsEnabled = false;
                textBox.LostFocus += (s, e) => this.SettingViewModel.KeyIsEnabled = true;
            }

            this.RenameDialog.CloseButton.Click += (sender, args) => this.RenameDialog.Hide();
            this.RenameDialog.PrimaryButton.Click += (_, __) =>
            {
                this.RenameDialog.Hide();
                string name = this.RenameTextBox.Text;

                //History
                LayersPropertyHistory history = new LayersPropertyHistory("Set name");
                
                //Selection
                this.SelectionViewModel.LayerName = name;
                this.SelectionViewModel.SetValue((layerage) =>
                {
                    ILayer layer = layerage.Self;

                    if (layer.Name != name)
                    {
                        //History
                        var previous = layer.Name;
                        history.UndoAction += () =>
                        {
                            layer.Name = previous;
                        };

                        layer.Name = name;
                    }
                });

                //History
                this.ViewModel.HistoryPush(history);
            };
        }
        private void ShowRenameDialog()
        {
            this.RenameTextBox.Text = this.SelectionViewModel.LayerName;

            this.RenameDialog.Show();
        }        

    }
}
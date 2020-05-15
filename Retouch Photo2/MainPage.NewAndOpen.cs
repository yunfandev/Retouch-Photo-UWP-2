﻿using FanKit.Transformers;
using Microsoft.Graphics.Canvas;
using Retouch_Photo2.Brushs;
using Retouch_Photo2.Elements;
using Retouch_Photo2.Elements.MainPages;
using Retouch_Photo2.Layers;
using Retouch_Photo2.Layers.Models;
using Retouch_Photo2.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Retouch_Photo2
{
    /// <summary> 
    /// Retouch_Photo2's the only <see cref = "MainPage" />. 
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private string Untitled = "Untitled";

        /// <summary>
        /// New from size.
        /// </summary>
        /// <param name="pixels"> The bitmap size. </param>
        private void NewFromSize(BitmapSize pixels)
        {
            this.LoadingControl.IsActive = true;

            //Project
            {
                string untitled = this.Untitled;
                string name = this.UntitledRenameByRecursive(untitled);
                int width = (int)pixels.Width;
                int height = (int)pixels.Height;

                Project project = new Project
                {
                    Name = name,
                    Width = width,
                    Height = height,
                };
                this.ViewModel.LoadFromProject(project);
            }

            //Transition
            TransitionData data = new TransitionData
            {
                Type = TransitionType.Size
            };

            this.LoadingControl.IsActive = false;
            this.Frame.Navigate(typeof(DrawPage), data);//Navigate
        }

        /// <summary>
        /// Open from ProjectViewItem.
        /// </summary>
        /// <param name="projectViewItem"> The ProjectViewItem. </param>
        private async void OpenFromProjectViewItem(ProjectViewItem projectViewItem)
        {
            this.LoadingControl.IsActive = true;

            //FileUtil
            {
                string name = projectViewItem.Name;
                if (name == null) return;
                if (name == string.Empty) return;

                await FileUtil.DeleteInTemporaryFolder();
                await FileUtil.MoveAll(name);

                //Load all photos. 
                IEnumerable<Photo> photos = XML.LoadPhotosFile();
                Photo.Instances.Clear();
                foreach (Photo photo in photos)
                {
                    await photo.ConstructPhotoSource(this.ViewModel.CanvasDevice);
                    Photo.Instances.Add(photo);
                }
            }

            //Project
            {
                string name = projectViewItem.Name;
                Project project = XML.LoadProjectFile(name);
                this.ViewModel.LoadFromProject(project);
            }

            //Transition
            TransitionData data = new TransitionData
            {
                Type = TransitionType.Transition,
                SourceRect = projectViewItem.GetVisualRect(Window.Current.Content),
                PageSize = new Size(this.ActualWidth, this.ActualHeight - 50)
            };

            this.LoadingControl.IsActive = false;
            this.Frame.Navigate(typeof(DrawPage), data);//Navigate   
        }


        /// <summary>
        /// New from Picture.
        /// </summary>
        /// <param name="pixels"> The picker locationId. </param>
        private async Task NewFromPicture(PickerLocationId location)
        {
            this.LoadingControl.IsActive = true;

            //Photo
            StorageFile copyFile = await FileUtil.PickAndCopySingleImageFileAsync(location);
            Photo photo = await Photo.CreatePhotoFromCopyFileAsync(this.ViewModel.CanvasDevice, copyFile);
            Photo.DuplicateChecking(photo);

            //Transformer
            string name = this.UntitledRenameByRecursive($"{photo.Name}");
            int width = (int)photo.Width;
            int height = (int)photo.Height;
            Transformer transformerSource = new Transformer(width, height, Vector2.Zero);

            //ImageLayer 
            Photocopier photocopier = photo.ToPhotocopier();
            ImageLayer imageLayer = new ImageLayer(transformerSource, photocopier);

            //Project
            {
                Project project = new Project
                {
                    Name = name,
                    Width = width,
                    Height = height,
                    Layers = new List<ILayer>
                    {
                         imageLayer
                    }
                };
                this.ViewModel.LoadFromProject(project);
            }

            //Transition
            TransitionData data = new TransitionData
            {
                Type = TransitionType.Size
            };

            this.LoadingControl.IsActive = false;
            this.Frame.Navigate(typeof(DrawPage), data);//Navigate
        }


        /// <summary>
        /// Get a name that doesn't have a rename.
        /// If there are, add the number.
        /// [Untitled] --> [Untitled1]   
        /// </summary>
        /// <param name="name"> The previous name. </param>
        /// <returns> The new name. </returns>
        private string UntitledRenameByRecursive(string name)
        {
            // Is there a re-named item?
            if (this.ProjectViewItems.All(i => i.Name != name))
                return name;

            int num = 0;
            string newName;

            do
            {
                num++;
                newName = $"{name}{num}";
            }
            // Is there a re-named item?
            while (this.ProjectViewItems.Any(i => i.Name == newName));

            return newName;
        }

    }
}
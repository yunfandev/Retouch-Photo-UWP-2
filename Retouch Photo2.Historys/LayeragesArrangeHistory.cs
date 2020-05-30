﻿using Retouch_Photo2.Layers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Retouch_Photo2.Historys
{
    /// <summary>
    /// Represents a history used to change the order in the arrangement of layerages.
    /// </summary>
    public class LayeragesArrangeHistory : HistoryBase, IHistory
    {
        public Action UndoAction { get; set; }
        public IList<Layerage> Layerages { get; private set; } = new List<Layerage>();

        //@Construct
        /// <summary>
        /// Initializes a LayeragesArrangeHistory.
        /// </summary>
        /// <param name="title"> The title. </param>  
        /// <param name="layerageCollection"> The layerage-collection. </param>  
        public LayeragesArrangeHistory(string title, LayerageCollection layerageCollection)
        {
            base.Title = title;

            foreach (Layerage  layerage in layerageCollection.RootLayerages)
            {
                this.Layerages.Add(layerage.Clone());
            }

            this.UndoAction = () =>
            {
                layerageCollection.RootLayerages.Clear();
                foreach (Layerage layerage in this.Layerages)
                {
                    layerageCollection.RootLayerages.Add(layerage.Clone());
                }
            };
        }

        public override void Undo()
        {
            this.UndoAction?.Invoke();
        }
    }
}
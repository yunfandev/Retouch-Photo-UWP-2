﻿using Retouch_Photo2.Layers;
using System.Collections.ObjectModel;

namespace Retouch_Photo2.TestApp.ViewModels
{
    /// <summary> 
    /// Retouch_Photo2's the only <see cref = "ViewModel" />. 
    /// </summary>
    public class MezzanineViewModel
    {
        /// <summary> If the layer is not **null**, insert it between layers. </summary>
        public Layer Layer { get; private set; }

        /// <summary> Index of the <see cref="Mezzanine.Layer"/>. </summary>
        public int Index { get; private set; }


        /// <summary>
        /// Sets the <see cref="Mezzanine.Layer"/>. 
        /// </summary>
        /// <param name="layer"> The source layer. </param>
        /// <param name="layers"> The destination layers. </param>
        public void SetLayer(Layer layer, Collection<Layer> layers)
        {
            this.Layer = layer;
            this.Index = this.GetfFrstIndex(layers);
        }


        /// <summary>
        /// Find the index of the first checked layer from the layers.
        /// </summary>
        /// <param name="layers"></param>
        /// <returns></returns>
        public int GetfFrstIndex(Collection<Layer> layers)
        {
            for (int i = 0; i < layers.Count; i++)
            {
                if (layers[i].IsChecked) return i;
            }
            return 0;
        }


        /// <summary>
        /// Turn off <see cref="Mezzanine.Layer"/>.
        /// </summary>
        public void None()
        {
            this.Layer = null;
            this.Index = -1;
        }


        /// <summary>
        /// Insert a layer into the layers.
        /// </summary>
        /// <param name="layer"> The source layer. </param>
        /// <param name="layers"> The destination layers. </param>
        public void Insert(Layer layer, Collection<Layer> layers)
        {
            int index = this.Index - 1;
            if (index < 0) index = 0;

            layers.Insert(index, layer);

            this.None();
        }

    }
}
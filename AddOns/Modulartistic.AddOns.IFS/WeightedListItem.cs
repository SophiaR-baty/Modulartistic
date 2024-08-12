using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modulartistic.AddOns.IFS
{
    class WeightedListItem<T>
    {
        public T Item { get; set; }
        public float Weight { get; set; }

        public WeightedListItem(T item, float weight)
        {
            Item = item;
            Weight = weight;
        }
    }
}

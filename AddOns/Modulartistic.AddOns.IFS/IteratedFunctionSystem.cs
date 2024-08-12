using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Modulartistic.AddOns.IFS
{
    internal class IteratedFunctionSystem
    {
        public List<WeightedListItem<Func<Complex, Complex>>> Functions;
        public List<Complex> Points;
        public Complex CurrentPoint;

        private Random _random;
        private Lazy<double> _totalWeight;
        
        public IteratedFunctionSystem()
        {
            _random = new Random();
            _totalWeight = new Lazy<double>(CalculateTotalWeight);
            Functions = new List<WeightedListItem<Func<Complex, Complex>>>();
            Points = new List<Complex>();
            CurrentPoint = new Complex();
    }

        private double CalculateTotalWeight()
        {
            double totalWeight = 0;
            foreach (var weightedItem in Functions)
            {
                totalWeight += weightedItem.Weight;
            }

            return totalWeight;
        }


        public Complex Next()
        {
            double randomValue = _random.NextDouble() * _totalWeight.Value;

            Func<Complex, Complex>? func = null;
            foreach (var weightedItem in Functions)
            {
                if (randomValue < weightedItem.Weight)
                {
                    func = weightedItem.Item;
                    break;
                }
                randomValue -= weightedItem.Weight;
            }
            if (func is null)
            {
                func = Functions[Functions.Count - 1].Item;
            }

            return CurrentPoint = func(CurrentPoint);
        }

        public void GeneratePoints(int depth) 
        {
            Points.Add(CurrentPoint);
            for (int i = 0; i < depth; i++)
            { 
                Points.Add(Next());
            }
        }
    }
}

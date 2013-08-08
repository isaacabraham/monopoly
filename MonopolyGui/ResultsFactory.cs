using System;
using System.Collections.Generic;

namespace MonopolyGui
{
    public class ResultsFactory
    {
        public static Dictionary<String, PositionCounter> CreateResultsDictionary()
        {
            var output = new Dictionary<String, PositionCounter>();
            foreach (var item in MonopolyData.Board)
                output[Controller.getName(item)] = new PositionCounter(item);
            return output;
        }
    }
}

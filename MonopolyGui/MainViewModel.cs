using System;
using System.Collections.Generic;

namespace MonopolyGui
{
    public class MainViewModel
    {
        public AutoPlayCommand AutoPlayCommand { get; set; }
        public StepByStepCommand StepByStepCommand { get; set; }
        public IDictionary<String, PositionCounter> Results { get; set; }

        public MainViewModel()
        {
            Results = ResultsFactory.CreateResultsDictionary();
            AutoPlayCommand = new AutoPlayCommand(Results);
            StepByStepCommand = new StepByStepCommand(Results);
        }
    }
}
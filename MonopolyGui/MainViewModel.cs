using System;
using System.Collections.Generic;
using System.Linq;

namespace MonopolyGui
{
    public class MainViewModel
    {
        public AutoPlayCommand AutoPlayCommand { get; set; }
        public StepByStepCommand StepByStepCommand { get; set; }
        public IDictionary<String, BoardPosition> Results { get; set; }

        public MainViewModel()
        {
            Results = MonopolyData.Board.ToDictionary(pos => Controller.getName(pos),
                                                      pos => new BoardPosition(pos));
            AutoPlayCommand = new AutoPlayCommand(Results);
            StepByStepCommand = new StepByStepCommand(Results);
        }
    }
}
using Monopoly;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MonopolyGui
{
    public class MainViewModel
    {
        public AutoPlayCommand AutoPlayCommand { get; set; }
        public StepByStepCommand StepByStepCommand { get; set; }
        public IDictionary<String, BoardPosition> PositionLookup { get; set; }

        public MainViewModel()
        {
            PositionLookup = Data.Board.ToDictionary(pos => pos.ToString(), pos => new BoardPosition(pos));
            AutoPlayCommand = new AutoPlayCommand(PositionLookup);
            StepByStepCommand = new StepByStepCommand(PositionLookup);
        }
    }
}
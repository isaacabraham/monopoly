using Monopoly;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MonopolyGui
{
    /// <summary>
    /// Plays a large number of moves for a game and replays them.
    /// </summary>
    public class AutoPlayCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        private readonly IDictionary<String, BoardPosition> results;
        public AutoPlayCommand(IDictionary<String, BoardPosition> results)
        {
            this.results = results;
        }

        public bool CanExecute(Object parameter) { return true; }
        public void Execute(Object parameter)
        {
            var controller = new Controller();
            controller.OnMoved += (o, e) => results[e.MovementData.Destination.ToString()].Increment();
            Task.Run(() => controller.PlayGame(50000));
        }
    }
}

using Microsoft.FSharp.Core;
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
            var onPrint = FuncConvert.ToFSharpFunc<Controller.State, Unit>(s =>
            {
                var name = Controller.getName(s.movingTo);
                results[name].Increment();
                return null;
            });

            Task.Run(() => Controller.playGame(50000, onPrint));
        }
    }
}

using Microsoft.FSharp.Core;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MonopolyGui
{
    /// <summary>
    /// Plays a game one move at a time.
    /// </summary>
    [ImplementPropertyChanged]
    public class StepByStepCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        private List<Controller.State>.Enumerator gameHistory;
        private readonly IDictionary<String, BoardPosition> results;
        public String Message { get; set; }
        
        public Boolean CanExecute(Object parameter) { return true; }
        public StepByStepCommand(IDictionary<String, BoardPosition> results)
        {
            this.results = results;
            var auditRecord = new List<Controller.State>();
            var onPrint = FuncConvert.ToFSharpFunc<Controller.State, Unit>(s =>
            {
                auditRecord.Add(s);
                return null;
            });

            Task.Run(() => Controller.playGame(5000, onPrint))
                .ContinueWith(t => { gameHistory = auditRecord.GetEnumerator(); });
        }

        public void Execute(Object parameter)
        {

            DeselectCurrentPosition(gameHistory.Current);
            gameHistory.MoveNext();

            var state = gameHistory.Current;            
            var movingToName = Controller.getName(state.movingTo);
            
            Message = String.Format("Rolled {0} & {1}. {2} {3} (Doubles: {4})", state.rolled.Item1, state.rolled.Item2, state.movementType, movingToName, state.doubleCount);
            SelectNextPosition(movingToName);
        }
        
        private void DeselectCurrentPosition(Controller.State state)
        {
            var gameHasStarted = (state != null);
            if (gameHasStarted)
                results[Controller.getName(state.movingTo)].Deselect();
        }
        private void SelectNextPosition(String movingToName)
        {
            var result = results[movingToName];
            result.Increment();
            result.Select();
        }
    }
}

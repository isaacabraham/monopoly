using Microsoft.FSharp.Core;
using Monopoly;
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

        private List<MovementEvent>.Enumerator gameHistory;
        private readonly IDictionary<String, BoardPosition> results;
        public string Message { get; set; }

        public bool CanExecute(object parameter) { return true; }
        public StepByStepCommand(IDictionary<String, BoardPosition> results)
        {
            this.results = results;

            var auditRecord = new List<MovementEvent>();
            var controller = new Controller();
            controller.OnMoved += (o, e) => auditRecord.Add(e);

            Task.Run(() => controller.PlayGame(5000))
                .ContinueWith(t => { gameHistory = auditRecord.GetEnumerator(); });
        }

        public void Execute(object parameter)
        {
            DeselectCurrentPosition(gameHistory.Current);
            gameHistory.MoveNext();

            var movingToName = gameHistory.Current.MovementData.Destination.ToString();

            // "Pattern matching" over a discriminated union in C#.
            // If MovementEvent is of type LandedOn, cast object and work with it.
            if (gameHistory.Current.IsLandedOn)
            {
                var landedOn = (MovementEvent.LandedOn)gameHistory.Current;
                Message = string.Format("Rolled {0} & {1}. Landed on {2} (Doubles: {3})", landedOn.Item2.Item1, landedOn.Item2.Item2, movingToName, landedOn.Item1.DoubleCount);
            }
            else
                Message = string.Format("Moved to {0}", movingToName);

            SelectNextPosition(movingToName);
        }

        private void DeselectCurrentPosition(MovementEvent state)
        {
            var gameHasStarted = (state != null);
            if (gameHasStarted)
                results[state.MovementData.Destination.ToString()].Deselect();
        }
        private void SelectNextPosition(string movingToName)
        {
            var result = results[movingToName];
            result.Increment();
            result.Select();
        }
    }

    public static class FSOptionEx
    {
        public static bool IsSome<T>(this FSharpOption<T> option)
        {
            return FSharpOption<T>.get_IsSome(option);
        }
    }
}
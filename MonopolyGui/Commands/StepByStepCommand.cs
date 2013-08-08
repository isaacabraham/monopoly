using Microsoft.FSharp.Core;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MonopolyGui
{
    [ImplementPropertyChanged]
    public class StepByStepCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        private List<Controller.State>.Enumerator replay;
        private readonly IDictionary<String, PositionCounter> results;
        public String Message { get; set; }
        
        public Boolean CanExecute(Object parameter) { return true; }
        public StepByStepCommand(IDictionary<String, PositionCounter> results)
        {
            this.results = results;
            var auditRecord = new List<Controller.State>();
            var onPrint = FuncConvert.ToFSharpFunc<Controller.State, Unit>(s =>
            {
                auditRecord.Add(s);
                return null;
            });

            Task.Run(() => Controller.playGame(5000, onPrint))
                .ContinueWith(t => { replay = auditRecord.GetEnumerator(); });
        }

        public void Execute(Object parameter)
        {
            if (replay.Current != null)
                results[Controller.getName(replay.Current.movingTo)].Deselect();
            
            replay.MoveNext();
            var movingToName = Controller.getName(replay.Current.movingTo);
            var result = results[movingToName];
            Message = String.Format("Rolled {0} & {1}. {2} {3} (Doubles: {4})", replay.Current.rolled.Item1, replay.Current.rolled.Item2, replay.Current.movementType, movingToName, replay.Current.doubleCount);
            result.Increment();
            result.Select();
        }
    }
}

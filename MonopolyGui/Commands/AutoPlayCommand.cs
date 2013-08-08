using Microsoft.FSharp.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;

namespace MonopolyGui
{
    public class AutoPlayCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        private readonly IDictionary<String, PositionCounter> results;
        public AutoPlayCommand(IDictionary<String, PositionCounter> results)
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
            //    .ContinueWith(t =>
            //    {
            //        var sets = results
            //                        .Select(r => new { r.Value.Position, r.Value.Count })
            //                        .Where(r => r.Position is Monopoly.Position.Property)
            //                        .Select(p => new { Position = (Monopoly.Position.Property)p.Position, p.Count })
            //                        .Select(p => new { typeof(Monopoly.Set.Tags).GetFields().First(f => (Int32)f.GetValue(null) == p.Position.Item1.Tag).Name, p.Count })
            //                        .Select(p => new { Brush = typeof(Brushes).GetProperty(p.Name).GetValue(typeof(Brushes), null), p.Name, p.Count })
            //                        .GroupBy(p => new { p.Name, p.Brush } )
            //                        .Select(p => new { Count = p.Sum(x => x.Count), p.Key.Brush, p.Key.Name })
            //                        .ToArray();
            //        Console.WriteLine(sets.Count());
            //    });
        }
    }
}

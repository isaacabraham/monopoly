using PropertyChanged;
using System;
using System.Windows.Media;

namespace MonopolyGui
{
    [ImplementPropertyChanged]
    public class PositionCounter
    {
        public Monopoly.Position Position { get; private set; }
        public Int32 Count { get; private set; }
        public Boolean Selected { get; private set; }
        public Brush Background { get; private set; }

        public PositionCounter(Monopoly.Position position)
        {
            Position = position;
        }

        public void Increment() { Count++; }
        public void Select()
        {
            Selected = true;
            Background = new SolidColorBrush(Colors.Green);
            Background.Opacity = .5;
        }
        public void Deselect() { Selected = false; Background = Brushes.Transparent; }
    }
}

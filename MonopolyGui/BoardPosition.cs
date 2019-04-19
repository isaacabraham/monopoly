using Monopoly;
using System;
using System.Windows.Media;

namespace MonopolyGui
{
    /// <summary>
    /// Represents a position on the monopoly board.
    /// </summary>
    public class BoardPosition : ViewModelBase
    {
        /// <summary>
        /// The actual position data.
        /// </summary>
        public Position Position { get; private set; }
        /// <summary>
        /// The number of times that this property has been landed on.
        /// </summary>
        public Int32 Count { get; private set; }
        /// <summary>
        /// Whether or not this property is "selected" i.e. currently landed on.
        /// </summary>
        public Boolean Selected { get; private set; }
        /// <summary>
        /// The background brush of the position.
        /// </summary>
        public Brush Background { get; private set; }

        public BoardPosition(Position position)
        {
            Position = position;
        }

        /// <summary>
        /// Increments the count by one.
        /// </summary>
        public void Increment() { Count++; }
        /// <summary>
        /// Selects the position.
        /// </summary>
        public void Select()
        {
            Selected = true;
            Background = new SolidColorBrush(Colors.Green);
            Background.Opacity = .5;
        }
        /// <summary>
        /// Deselects the position.
        /// </summary>
        public void Deselect()
        {
            Selected = false;
            Background = Brushes.Transparent;
        }
    }
}

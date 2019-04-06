using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace UI.MVVM.ViewModel
{
    /// <summary>
    /// Abstrakte Basisklasse zum Bereitstellen des Interfaces INotifyPropertyChanged
    /// </summary>
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        //[NotifyPropertyChangedInvocator] - ReSharper attribute https://stackoverflow.com/questions/23213146/what-is-notifypropertychangedinvocator-in-c-sharp-when-implements-inotifyprope
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        // Alternativ bzw ergänzend aus Kühnel, C#6 mit Visual Studio 2015
        /// <summary>
        /// Generic method, stores new property value in an intended private  field and fires event PropertyChanged
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="storage"></param>
        /// <param name="Value"></param>
        /// <param name="property"></param>
        protected void SetProperty<T>(
            ref T storage,
            T Value,
            [CallerMemberName] string property = null)
        {
            if (Equals(storage, Value))
            {
                return;
            }

            storage = Value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }

        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
            => PropertyChanged?.Invoke(this, e);
    }
}

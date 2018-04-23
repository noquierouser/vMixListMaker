using System;
using System.ComponentModel;

namespace vMixListMaker
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private TimeSpan _startingTime = new TimeSpan(17, 45, 00);

        public TimeSpan StartingTime
        {
            get => _startingTime;
            set
            {
                if (_startingTime != value)
                {
                    _startingTime = value;
                    OnPropertyChanged("StartingTime");
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string prop) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
    }
}

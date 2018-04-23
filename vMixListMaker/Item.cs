using System;
using System.ComponentModel;

namespace vMixListMaker
{
    public class Item : INotifyPropertyChanged
    {
        private DateTime _startTime;
        private DateTime _endTime;

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string prop) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));

        public Guid Id { get; set; }

        public string FileName { get; set; }

        public string FilePath { get; set; }

        public TimeSpan FileDuration { get; set; }

        public DateTime StartTime
        {
            get => _startTime;

            set
            {
                if (_startTime != value)
                {
                    _startTime = value;
                    OnPropertyChanged("StartTime");
                }
            }
        }

        public DateTime EndTime
        {
            get => _endTime;

            set
            {
                if (_endTime != value)
                {
                    _endTime = value;
                    OnPropertyChanged("EndTime");
                }
            }
        }
    }
}

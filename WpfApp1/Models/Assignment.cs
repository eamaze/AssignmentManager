using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace WpfApp1.Models
{
    public class Assignment : INotifyPropertyChanged
    {
        private int _id;
        private string _name;
        private string _description;
        private DateTime _dueDate;
        private TimeSpan _dueTime;
        private string _lectureSection;
        private string _submissionUrl;

        public int Id
        {
            get { return _id; }
            set { SetProperty(ref _id, value); }
        }

        public string Name
        {
            get { return _name; }
            set { SetProperty(ref _name, value); }
        }

        public string Description
        {
            get { return _description; }
            set { SetProperty(ref _description, value); }
        }

        public DateTime DueDate
        {
            get { return _dueDate; }
            set { SetProperty(ref _dueDate, value); }
        }

        public TimeSpan DueTime
        {
            get { return _dueTime; }
            set { SetProperty(ref _dueTime, value); }
        }

        public string LectureSection
        {
            get { return _lectureSection; }
            set { SetProperty(ref _lectureSection, value); }
        }

        public string SubmissionUrl
        {
            get { return _submissionUrl; }
            set { SetProperty(ref _submissionUrl, value); }
        }

        public DateTime DueDateTime => DueDate.Add(DueTime);

        public Assignment()
        {
            DueTime = new TimeSpan(23, 59, 59);
        }

        public Assignment(int id, string name, string description, DateTime dueDate, TimeSpan dueTime, string lectureSection, string submissionUrl = "")
        {
            Id = id;
            Name = name;
            Description = description;
            DueDate = dueDate;
            DueTime = dueTime;
            LectureSection = lectureSection;
            SubmissionUrl = submissionUrl;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected bool SetProperty<T>(ref T backingStore, T value, [CallerMemberName] string propertyName = "")
        {
            if (EqualityComparer<T>.Default.Equals(backingStore, value))
                return false;

            backingStore = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

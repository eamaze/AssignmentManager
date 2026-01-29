using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using WpfApp1.Models;
using WpfApp1.Services;

namespace WpfApp1.ViewModels
{
    public class AssignmentViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<Assignment> _assignments;
        private Assignment _selectedAssignment;
        private Assignment _currentAssignment;
        private string _searchText;
        private int _nextId = 1;
        private bool _isEditing = false;
        private readonly AssignmentStorageService _storageService;

        public ObservableCollection<Assignment> Assignments
        {
            get { return _assignments; }
            set { SetProperty(ref _assignments, value); }
        }

        public Assignment SelectedAssignment
        {
            get { return _selectedAssignment; }
            set { SetProperty(ref _selectedAssignment, value); }
        }

        public Assignment CurrentAssignment
        {
            get { return _currentAssignment; }
            set { SetProperty(ref _currentAssignment, value); }
        }

        public string SearchText
        {
            get { return _searchText; }
            set { SetProperty(ref _searchText, value); }
        }

        public AssignmentViewModel()
        {
            Assignments = new ObservableCollection<Assignment>();
            CurrentAssignment = new Assignment { DueDate = DateTime.Now };
            _storageService = new AssignmentStorageService();
            LoadAssignments();
        }

        /// <summary>
        /// Loads assignments from storage, or loads sample data if this is the first time the app is run.
        /// </summary>
        public void LoadAssignments()
        {
            var storedAssignments = _storageService.LoadAssignments();
            
            Assignments.Clear();
            
            if (storedAssignments.Count > 0)
            {
                foreach (var assignment in storedAssignments.OrderBy(a => a.Id))
                {
                    Assignments.Add(assignment);
                }
                _nextId = storedAssignments.Max(a => a.Id) + 1;
            }
            else if (_storageService.IsFirstTimeInitialization())
            {
                // Load sample data only on first app launch
                LoadSampleData();
            }
            // If storage is empty but not first time, just keep it empty (user deleted all)
        }

        public void LoadSampleData()
        {
            Assignments.Clear();
            var sampleAssignments = new List<Assignment>
            {
                new Assignment(1, "Assignment 1", "Complete chapter 1-3", DateTime.Now.AddDays(7), new TimeSpan(17, 0, 0), "CS 101", "https://classroom.google.com"),
                new Assignment(2, "Assignment 2", "Write a report on algorithms", DateTime.Now.AddDays(14), new TimeSpan(23, 59, 59), "CS 201", ""),
                new Assignment(3, "Assignment 3", "Build a simple calculator", DateTime.Now.AddDays(3), new TimeSpan(14, 30, 0), "CS 102", "https://github.com"),
                new Assignment(4, "Quiz Preparation", "Study chapters 5-8", DateTime.Now.AddDays(2), new TimeSpan(10, 0, 0), "CS 101", "")
            };
            
            foreach (var assignment in sampleAssignments)
            {
                Assignments.Add(assignment);
            }
            
            _nextId = 5;
            SaveAllAssignments();
            _storageService.MarkAsInitialized();
        }

        public void AddAssignment()
        {
            if (string.IsNullOrWhiteSpace(CurrentAssignment.Name))
                return;

            var assignment = new Assignment
            {
                Id = _nextId++,
                Name = CurrentAssignment.Name,
                Description = CurrentAssignment.Description,
                DueDate = CurrentAssignment.DueDate,
                DueTime = CurrentAssignment.DueTime,
                LectureSection = CurrentAssignment.LectureSection,
                SubmissionUrl = CurrentAssignment.SubmissionUrl
            };

            Assignments.Add(assignment);
            _storageService.SaveAssignment(assignment);
            ClearCurrentAssignment();
        }

        public void UpdateAssignment()
        {
            if (SelectedAssignment == null)
                return;

            SelectedAssignment.Name = CurrentAssignment.Name;
            SelectedAssignment.Description = CurrentAssignment.Description;
            SelectedAssignment.DueDate = CurrentAssignment.DueDate;
            SelectedAssignment.DueTime = CurrentAssignment.DueTime;
            SelectedAssignment.LectureSection = CurrentAssignment.LectureSection;
            SelectedAssignment.SubmissionUrl = CurrentAssignment.SubmissionUrl;

            _storageService.SaveAssignment(SelectedAssignment);
            _isEditing = false;
        }

        public void DeleteAssignment()
        {
            if (SelectedAssignment != null)
            {
                int assignmentId = SelectedAssignment.Id;
                Assignments.Remove(SelectedAssignment);
                _storageService.DeleteAssignment(assignmentId);
                ClearCurrentAssignment();
            }
        }

        public void EditAssignment()
        {
            if (SelectedAssignment == null)
                return;

            _isEditing = true;
            CurrentAssignment = new Assignment
            {
                Id = SelectedAssignment.Id,
                Name = SelectedAssignment.Name,
                Description = SelectedAssignment.Description,
                DueDate = SelectedAssignment.DueDate,
                DueTime = SelectedAssignment.DueTime,
                LectureSection = SelectedAssignment.LectureSection,
                SubmissionUrl = SelectedAssignment.SubmissionUrl
            };
        }

        protected bool SetProperty<T>(ref T backingStore, T value, [CallerMemberName] string propertyName = "", Action onChanged = null)
        {
            if (EqualityComparer<T>.Default.Equals(backingStore, value))
                return false;

            backingStore = value;
            onChanged?.Invoke();
            OnPropertyChanged(propertyName);

            // Auto-update assignment when editing
            if (_isEditing && propertyName != nameof(CurrentAssignment) && SelectedAssignment != null)
            {
                UpdateAssignment();
            }

            return true;
        }


        public void ClearCurrentAssignment()
        {
            _isEditing = false;
            CurrentAssignment = new Assignment { DueDate = DateTime.Now };
            SelectedAssignment = null;
        }

        /// <summary>
        /// Saves all assignments to storage.
        /// </summary>
        public void SaveAllAssignments()
        {
            _storageService.SaveAssignments(Assignments.ToList());
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

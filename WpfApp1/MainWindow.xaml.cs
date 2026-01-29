using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using WpfApp1.Models;
using WpfApp1.ViewModels;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private AssignmentViewModel _viewModel;
        private DispatcherTimer _notificationTimer;
        private Assignment _currentEditingAssignment;
        private bool _isEditMode;

        public MainWindow()
        {
            InitializeComponent();
            _viewModel = new AssignmentViewModel();
            DataContext = _viewModel;

            _notificationTimer = new DispatcherTimer();
            _notificationTimer.Interval = System.TimeSpan.FromSeconds(4);
            _notificationTimer.Tick += (s, e) =>
            {
                NotificationBar.Visibility = Visibility.Collapsed;
                _notificationTimer.Stop();
            };
        }

        private void ShowNotification(string message)
        {
            NotificationText.Text = message;
            NotificationBar.Visibility = Visibility.Visible;
            _notificationTimer.Stop();
            _notificationTimer.Start();
        }

        private void CreateNewAssignmentButton_Click(object sender, RoutedEventArgs e)
        {
            _currentEditingAssignment = new Assignment { DueDate = System.DateTime.Now };
            _viewModel.CurrentAssignment = _currentEditingAssignment;
            _isEditMode = false;
            ShowEditPopup();
        }

        private void EditAssignmentButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button?.Tag is Assignment assignment)
            {
                _currentEditingAssignment = assignment;
                _viewModel.SelectedAssignment = assignment;
                _viewModel.EditAssignment();
                _viewModel.CurrentAssignment = _viewModel.CurrentAssignment; // Refresh binding
                _isEditMode = true;
                ShowEditPopup();
            }
        }

        private void AssignmentCard_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var border = sender as Border;
            if (border?.Tag is Assignment assignment)
            {
                ShowDetailsPopup(assignment);
            }
        }

        private void ShowEditPopup()
        {
            PopupTitle.Text = _isEditMode ? "Edit Assignment" : "Create New Assignment";
            PopupDeleteButton.Visibility = _isEditMode ? Visibility.Visible : Visibility.Collapsed;
            PopupSaveButton.Content = _isEditMode ? "Save" : "Create";
            
            PopupContent.Children.Clear();
            
            var stackPanel = new StackPanel { Orientation = Orientation.Vertical };
            
            // Assignment Name
            stackPanel.Children.Add(new TextBlock 
            { 
                Text = "Assignment Name", 
                FontWeight = FontWeights.SemiBold, 
                Margin = new Thickness(0, 0, 0, 6), 
                Foreground = (System.Windows.Media.Brush)FindResource("MaterialDesignBody"), 
                FontFamily = new System.Windows.Media.FontFamily("Segoe UI") 
            });
            var nameTextBox = new TextBox 
            { 
                Text = _viewModel.CurrentAssignment?.Name ?? "", 
                Padding = new Thickness(12, 10, 12, 10), 
                BorderThickness = new Thickness(1), 
                BorderBrush = (System.Windows.Media.Brush)FindResource("MaterialDesignDivider"), 
                Margin = new Thickness(0, 0, 0, 14), 
                Background = (System.Windows.Media.Brush)FindResource("MaterialDesignCardBackground"), 
                Foreground = (System.Windows.Media.Brush)FindResource("MaterialDesignBody"), 
                FontFamily = new System.Windows.Media.FontFamily("Segoe UI") 
            };
            nameTextBox.TextChanged += (s, e) => 
            { 
                if (_viewModel.CurrentAssignment != null) 
                    _viewModel.CurrentAssignment.Name = nameTextBox.Text; 
            };
            stackPanel.Children.Add(nameTextBox);
            
            // Lecture Section
            stackPanel.Children.Add(new TextBlock 
            { 
                Text = "Lecture Section", 
                FontWeight = FontWeights.SemiBold, 
                Margin = new Thickness(0, 0, 0, 6), 
                Foreground = (System.Windows.Media.Brush)FindResource("MaterialDesignBody"), 
                FontFamily = new System.Windows.Media.FontFamily("Segoe UI") 
            });
            var lectureTextBox = new TextBox 
            { 
                Text = _viewModel.CurrentAssignment?.LectureSection ?? "", 
                Padding = new Thickness(12, 10, 12, 10), 
                BorderThickness = new Thickness(1), 
                BorderBrush = (System.Windows.Media.Brush)FindResource("MaterialDesignDivider"), 
                Margin = new Thickness(0, 0, 0, 14), 
                Background = (System.Windows.Media.Brush)FindResource("MaterialDesignCardBackground"), 
                Foreground = (System.Windows.Media.Brush)FindResource("MaterialDesignBody"), 
                FontFamily = new System.Windows.Media.FontFamily("Segoe UI") 
            };
            lectureTextBox.TextChanged += (s, e) => 
            { 
                if (_viewModel.CurrentAssignment != null) 
                    _viewModel.CurrentAssignment.LectureSection = lectureTextBox.Text; 
            };
            stackPanel.Children.Add(lectureTextBox);
            
            // Description
            stackPanel.Children.Add(new TextBlock 
            { 
                Text = "Description", 
                FontWeight = FontWeights.SemiBold, 
                Margin = new Thickness(0, 0, 0, 6), 
                Foreground = (System.Windows.Media.Brush)FindResource("MaterialDesignBody"), 
                FontFamily = new System.Windows.Media.FontFamily("Segoe UI") 
            });
            var descriptionTextBox = new TextBox 
            { 
                Text = _viewModel.CurrentAssignment?.Description ?? "", 
                TextWrapping = TextWrapping.Wrap, 
                AcceptsReturn = true, 
                Height = 100, 
                VerticalAlignment = VerticalAlignment.Top, 
                Padding = new Thickness(12, 10, 12, 10), 
                BorderThickness = new Thickness(1), 
                BorderBrush = (System.Windows.Media.Brush)FindResource("MaterialDesignDivider"), 
                Margin = new Thickness(0, 0, 0, 14), 
                Background = (System.Windows.Media.Brush)FindResource("MaterialDesignCardBackground"), 
                Foreground = (System.Windows.Media.Brush)FindResource("MaterialDesignBody"), 
                FontFamily = new System.Windows.Media.FontFamily("Segoe UI") 
            };
            descriptionTextBox.TextChanged += (s, e) => 
            { 
                if (_viewModel.CurrentAssignment != null) 
                    _viewModel.CurrentAssignment.Description = descriptionTextBox.Text; 
            };
            stackPanel.Children.Add(descriptionTextBox);
            
            // Date and Time in Grid
            var dateTimeGrid = new Grid 
            { 
                Margin = new Thickness(0, 0, 0, 14) 
            };
            dateTimeGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            dateTimeGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            
            // Due Date
            var datePanel = new StackPanel 
            { 
                Margin = new Thickness(0, 0, 6, 0) 
            };
            datePanel.Children.Add(new TextBlock 
            { 
                Text = "Due Date", 
                FontWeight = FontWeights.SemiBold, 
                Margin = new Thickness(0, 0, 0, 6), 
                Foreground = (System.Windows.Media.Brush)FindResource("MaterialDesignBody"), 
                FontFamily = new System.Windows.Media.FontFamily("Segoe UI") 
            });
            var datePicker = new DatePicker 
            { 
                SelectedDate = _viewModel.CurrentAssignment?.DueDate, 
                Padding = new Thickness(12, 10, 12, 10), 
                Background = (System.Windows.Media.Brush)FindResource("MaterialDesignCardBackground"), 
                Foreground = (System.Windows.Media.Brush)FindResource("MaterialDesignBody"), 
                FontFamily = new System.Windows.Media.FontFamily("Segoe UI"),
                BorderBrush = (System.Windows.Media.Brush)FindResource("MaterialDesignDivider"),
                BorderThickness = new Thickness(1),
                Style = (System.Windows.Style)FindResource("DarkDatePickerStyle")
            };
            datePicker.SelectedDateChanged += (s, e) => 
            { 
                if (_viewModel.CurrentAssignment != null && datePicker.SelectedDate.HasValue) 
                {
                    _viewModel.CurrentAssignment.DueDate = datePicker.SelectedDate.Value;
                }
            };
            datePanel.Children.Add(datePicker);
            Grid.SetColumn(datePanel, 0);
            dateTimeGrid.Children.Add(datePanel);
            
            // Due Time
            var timePanel = new StackPanel 
            { 
                Margin = new Thickness(6, 0, 0, 0) 
            };
            timePanel.Children.Add(new TextBlock 
            { 
                Text = "Due Time", 
                FontWeight = FontWeights.SemiBold, 
                Margin = new Thickness(0, 0, 0, 6), 
                Foreground = (System.Windows.Media.Brush)FindResource("MaterialDesignBody"), 
                FontFamily = new System.Windows.Media.FontFamily("Segoe UI") 
            });

            // Create time picker with hour and minute spinners
            var timePickerGrid = new Grid();
            timePickerGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            timePickerGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            timePickerGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });


            // Hour TextBox
            var hourTextBox = new System.Windows.Controls.TextBox
            {
                Text = _viewModel.CurrentAssignment?.DueTime.Hours.ToString("D2") ?? "23",
                Padding = new Thickness(8, 10, 8, 10),
                BorderThickness = new Thickness(1),
                BorderBrush = (System.Windows.Media.Brush)FindResource("MaterialDesignDivider"),
                Background = (System.Windows.Media.Brush)FindResource("MaterialDesignCardBackground"),
                Foreground = (System.Windows.Media.Brush)FindResource("MaterialDesignBody"),
                FontFamily = new System.Windows.Media.FontFamily("Segoe UI"),
                TextAlignment = TextAlignment.Center,
                FontSize = 14,
                MaxLength = 2
            };

            // Colon separator
            var colonText = new TextBlock
            {
                Text = ":",
                Foreground = (System.Windows.Media.Brush)FindResource("MaterialDesignBody"),
                FontSize = 18,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(4, 0, 4, 0),
                FontWeight = FontWeights.Bold
            };

            // Minute TextBox
            var minuteTextBox = new System.Windows.Controls.TextBox
            {
                Text = _viewModel.CurrentAssignment?.DueTime.Minutes.ToString("D2") ?? "59",
                Padding = new Thickness(8, 10, 8, 10),
                BorderThickness = new Thickness(1),
                BorderBrush = (System.Windows.Media.Brush)FindResource("MaterialDesignDivider"),
                Background = (System.Windows.Media.Brush)FindResource("MaterialDesignCardBackground"),
                Foreground = (System.Windows.Media.Brush)FindResource("MaterialDesignBody"),
                FontFamily = new System.Windows.Media.FontFamily("Segoe UI"),
                TextAlignment = TextAlignment.Center,
                FontSize = 14,
                MaxLength = 2
            };

            // Add input validation for hour textbox (0-23)
            hourTextBox.PreviewTextInput += (s, e) =>
            {
                e.Handled = !System.Text.RegularExpressions.Regex.IsMatch(e.Text, @"^[0-9]$");
            };
            hourTextBox.TextChanged += (s, e) =>
            {
                if (hourTextBox.Text.Length > 0 && int.TryParse(hourTextBox.Text, out var hour))
                {
                    // Auto-correct if hour exceeds 23
                    if (hour > 23)
                    {
                        hourTextBox.Text = "23";
                        hour = 23;
                    }
                    // Update time if minute is also valid
                    if (_viewModel.CurrentAssignment != null && int.TryParse(minuteTextBox.Text, out var minute))
                    {
                        _viewModel.CurrentAssignment.DueTime = new System.TimeSpan(hour, minute, 0);
                    }
                }
            };

            // Add input validation for minute textbox (0-59)
            minuteTextBox.PreviewTextInput += (s, e) =>
            {
                e.Handled = !System.Text.RegularExpressions.Regex.IsMatch(e.Text, @"^[0-9]$");
            };
            minuteTextBox.TextChanged += (s, e) =>
            {
                if (minuteTextBox.Text.Length > 0 && int.TryParse(minuteTextBox.Text, out var minute))
                {
                    // Auto-correct if minute exceeds 59
                    if (minute > 59)
                    {
                        minuteTextBox.Text = "59";
                        minute = 59;
                    }
                    // Update time if hour is also valid
                    if (_viewModel.CurrentAssignment != null && int.TryParse(hourTextBox.Text, out var hour))
                    {
                        _viewModel.CurrentAssignment.DueTime = new System.TimeSpan(hour, minute, 0);
                    }
                }
            };


            Grid.SetColumn(hourTextBox, 0);
            Grid.SetColumn(colonText, 1);
            Grid.SetColumn(minuteTextBox, 2);

            timePickerGrid.Children.Add(hourTextBox);
            timePickerGrid.Children.Add(colonText);
            timePickerGrid.Children.Add(minuteTextBox);

            // Update time whenever hour or minute changes
            hourTextBox.TextChanged += (s, e) =>
            {
                if (_viewModel.CurrentAssignment != null && int.TryParse(hourTextBox.Text, out var hour) && int.TryParse(minuteTextBox.Text, out var minute))
                {
                    hour = System.Math.Max(0, System.Math.Min(23, hour));
                    _viewModel.CurrentAssignment.DueTime = new System.TimeSpan(hour, minute, 0);
                }
            };

            minuteTextBox.TextChanged += (s, e) =>
            {
                if (_viewModel.CurrentAssignment != null && int.TryParse(hourTextBox.Text, out var hour) && int.TryParse(minuteTextBox.Text, out var minute))
                {
                    minute = System.Math.Max(0, System.Math.Min(59, minute));
                    _viewModel.CurrentAssignment.DueTime = new System.TimeSpan(hour, minute, 0);
                }
            };

            timePanel.Children.Add(timePickerGrid);
            Grid.SetColumn(timePanel, 1);
            dateTimeGrid.Children.Add(timePanel);
            stackPanel.Children.Add(dateTimeGrid);
            
            // Submission URL
            stackPanel.Children.Add(new TextBlock 
            { 
                Text = "Submission URL (Optional)", 
                FontWeight = FontWeights.SemiBold, 
                Margin = new Thickness(0, 0, 0, 6), 
                Foreground = (System.Windows.Media.Brush)FindResource("MaterialDesignBody"), 
                FontFamily = new System.Windows.Media.FontFamily("Segoe UI") 
            });
            var urlTextBox = new TextBox 
            { 
                Text = _viewModel.CurrentAssignment?.SubmissionUrl ?? "", 
                Padding = new Thickness(12, 10, 12, 10), 
                BorderThickness = new Thickness(1), 
                BorderBrush = (System.Windows.Media.Brush)FindResource("MaterialDesignDivider"), 
                Margin = new Thickness(0, 0, 0, 20), 
                Background = (System.Windows.Media.Brush)FindResource("MaterialDesignCardBackground"), 
                Foreground = (System.Windows.Media.Brush)FindResource("MaterialDesignBody"), 
                FontFamily = new System.Windows.Media.FontFamily("Segoe UI") 
            };
            urlTextBox.TextChanged += (s, e) => 
            { 
                if (_viewModel.CurrentAssignment != null) 
                    _viewModel.CurrentAssignment.SubmissionUrl = urlTextBox.Text; 
            };
            stackPanel.Children.Add(urlTextBox);
            
            PopupContent.Children.Add(stackPanel);
            
            PopupOverlay.Visibility = Visibility.Visible;
            EditPopup.Visibility = Visibility.Visible;
        }

        private void ShowDetailsPopup(Assignment assignment)
        {
            // Set the current assignment references so edit will work correctly
            _currentEditingAssignment = assignment;
            _viewModel.SelectedAssignment = assignment;
            
            PopupTitle.Text = "Assignment Details";
            PopupDeleteButton.Visibility = Visibility.Collapsed;
            PopupSaveButton.Content = "Edit";
            
            PopupContent.Children.Clear();
            
            var stackPanel = new StackPanel { Orientation = Orientation.Vertical };
            
            stackPanel.Children.Add(new TextBlock 
            { 
                Text = assignment.Name, 
                FontSize = 18, 
                FontWeight = FontWeights.SemiBold, 
                Foreground = (System.Windows.Media.Brush)FindResource("MaterialDesignBody"), 
                FontFamily = new System.Windows.Media.FontFamily("Segoe UI"), 
                Margin = new Thickness(0, 0, 0, 8) 
            });
            
            stackPanel.Children.Add(new TextBlock 
            { 
                Text = assignment.LectureSection, 
                Foreground = (System.Windows.Media.Brush)FindResource("MaterialDesignBodyLight"), 
                FontSize = 12, 
                Margin = new Thickness(0, 0, 0, 16), 
                FontFamily = new System.Windows.Media.FontFamily("Segoe UI") 
            });
            
            stackPanel.Children.Add(new TextBlock 
            { 
                Text = "Due Date & Time", 
                FontWeight = FontWeights.SemiBold, 
                Foreground = (System.Windows.Media.Brush)FindResource("MaterialDesignBody"), 
                FontFamily = new System.Windows.Media.FontFamily("Segoe UI"), 
                Margin = new Thickness(0, 0, 0, 6) 
            });
            
            stackPanel.Children.Add(new TextBlock 
            { 
                Text = $"{assignment.DueDate:MMMM d, yyyy} at {System.DateTime.MinValue.Add(assignment.DueTime):hh\\:mm\\ tt}", 
                Foreground = (System.Windows.Media.Brush)FindResource("SecondaryHueMid"), 
                FontSize = 12, 
                Margin = new Thickness(0, 0, 0, 16), 
                FontFamily = new System.Windows.Media.FontFamily("Segoe UI") 
            });
            
            if (!string.IsNullOrEmpty(assignment.Description))
            {
                stackPanel.Children.Add(new TextBlock 
                { 
                    Text = "Description", 
                    FontWeight = FontWeights.SemiBold, 
                    Foreground = (System.Windows.Media.Brush)FindResource("MaterialDesignBody"), 
                    FontFamily = new System.Windows.Media.FontFamily("Segoe UI"), 
                    Margin = new Thickness(0, 0, 0, 6) 
                });
                stackPanel.Children.Add(new TextBlock 
                { 
                    Text = assignment.Description, 
                    TextWrapping = TextWrapping.Wrap, 
                    Foreground = (System.Windows.Media.Brush)FindResource("MaterialDesignBodyLight"), 
                    FontSize = 11, 
                    Margin = new Thickness(0, 0, 0, 16), 
                    FontFamily = new System.Windows.Media.FontFamily("Segoe UI") 
                });
            }
            
            if (!string.IsNullOrEmpty(assignment.SubmissionUrl))
            {
                stackPanel.Children.Add(new TextBlock 
                { 
                    Text = "Submission URL", 
                    FontWeight = FontWeights.SemiBold, 
                    Foreground = (System.Windows.Media.Brush)FindResource("MaterialDesignBody"), 
                    FontFamily = new System.Windows.Media.FontFamily("Segoe UI"), 
                    Margin = new Thickness(0, 0, 0, 6) 
                });
                var linkButton = new TextBlock 
                { 
                    Text = assignment.SubmissionUrl, 
                    Foreground = (System.Windows.Media.Brush)FindResource("SecondaryHueMid"), 
                    TextDecorations = TextDecorations.Underline,
                    Cursor = System.Windows.Input.Cursors.Hand, 
                    FontFamily = new System.Windows.Media.FontFamily("Segoe UI") 
                };
                linkButton.MouseLeftButtonDown += (s, e) =>
                {
                    try
                    {
                        string url = assignment.SubmissionUrl;
                        // Ensure the URL has a scheme (http:// or https://)
                        if (!url.StartsWith("http://") && !url.StartsWith("https://"))
                        {
                            url = "https://" + url;
                        }
                        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo { FileName = url, UseShellExecute = true });
                    }
                    catch { }
                };
                stackPanel.Children.Add(linkButton);
            }
            
            PopupContent.Children.Add(stackPanel);
            
            PopupOverlay.Visibility = Visibility.Visible;
            EditPopup.Visibility = Visibility.Visible;
        }

        private void ClosePopup_Click(object sender, RoutedEventArgs e)
        {
            PopupOverlay.Visibility = Visibility.Collapsed;
            EditPopup.Visibility = Visibility.Collapsed;
        }

        private void PopupCancel_Click(object sender, RoutedEventArgs e)
        {
            PopupOverlay.Visibility = Visibility.Collapsed;
            EditPopup.Visibility = Visibility.Collapsed;
        }

        private void PopupSave_Click(object sender, RoutedEventArgs e)
        {
            if (PopupSaveButton.Content.ToString() == "Edit")
            {
                _currentEditingAssignment = _currentEditingAssignment ?? (_viewModel.SelectedAssignment ?? _currentEditingAssignment);
                _viewModel.SelectedAssignment = _currentEditingAssignment;
                _viewModel.EditAssignment();
                _isEditMode = true;
                ShowEditPopup();
            }
            else
            {
                if (string.IsNullOrWhiteSpace(_viewModel.CurrentAssignment.Name))
                {
                    ShowNotification("Please enter an assignment name.");
                    return;
                }

                if (_isEditMode)
                {
                    _viewModel.UpdateAssignment();
                    CustomCalendar.InvalidateCalendar();
                    ShowNotification("Assignment updated successfully!");
                }
                else
                {
                    _viewModel.AddAssignment();
                    CustomCalendar.InvalidateCalendar();
                    ShowNotification("Assignment created successfully!");
                }

                PopupOverlay.Visibility = Visibility.Collapsed;
                EditPopup.Visibility = Visibility.Collapsed;
            }
        }

        private void PopupDeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (_viewModel.SelectedAssignment == null)
                return;

            DeleteConfirmMessage.Text = $"Are you sure you want to delete '{_viewModel.SelectedAssignment.Name}'?";
            PopupOverlay.Visibility = Visibility.Visible;
            DeleteConfirmPopup.Visibility = Visibility.Visible;
        }

        private void DeleteConfirmCancel_Click(object sender, RoutedEventArgs e)
        {
            PopupOverlay.Visibility = Visibility.Collapsed;
            DeleteConfirmPopup.Visibility = Visibility.Collapsed;
        }

        private void DeleteConfirmYes_Click(object sender, RoutedEventArgs e)
        {
            if (_viewModel.SelectedAssignment == null)
                return;

            string assignmentName = _viewModel.SelectedAssignment.Name;
            _viewModel.DeleteAssignment();
            CustomCalendar.InvalidateCalendar();
            ShowNotification($"Assignment '{assignmentName}' deleted successfully!");
            
            PopupOverlay.Visibility = Visibility.Collapsed;
            EditPopup.Visibility = Visibility.Collapsed;
            DeleteConfirmPopup.Visibility = Visibility.Collapsed;
        }

        private void CustomCalendar_AssignmentClicked(object sender, RoutedEventArgs e)
        {
            if (e.OriginalSource is Assignment assignment)
            {
                ShowDetailsPopup(assignment);
            }
        }

        private void CustomCalendar_CreateAssignment(object sender, RoutedEventArgs e)
        {
            if (e.OriginalSource is DateTime date)
            {
                _currentEditingAssignment = new Assignment { DueDate = date };
                _viewModel.CurrentAssignment = _currentEditingAssignment;
                _isEditMode = false;
                ShowEditPopup();
            }
        }

        private void SettingsPanel_SyncCompleted(object sender, EventArgs e)
        {
            // Refresh the calendar and assignment list when sync completes
            CustomCalendar.InvalidateCalendar();
            _viewModel.LoadAssignments();
        }
    }
}



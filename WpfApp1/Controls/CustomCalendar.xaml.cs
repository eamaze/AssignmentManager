using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using WpfApp1.Models;

namespace WpfApp1.Controls
{
    public partial class CustomCalendar : UserControl
    {
        public static readonly DependencyProperty AssignmentsProperty =
            DependencyProperty.Register("Assignments", typeof(ObservableCollection<Assignment>), typeof(CustomCalendar),
                new PropertyMetadata(null, OnAssignmentsChanged));

        public ObservableCollection<Assignment> Assignments
        {
            get { return (ObservableCollection<Assignment>)GetValue(AssignmentsProperty); }
            set { SetValue(AssignmentsProperty, value); }
        }

        public static readonly DependencyProperty SelectedAssignmentProperty =
            DependencyProperty.Register("SelectedAssignment", typeof(Assignment), typeof(CustomCalendar),
                new PropertyMetadata(null));

        public Assignment SelectedAssignment
        {
            get { return (Assignment)GetValue(SelectedAssignmentProperty); }
            set { SetValue(SelectedAssignmentProperty, value); }
        }

        public static readonly RoutedEvent AssignmentClickedEvent =
            EventManager.RegisterRoutedEvent("AssignmentClicked", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(CustomCalendar));

        public event RoutedEventHandler AssignmentClicked
        {
            add { AddHandler(AssignmentClickedEvent, value); }
            remove { RemoveHandler(AssignmentClickedEvent, value); }
        }

        public static readonly RoutedEvent CreateAssignmentEvent =
            EventManager.RegisterRoutedEvent("CreateAssignment", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(CustomCalendar));

        public event RoutedEventHandler CreateAssignment
        {
            add { AddHandler(CreateAssignmentEvent, value); }
            remove { RemoveHandler(CreateAssignmentEvent, value); }
        }

        private DateTime _currentDate = DateTime.Now;
        private ViewMode _currentView = ViewMode.Month;

        public enum ViewMode
        {
            Day,
            Week,
            Month
        }

        public CustomCalendar()
        {
            InitializeComponent();
            RefreshCalendar();
        }

        private static void OnAssignmentsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is CustomCalendar calendar)
            {
                // Unsubscribe from old collection's CollectionChanged event
                if (e.OldValue is ObservableCollection<Assignment> oldCollection)
                {
                    oldCollection.CollectionChanged -= calendar.OnAssignmentsCollectionChanged;
                }

                // Subscribe to new collection's CollectionChanged event
                if (e.NewValue is ObservableCollection<Assignment> newCollection)
                {
                    newCollection.CollectionChanged += calendar.OnAssignmentsCollectionChanged;
                }

                calendar.RefreshCalendar();
            }
        }

        private void OnAssignmentsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            // Refresh calendar whenever the assignments collection changes (add, remove, clear, etc.)
            RefreshCalendar();
        }

        private void PreviousButton_Click(object sender, RoutedEventArgs e)
        {
            _currentDate = _currentView switch
            {
                ViewMode.Day => _currentDate.AddDays(-1),
                ViewMode.Week => _currentDate.AddDays(-7),
                ViewMode.Month => _currentDate.AddMonths(-1),
                _ => _currentDate
            };
            RefreshCalendar();
        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            _currentDate = _currentView switch
            {
                ViewMode.Day => _currentDate.AddDays(1),
                ViewMode.Week => _currentDate.AddDays(7),
                ViewMode.Month => _currentDate.AddMonths(1),
                _ => _currentDate
            };
            RefreshCalendar();
        }

        private void DayViewButton_Click(object sender, RoutedEventArgs e)
        {
            _currentView = ViewMode.Day;
            UpdateViewButtons();
            RefreshCalendar();
        }

        private void WeekViewButton_Click(object sender, RoutedEventArgs e)
        {
            _currentView = ViewMode.Week;
            UpdateViewButtons();
            RefreshCalendar();
        }

        private void MonthViewButton_Click(object sender, RoutedEventArgs e)
        {
            _currentView = ViewMode.Month;
            UpdateViewButtons();
            RefreshCalendar();
        }

        private void OnAssignmentClicked(Assignment assignment)
        {
            RaiseEvent(new RoutedEventArgs(AssignmentClickedEvent, assignment));
        }

        private void OnCreateAssignmentForDate(DateTime date)
        {
            RaiseEvent(new RoutedEventArgs(CreateAssignmentEvent, date));
        }



        private void UpdateViewButtons()
        {
            var darkBrush = (Brush)FindResource("SecondaryHueMid");
            var lightBrush = (Brush)FindResource("MaterialDesignBodyLight");
            var bodyBrush = (Brush)FindResource("MaterialDesignBody");

            DayViewButton.Background = _currentView == ViewMode.Day ? darkBrush : Brushes.Transparent;
            DayViewButton.Foreground = _currentView == ViewMode.Day ? bodyBrush : lightBrush;
            DayViewButton.BorderThickness = _currentView == ViewMode.Day ? new Thickness(0) : new Thickness(1);

            WeekViewButton.Background = _currentView == ViewMode.Week ? darkBrush : Brushes.Transparent;
            WeekViewButton.Foreground = _currentView == ViewMode.Week ? bodyBrush : lightBrush;
            WeekViewButton.BorderThickness = _currentView == ViewMode.Week ? new Thickness(0) : new Thickness(1);

            MonthViewButton.Background = _currentView == ViewMode.Month ? darkBrush : Brushes.Transparent;
            MonthViewButton.Foreground = _currentView == ViewMode.Month ? bodyBrush : lightBrush;
            MonthViewButton.BorderThickness = _currentView == ViewMode.Month ? new Thickness(0) : new Thickness(1);
        }

        private void RefreshCalendar()
        {
            RenderCalendarInternal();
        }

        /// <summary>
        /// Public method to refresh the calendar from external sources.
        /// Call this whenever assignments are modified to ensure the calendar is up-to-date.
        /// </summary>
        public void InvalidateCalendar()
        {
            RenderCalendarInternal();
        }

        private void RenderCalendarInternal()
        {
            CalendarGrid.Children.Clear();
            CalendarGrid.RowDefinitions.Clear();
            CalendarGrid.ColumnDefinitions.Clear();

            switch (_currentView)
            {
                case ViewMode.Day:
                    RenderDayView();
                    break;
                case ViewMode.Week:
                    RenderWeekView();
                    break;
                case ViewMode.Month:
                    RenderMonthView();
                    break;
            }

            UpdateViewButtons();
        }

        private void RenderDayView()
        {
            HeaderTextBlock.Text = _currentDate.ToString("dddd, MMMM d, yyyy");

            CalendarGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            CalendarGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

            var dayAssignments = Assignments?.Where(a => a.DueDate.Date == _currentDate.Date).ToList() ?? new System.Collections.Generic.List<Assignment>();

            if (dayAssignments.Count == 0)
            {
                var emptyText = new TextBlock
                {
                    Text = "No assignments due on this day",
                    Foreground = (SolidColorBrush)FindResource("MaterialDesignBodyLight"),
                    FontFamily = new System.Windows.Media.FontFamily("Segoe UI"),
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    Margin = new Thickness(16)
                };
                Grid.SetRow(emptyText, 0);
                CalendarGrid.Children.Add(emptyText);
            }
            else
            {
                foreach (var assignment in dayAssignments)
                {
                    CalendarGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
                    var card = CreateAssignmentCard(assignment);
                    Grid.SetRow(card, CalendarGrid.RowDefinitions.Count - 1);
                    CalendarGrid.Children.Add(card);
                }
            }
        }

        private void RenderWeekView()
        {
            var weekStart = _currentDate.AddDays(-(int)_currentDate.DayOfWeek);
            var weekEnd = weekStart.AddDays(6);
            HeaderTextBlock.Text = $"{weekStart:MMM d} - {weekEnd:MMM d, yyyy}";

            for (int i = 0; i < 7; i++)
            {
                CalendarGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            }

            CalendarGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

            string[] dayNames = { "Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat" };
            for (int i = 0; i < 7; i++)
            {
                var dayHeader = new TextBlock
                {
                    Text = dayNames[i],
                    FontWeight = System.Windows.FontWeights.SemiBold,
                    Foreground = (SolidColorBrush)FindResource("MaterialDesignBody"),
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Margin = new Thickness(8),
                    FontFamily = new System.Windows.Media.FontFamily("Segoe UI")
                };
                Grid.SetColumn(dayHeader, i);
                Grid.SetRow(dayHeader, 0);
                CalendarGrid.Children.Add(dayHeader);
            }

            CalendarGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });

            for (int i = 0; i < 7; i++)
            {
                var currentDay = weekStart.AddDays(i);
                var dayAssignments = Assignments?.Where(a => a.DueDate.Date == currentDay.Date).ToList() ?? new System.Collections.Generic.List<Assignment>();

                var dayColumn = new StackPanel { Orientation = Orientation.Vertical, Margin = new Thickness(4) };

                var dateText = new TextBlock
                {
                    Text = currentDay.Day.ToString(),
                    FontWeight = System.Windows.FontWeights.Bold,
                    Foreground = (SolidColorBrush)FindResource("MaterialDesignBody"),
                    Margin = new Thickness(0, 0, 0, 8),
                    FontFamily = new System.Windows.Media.FontFamily("Segoe UI")
                };
                dayColumn.Children.Add(dateText);

                foreach (var assignment in dayAssignments.Take(2))
                {
                    var assignmentButton = new Button
                    {
                        Content = assignment.Name,
                        FontSize = 10,
                        Padding = new Thickness(2, 2, 2, 2),
                        Margin = new Thickness(0, 2, 0, 0),
                        Background = System.Windows.Media.Brushes.Transparent,
                        Foreground = (SolidColorBrush)FindResource("SecondaryHueMid"),
                        BorderThickness = new Thickness(0),
                        HorizontalAlignment = HorizontalAlignment.Left,
                        Cursor = System.Windows.Input.Cursors.Hand,
                        FontFamily = new System.Windows.Media.FontFamily("Segoe UI"),
                        Tag = assignment
                    };
                    assignmentButton.Click += (s, e) =>
                    {
                        if (s is Button btn && btn.Tag is Assignment a)
                        {
                            OnAssignmentClicked(a); 
                        }
                    };
                    dayColumn.Children.Add(assignmentButton);
                }

                if (dayAssignments.Count > 2)
                {
                    var moreText = new TextBlock
                    {
                        Text = $"+{dayAssignments.Count - 2} more",
                        FontSize = 9,
                        Foreground = (SolidColorBrush)FindResource("MaterialDesignBodyLight"),
                        FontFamily = new System.Windows.Media.FontFamily("Segoe UI")
                    };
                    dayColumn.Children.Add(moreText);
                }

                var isToday = currentDay.Date == DateTime.Today;
                var border = new Border
                {
                    Background = System.Windows.Media.Brushes.Transparent,
                    BorderBrush = isToday ? (SolidColorBrush)FindResource("SecondaryHueMid") : (SolidColorBrush)FindResource("MaterialDesignDivider"),
                    BorderThickness = new Thickness(isToday ? 2 : 1),
                    CornerRadius = new CornerRadius(4),
                    Padding = new Thickness(8),
                    Child = dayColumn
                };

                // Add hover event to show/hide add button
                border.MouseEnter += (s, e) =>
                {
                    var addButton = new Button
                    {
                        Content = "+",
                        FontSize = 9,
                        Padding = new Thickness(2, 1, 2, 1),
                        Margin = new Thickness(0, 2, 0, 0),
                        Background = System.Windows.Media.Brushes.Transparent,
                        Foreground = (SolidColorBrush)FindResource("SecondaryHueMid"),
                        BorderThickness = new Thickness(1),
                        BorderBrush = (SolidColorBrush)FindResource("SecondaryHueMid"),
                        HorizontalAlignment = HorizontalAlignment.Stretch,
                        Cursor = System.Windows.Input.Cursors.Hand,
                        FontFamily = new System.Windows.Media.FontFamily("Segoe UI"),
                        FontWeight = System.Windows.FontWeights.Bold,
                        Tag = currentDay
                    };

                    addButton.Click += (sender, args) =>
                    {
                        OnCreateAssignmentForDate(currentDay);
                    };

                    dayColumn.Children.Add(addButton);
                };

                border.MouseLeave += (s, e) =>
                {
                    // Remove the add button if it's the last child and it's a + button
                    if (dayColumn.Children.Count > 0)
                    {
                        var lastChild = dayColumn.Children[dayColumn.Children.Count - 1];
                        if (lastChild is Button btn && btn.Content?.ToString() == "+")
                        {
                            dayColumn.Children.Remove(lastChild);
                        }
                    }
                };

                Grid.SetColumn(border, i);
                Grid.SetRow(border, 1);
                CalendarGrid.Children.Add(border);
            }
        }

        private void RenderMonthView()
        {
            HeaderTextBlock.Text = _currentDate.ToString("MMMM yyyy");

            var firstDay = new DateTime(_currentDate.Year, _currentDate.Month, 1);
            var lastDay = firstDay.AddMonths(1).AddDays(-1);
            var startDate = firstDay.AddDays(-(int)firstDay.DayOfWeek);

            for (int i = 0; i < 7; i++)
            {
                CalendarGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            }

            string[] dayNames = { "Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat" };
            CalendarGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

            for (int i = 0; i < 7; i++)
            {
                var dayHeader = new TextBlock
                {
                    Text = dayNames[i],
                    FontWeight = System.Windows.FontWeights.SemiBold,
                    Foreground = (SolidColorBrush)FindResource("MaterialDesignBody"),
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Margin = new Thickness(8),
                    FontFamily = new System.Windows.Media.FontFamily("Segoe UI")
                };
                Grid.SetColumn(dayHeader, i);
                Grid.SetRow(dayHeader, 0);
                CalendarGrid.Children.Add(dayHeader);
            }

            var currentDate = startDate;
            int row = 1;
            while (currentDate <= lastDay || currentDate.DayOfWeek != DayOfWeek.Sunday)
            {
                CalendarGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

                for (int col = 0; col < 7; col++)
                {
                    var dayAssignments = Assignments?.Where(a => a.DueDate.Date == currentDate.Date).ToList() ?? new System.Collections.Generic.List<Assignment>();
                    var dayCell = CreateDayCell(currentDate, dayAssignments, currentDate.Month != _currentDate.Month);

                    Grid.SetRow(dayCell, row);
                    Grid.SetColumn(dayCell, col);
                    CalendarGrid.Children.Add(dayCell);

                    currentDate = currentDate.AddDays(1);
                }
                row++;
            }
        }

        private Border CreateDayCell(DateTime date, System.Collections.Generic.List<Assignment> assignments, bool isOutsideMonth)
        {
            var cellStack = new StackPanel { Orientation = Orientation.Vertical, Margin = new Thickness(4) };

            var dateText = new TextBlock
            {
                Text = date.Day.ToString(),
                FontWeight = System.Windows.FontWeights.Bold,
                Foreground = isOutsideMonth ? (SolidColorBrush)FindResource("MaterialDesignBodyLight") : (SolidColorBrush)FindResource("MaterialDesignBody"),
                Margin = new Thickness(0, 0, 0, 4),
                FontFamily = new System.Windows.Media.FontFamily("Segoe UI"),
                FontSize = 12
            };
            cellStack.Children.Add(dateText);

            foreach (var assignment in assignments.Take(2))
            {
                var assignmentButton = new Button
                {
                    Content = assignment.Name,
                    FontSize = 9,
                    Padding = new Thickness(2, 1, 2, 1),
                    Margin = new Thickness(0, 1, 0, 0),
                    Background = System.Windows.Media.Brushes.Transparent,
                    Foreground = (SolidColorBrush)FindResource("SecondaryHueMid"),
                    BorderThickness = new Thickness(0),
                    HorizontalAlignment = HorizontalAlignment.Left,
                    Cursor = System.Windows.Input.Cursors.Hand,
                    FontFamily = new System.Windows.Media.FontFamily("Segoe UI"),
                    Tag = assignment
                };
                assignmentButton.Click += (s, e) =>
                {
                    if (s is Button btn && btn.Tag is Assignment a)
                    {
                        OnAssignmentClicked(a);
                    }
                };
                cellStack.Children.Add(assignmentButton);
            }

            if (assignments.Count > 2)
            {
                var moreText = new TextBlock
                {
                    Text = $"+{assignments.Count - 2}",
                    FontSize = 8,
                    Foreground = (SolidColorBrush)FindResource("MaterialDesignBodyLight"),
                    FontFamily = new System.Windows.Media.FontFamily("Segoe UI")
                };
                cellStack.Children.Add(moreText);
            }

            var isToday = date.Date == DateTime.Today;
            var border = new Border
            {
                Background = System.Windows.Media.Brushes.Transparent,
                BorderBrush = isToday ? (SolidColorBrush)FindResource("SecondaryHueMid") : (SolidColorBrush)FindResource("MaterialDesignDivider"),
                BorderThickness = new Thickness(isToday ? 2 : 1),
                CornerRadius = new CornerRadius(4),
                Padding = new Thickness(8),
                Child = cellStack,
                MinHeight = 80,
                MinWidth = 80
            };

            // Add hover event to show/hide add button
            border.MouseEnter += (s, e) =>
            {
                if (isOutsideMonth) return;
                
                var addButton = new Button
                {
                    Content = "+",
                    FontSize = 9,
                    Padding = new Thickness(2, 1, 2, 1),
                    Margin = new Thickness(0, 2, 0, 0),
                    Background = System.Windows.Media.Brushes.Transparent,
                    Foreground = (SolidColorBrush)FindResource("SecondaryHueMid"),
                    BorderThickness = new Thickness(1),
                    BorderBrush = (SolidColorBrush)FindResource("SecondaryHueMid"),
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    Cursor = System.Windows.Input.Cursors.Hand,
                    FontFamily = new System.Windows.Media.FontFamily("Segoe UI"),
                    FontWeight = System.Windows.FontWeights.Bold,
                    Tag = date
                };

                addButton.Click += (sender, args) =>
                {
                    OnCreateAssignmentForDate(date);
                };

                cellStack.Children.Add(addButton);
            };

            border.MouseLeave += (s, e) =>
            {
                // Remove the add button if it's the last child and it's a + button
                if (cellStack.Children.Count > 0)
                {
                    var lastChild = cellStack.Children[cellStack.Children.Count - 1];
                    if (lastChild is Button btn && btn.Content?.ToString() == "+")
                    {
                        cellStack.Children.Remove(lastChild);
                    }
                }
            };

            return border;
        }

        private Border CreateAssignmentCard(Assignment assignment)
        {
            var cardStack = new StackPanel { Orientation = Orientation.Vertical };

            var nameText = new TextBlock
            {
                Text = assignment.Name,
                FontWeight = System.Windows.FontWeights.Bold,
                Foreground = (SolidColorBrush)FindResource("MaterialDesignBody"),
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(0, 0, 0, 8),
                FontFamily = new System.Windows.Media.FontFamily("Segoe UI"),
                FontSize = 14
            };
            cardStack.Children.Add(nameText);

            var courseText = new TextBlock
            {
                Text = assignment.LectureSection,
                Foreground = (SolidColorBrush)FindResource("MaterialDesignBodyLight"),
                FontSize = 11,
                Margin = new Thickness(0, 0, 0, 4),
                FontFamily = new System.Windows.Media.FontFamily("Segoe UI")
            };
            cardStack.Children.Add(courseText);

            var timeText = new TextBlock
            {
                Text = $"Due: {System.DateTime.MinValue.Add(assignment.DueTime):hh\\:mm\\ tt}",
                Foreground = (SolidColorBrush)FindResource("SecondaryHueMid"),
                FontSize = 11,
                FontWeight = System.Windows.FontWeights.SemiBold,
                Margin = new Thickness(0, 0, 0, 4),
                FontFamily = new System.Windows.Media.FontFamily("Segoe UI")
            };
            cardStack.Children.Add(timeText);

            if (!string.IsNullOrEmpty(assignment.Description))
            {
                var descText = new TextBlock
                {
                    Text = assignment.Description,
                    Foreground = (SolidColorBrush)FindResource("MaterialDesignBodyLight"),
                    FontSize = 10,
                    TextWrapping = TextWrapping.Wrap,
                    Margin = new Thickness(0, 0, 0, 4),
                    FontFamily = new System.Windows.Media.FontFamily("Segoe UI")
                };
                cardStack.Children.Add(descText);
            }

            if (!string.IsNullOrEmpty(assignment.SubmissionUrl))
            {
                var linkText = new TextBlock
                {
                    Text = "📎 Submission Link",
                    Foreground = (SolidColorBrush)FindResource("SecondaryHueMid"),
                    FontSize = 10,
                    TextDecorations = System.Windows.TextDecorations.Underline,
                    Cursor = System.Windows.Input.Cursors.Hand,
                    FontFamily = new System.Windows.Media.FontFamily("Segoe UI")
                };
                linkText.MouseLeftButtonDown += (s, e) =>
                {
                    try
                    {
                        string url = assignment.SubmissionUrl;
                        // Ensure the URL has a scheme (http:// or https://)
                        if (!url.StartsWith("http://") && !url.StartsWith("https://"))
                        {
                            url = "https://" + url;
                        }
                        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                        {
                            FileName = url,
                            UseShellExecute = true
                        });
                    }
                    catch { }
                };
                cardStack.Children.Add(linkText);
            }

            var card = new Border
            {
                Background = (SolidColorBrush)FindResource("MaterialDesignCardBackground"),
                BorderBrush = (SolidColorBrush)FindResource("MaterialDesignDivider"),
                BorderThickness = new Thickness(1),
                CornerRadius = new CornerRadius(4),
                Padding = new Thickness(12),
                Child = cardStack,
                Margin = new Thickness(0, 8, 0, 0),
                Cursor = System.Windows.Input.Cursors.Hand,
                Tag = assignment
            };
            
            card.MouseLeftButtonDown += (s, e) =>
            {
                OnAssignmentClicked(assignment);
                e.Handled = true;
            };

            return card;
        }
    }
}


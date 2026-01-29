using System.Windows;
using WpfApp1.ViewModels;

namespace WpfApp1
{
    public partial class EditAssignmentWindow : Window
    {
        private AssignmentViewModel _viewModel;
        private bool _isNewAssignment;

        public EditAssignmentWindow(AssignmentViewModel viewModel, bool isNewAssignment)
        {
            InitializeComponent();
            _viewModel = viewModel;
            _isNewAssignment = isNewAssignment;
            DataContext = _viewModel;

            if (isNewAssignment)
            {
                HeaderTitle.Text = "Create New Assignment";
                SaveButton.Content = "Create";
                DeleteButton.Visibility = Visibility.Collapsed;
            }
            else
            {
                HeaderTitle.Text = "Edit Assignment";
                SaveButton.Content = "Save";
                DeleteButton.Visibility = Visibility.Visible;
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(_viewModel.CurrentAssignment.Name))
            {
                MessageBox.Show("Please enter an assignment name.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (_isNewAssignment)
            {
                _viewModel.AddAssignment();
                MessageBox.Show("Assignment created successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                _viewModel.UpdateAssignment();
                MessageBox.Show("Assignment updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }

            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (_viewModel.SelectedAssignment == null)
                return;

            var result = MessageBox.Show(
                $"Are you sure you want to delete '{_viewModel.SelectedAssignment.Name}'?",
                "Confirm Delete",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                _viewModel.DeleteAssignment();
                MessageBox.Show("Assignment deleted successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                DialogResult = true;
                Close();
            }
        }
    }
}

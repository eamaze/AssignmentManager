using System;
using System.Windows;
using System.Windows.Controls;
using WpfApp1.Services;

namespace WpfApp1.Controls
{
    public partial class SettingsPanel : UserControl
    {
        private BrightspaceApiService _apiService;
        private BrightspaceCredentialsManager _credentialsManager;
        private bool _isConnected = false;

        public event EventHandler SyncCompleted;

        public SettingsPanel()
        {
            InitializeComponent();
            _credentialsManager = new BrightspaceCredentialsManager();
            _apiService = new BrightspaceApiService();
            LoadSavedSettings();
        }

        private void LoadSavedSettings()
        {
            try
            {
                var credentials = _credentialsManager.LoadCredentials();
                if (credentials != null)
                {
                    BrightspaceUrlTextBox.Text = credentials.BrightspaceUrl;
                    ApiTokenPasswordBox.Password = credentials.AccessToken;
                    AutoSyncCheckBox.IsChecked = credentials.AutoSync;
                    UpdateConnectionStatus();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading settings: {ex.Message}");
            }
        }

        private async void TestConnectionButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(BrightspaceUrlTextBox.Text) || 
                string.IsNullOrWhiteSpace(ApiTokenPasswordBox.Password))
            {
                ConnectionStatusText.Text = "Connection Status: Please enter both URL and token";
                ConnectionStatusText.Foreground = (System.Windows.Media.Brush)FindResource("MaterialDesignBodyLight");
                return;
            }

            TestConnectionButton.IsEnabled = false;
            ConnectionStatusText.Text = "Connection Status: Testing...";

            try
            {
                _apiService.Initialize(BrightspaceUrlTextBox.Text, ApiTokenPasswordBox.Password);
                bool isValid = await _apiService.ValidateConnectionAsync();

                if (isValid)
                {
                    _isConnected = true;
                    SyncNowButton.IsEnabled = true;
                    ConnectionStatusText.Text = "Connection Status: ? Connected successfully";
                    ConnectionStatusText.Foreground = (System.Windows.Media.Brush)FindResource("SecondaryHueMid");
                    MessageBox.Show("Connection successful!", "Brightspace API", 
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    _isConnected = false;
                    SyncNowButton.IsEnabled = false;
                    ConnectionStatusText.Text = "Connection Status: ? Connection failed";
                    ConnectionStatusText.Foreground = (System.Windows.Media.Brush)FindResource("MaterialDesignBodyLight");
                    MessageBox.Show("Failed to connect to Brightspace. Please check your credentials.", 
                        "Connection Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                _isConnected = false;
                SyncNowButton.IsEnabled = false;
                ConnectionStatusText.Text = "Connection Status: ? Error occurred";
                MessageBox.Show($"Error: {ex.Message}", "Connection Error", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                TestConnectionButton.IsEnabled = true;
            }
        }

        private void SaveSettingsButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(BrightspaceUrlTextBox.Text))
            {
                MessageBox.Show("Please enter a Brightspace URL", "Validation Error", 
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var credentials = new BrightspaceCredentials
                {
                    BrightspaceUrl = BrightspaceUrlTextBox.Text,
                    AccessToken = ApiTokenPasswordBox.Password,
                    AutoSync = AutoSyncCheckBox.IsChecked ?? false
                };

                _credentialsManager.SaveCredentials(credentials);
                MessageBox.Show("Settings saved successfully!", "Success", 
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving settings: {ex.Message}", "Error", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void SyncNowButton_Click(object sender, RoutedEventArgs e)
        {
            if (!_isConnected)
            {
                MessageBox.Show("Please test the connection first", "Not Connected", 
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            SyncNowButton.IsEnabled = false;
            SyncStatusText.Text = "Syncing assignments...";

            try
            {
                var storageService = new AssignmentStorageService();
                var syncedCount = await _apiService.SyncAssignmentsAsync(storageService);
                
                SyncStatusText.Text = $"? Synced {syncedCount.Count} new assignments";
                SyncStatusText.Foreground = (System.Windows.Media.Brush)FindResource("SecondaryHueMid");
                
                SyncCompleted?.Invoke(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                SyncStatusText.Text = $"? Sync failed: {ex.Message}";
                SyncStatusText.Foreground = (System.Windows.Media.Brush)FindResource("MaterialDesignBodyLight");
                System.Diagnostics.Debug.WriteLine($"Sync error: {ex.Message}");
            }
            finally
            {
                SyncNowButton.IsEnabled = _isConnected;
            }
        }

        private void ClearCredentialsButton_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Are you sure you want to clear Brightspace credentials?", 
                "Confirm", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                _credentialsManager.ClearCredentials();
                BrightspaceUrlTextBox.Clear();
                ApiTokenPasswordBox.Clear();
                AutoSyncCheckBox.IsChecked = false;
                _isConnected = false;
                SyncNowButton.IsEnabled = false;
                UpdateConnectionStatus();
                MessageBox.Show("Credentials cleared", "Success", 
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void UpdateConnectionStatus()
        {
            if (!string.IsNullOrWhiteSpace(BrightspaceUrlTextBox.Text))
            {
                ConnectionStatusText.Text = "Connection Status: Not Tested";
                ConnectionStatusText.Foreground = (System.Windows.Media.Brush)FindResource("MaterialDesignBodyLight");
            }
            else
            {
                ConnectionStatusText.Text = "Connection Status: Not Configured";
            }
        }

        public async void PerformAutoSync()
        {
            if (AutoSyncCheckBox.IsChecked == true)
            {
                var credentials = _credentialsManager.LoadCredentials();
                if (credentials != null)
                {
                    _apiService.Initialize(credentials.BrightspaceUrl, credentials.AccessToken);
                    bool isValid = await _apiService.ValidateConnectionAsync();
                    
                    if (isValid)
                    {
                        await System.Threading.Tasks.Task.Delay(500); // Brief delay
                        SyncNowButton_Click(this, null);
                    }
                }
            }
        }
    }
}

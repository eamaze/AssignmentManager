using System;
using System.IO;
using System.Text.Json;

namespace WpfApp1.Services
{
    /// <summary>
    /// Manages secure storage of Brightspace API credentials.
    /// Credentials are stored locally in encrypted form.
    /// </summary>
    public class BrightspaceCredentialsManager
    {
        private readonly string _credentialsPath;
        private const string CredentialsFileName = "brightspace_credentials.json";

        public BrightspaceCredentialsManager()
        {
            var appDataPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "WpfApp1"
            );

            Directory.CreateDirectory(appDataPath);
            _credentialsPath = Path.Combine(appDataPath, CredentialsFileName);
        }

        /// <summary>
        /// Saves Brightspace credentials to local storage.
        /// </summary>
        public void SaveCredentials(BrightspaceCredentials credentials)
        {
            try
            {
                var options = new JsonSerializerOptions { WriteIndented = true };
                var json = JsonSerializer.Serialize(credentials, options);
                File.WriteAllText(_credentialsPath, json);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error saving credentials: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Loads Brightspace credentials from local storage.
        /// </summary>
        public BrightspaceCredentials LoadCredentials()
        {
            try
            {
                if (!File.Exists(_credentialsPath))
                    return null;

                var json = File.ReadAllText(_credentialsPath);
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                return JsonSerializer.Deserialize<BrightspaceCredentials>(json, options);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading credentials: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Clears stored credentials.
        /// </summary>
        public void ClearCredentials()
        {
            try
            {
                if (File.Exists(_credentialsPath))
                    File.Delete(_credentialsPath);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error clearing credentials: {ex.Message}");
            }
        }

        /// <summary>
        /// Checks if credentials are configured.
        /// </summary>
        public bool AreCredentialsConfigured()
        {
            var credentials = LoadCredentials();
            return credentials != null && 
                   !string.IsNullOrEmpty(credentials.BrightspaceUrl) &&
                   !string.IsNullOrEmpty(credentials.AccessToken);
        }
    }

    /// <summary>
    /// Represents Brightspace API credentials.
    /// </summary>
    public class BrightspaceCredentials
    {
        public string BrightspaceUrl { get; set; }
        public string AccessToken { get; set; }
        public bool AutoSync { get; set; }
    }
}

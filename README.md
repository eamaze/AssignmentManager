# Assignment Manager

A modern, feature-rich desktop application for managing academic assignments and syncing with Brightspace (D2L Learning Management System). Built with WPF and .NET 10, Assignment Manager helps students stay organized by consolidating assignments from multiple courses into a single intuitive interface.

![License](https://img.shields.io/badge/license-MIT-blue.svg)
![Platform](https://img.shields.io/badge/platform-Windows-brightgreen.svg)
![.NET](https://img.shields.io/badge/.NET-10-blueviolet.svg)

---

## ? Features

### ?? Assignment Management
- **Create, Edit & Delete Assignments** - Full CRUD operations for managing your academic workload
- **Rich Assignment Details** - Store assignment names, descriptions, due dates, times, course information, and submission URLs
- **Interactive Calendar View** - Visualize your assignments with a custom calendar control showing deadlines at a glance
- **Quick Search & Browse** - Easily find assignments and view all upcoming tasks

### ?? Brightspace Integration
- **Automatic Sync** - Pull assignments directly from your Brightspace (D2L) courses
- **Secure Credential Storage** - Store your Brightspace API credentials securely in local storage
- **One-Click Sync** - Manually sync assignments at any time with the "Sync Now" button
- **Auto-Sync on Startup** - Option to automatically sync Brightspace assignments when the application launches
- **Connection Validation** - Test your Brightspace connection before syncing to ensure credentials are correct

### ?? Data Persistence
- **Automatic Saving** - All changes are automatically saved to local storage
- **JSON-Based Storage** - Assignments stored in human-readable JSON format
- **Cross-Session Persistence** - Assignments persist between application sessions
- **Sample Data** - New installations include helpful sample assignments to get you started

### ?? Modern User Interface
- **Material Design** - Clean, modern interface built with Material Design principles
- **Tab-Based Navigation** - Organize features into Calendar and Settings tabs
- **Responsive Layout** - Adapts to different window sizes with smooth scrolling
- **Notification System** - Visual feedback for all user actions (create, update, delete, sync)
- **Dark/Light Theme Support** - Material Design theme system

### ?? Settings & Configuration
- **Brightspace Credentials** - Manage your Brightspace URL and API access token
- **Auto-Sync Preferences** - Enable or disable automatic synchronization on startup
- **Clear Credentials** - Securely remove stored credentials from your machine
- **Connection Testing** - Validate Brightspace setup without syncing data

---

## ?? Getting Started

### Prerequisites
- Windows 7 or later
- .NET 10 runtime (or newer)
- Brightspace account with API access (optional, for sync features)

### Installation
1. Clone the repository:
   ```bash
   git clone https://github.com/eamaze/AssignmentManager
   cd AssignmentManager
   ```

2. Build the project:
   ```bash
   dotnet build
   ```

3. Run the application:
   ```bash
   dotnet run --project WpfApp1
   ```

### First Run
On your first launch, Assignment Manager will:
- Create a local data directory in `%AppData%\WpfApp1`
- Load sample assignments to help you get familiar with the interface
- Automatically save your workspace

---

## ?? Brightspace Integration Setup

### Step 1: Generate API Credentials
1. Log in to your Brightspace instance
2. Navigate to **User Profile** ? **API Keys** (or **Developer Tools**)
3. Create a new API key and note:
   - **Brightspace URL**: Full URL to your Brightspace instance (e.g., `https://myuniversity.brightspace.com`)
   - **Access Token**: Your generated API access token

### Step 2: Configure in Assignment Manager
1. Open the Assignment Manager application
2. Click the **Settings** tab
3. Enter your Brightspace credentials:
   - **Brightspace URL**: Full URL to your Brightspace instance
   - **API Access Token**: Your generated API token
4. Click **Test Connection** to verify credentials
5. Click **Save Settings** to store credentials locally

### Step 3: Start Syncing
- Click **Sync Now** to immediately pull assignments from your courses
- Enable **Automatically sync Brightspace assignments on startup** to sync automatically

For detailed troubleshooting and more information, see [BRIGHTSPACE_INTEGRATION.md](WpfApp1/BRIGHTSPACE_INTEGRATION.md).

---

## ?? Data Storage

### Storage Locations
- **Assignments**: `%AppData%\WpfApp1\assignments.json`
- **Brightspace Credentials**: `%AppData%\WpfApp1\brightspace_credentials.json`

On Windows, `%AppData%` typically expands to `C:\Users\[YourUsername]\AppData\Roaming\`

### Data Format
Assignments are stored in a clean JSON format:
```json
[
  {
    "id": 1,
    "name": "Project Proposal",
    "description": "Submit project proposal for approval",
    "dueDate": "2024-12-31T00:00:00",
    "dueTime": "17:00:00",
    "lectureSection": "CS 101",
    "submissionUrl": "https://classroom.example.com"
  }
]
```

For more details, see [PERSISTENCE_DOCS.md](WpfApp1/PERSISTENCE_DOCS.md).

---

## ??? Project Structure

```
WpfApp1/
??? Controls/                    # Custom UI controls
?   ??? CustomCalendar.xaml     # Calendar control for assignment visualization
?   ??? CustomCalendar.xaml.cs
?   ??? SettingsPanel.xaml      # Settings panel for configuration
?   ??? SettingsPanel.xaml.cs
??? Services/                    # Business logic services
?   ??? BrightspaceApiService.cs # Brightspace API integration
?   ??? BrightspaceCredentialsManager.cs # Credential management
?   ??? AssignmentStorageService.cs # Data persistence
??? ViewModels/                  # MVVM ViewModels
?   ??? AssignmentViewModel.cs  # Main application ViewModel
??? Models/                      # Data models
?   ??? Assignment.cs           # Assignment data model
??? MainWindow.xaml             # Main application window
??? MainWindow.xaml.cs
??? App.xaml                    # Application configuration
??? README.md
```

---

## ?? Architecture

Assignment Manager follows the **MVVM (Model-View-ViewModel)** pattern:

- **Models**: Data classes representing assignments
- **Views**: XAML files defining the UI (MainWindow, SettingsPanel, CustomCalendar)
- **ViewModels**: `AssignmentViewModel` handles state and business logic
- **Services**: Separate services for API calls, credential management, and data persistence

This architecture ensures clean separation of concerns and makes the codebase easy to test and maintain.

---

## ?? Security

- **Local Storage Only**: Your data never leaves your machine unless you explicitly sync with Brightspace
- **Secure Credential Handling**: API credentials are stored locally and never transmitted unnecessarily
- **Token-Based Authentication**: Uses OAuth 2.0 Bearer token authentication with Brightspace
- **No Password Storage**: Only API tokens are stored; never your Brightspace password
- **Clear Credentials Option**: Easily remove stored credentials from your machine at any time

---

## ??? Development

### Building from Source
```bash
dotnet build WpfApp1/WpfApp1.csproj
```

### Running Tests
```bash
dotnet test
```

### Building for Release
```bash
dotnet publish -c Release -o ./publish
```

### Technologies Used
- **Framework**: WPF (Windows Presentation Foundation)
- **.NET Version**: .NET 10
- **UI Framework**: Material Design in XAML
- **Architecture**: MVVM Pattern
- **Data Format**: JSON
- **API Integration**: Brightspace REST API

---

## ?? Usage Examples

### Creating an Assignment
1. Click the **Create New Assignment** button
2. Fill in the assignment details:
   - Assignment Name
   - Description
   - Due Date & Time
   - Lecture Section/Course
   - Submission URL (optional)
3. Click **Create** to save

### Editing an Assignment
1. Click on an assignment card in the calendar view
2. Click the **Edit** button in the details popup
3. Modify the assignment information
4. Click **Save** to apply changes

### Syncing with Brightspace
1. Configure your Brightspace credentials in the **Settings** tab
2. Click **Sync Now** to pull all assignments from your courses
3. Review the sync status message for confirmation

### Viewing Assignments
- **Calendar View**: See all assignments on the custom calendar, organized by due date
- **List View**: Browse all assignments with quick details
- **Details Popup**: Click any assignment to view full information

---

## ?? Troubleshooting

### Brightspace Connection Issues
- Verify your Brightspace URL is correct (should not include `/d2l/` or `/api/`)
- Check that your API token is valid and hasn't expired
- Ensure your Brightspace instance is accessible from your network
- Contact your institution's Brightspace administrator if issues persist

### Assignments Not Saving
- Check that `%AppData%\WpfApp1\` directory has write permissions
- Verify your disk has sufficient space
- Try restarting the application

### Sync Not Working
- Ensure you have at least one course enrolled in Brightspace
- Verify your courses have assignments with due dates
- Test your connection using the **Test Connection** button

For more detailed troubleshooting, see the [Brightspace Integration Documentation](WpfApp1/BRIGHTSPACE_INTEGRATION.md).

---

## ?? License

This project is licensed under the MIT License - see the LICENSE file for details.

---

## ?? Contributing

Contributions are welcome! Please feel free to:
1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

---

## ?? Support

For issues, questions, or suggestions:
- Open an [issue on GitHub](https://github.com/eamaze/AssignmentManager/issues)
- Check the [Brightspace Integration Guide](WpfApp1/BRIGHTSPACE_INTEGRATION.md)
- Review the [Data Persistence Documentation](WpfApp1/PERSISTENCE_DOCS.md)

---

## ?? Acknowledgments

- Built with [Material Design in XAML](https://github.com/MaterialDesignInXAML/MaterialDesignInXamlToolkit)
- Brightspace API integration based on [official documentation](https://docs.valence.desire2learn.com/)
- Inspired by the need for better assignment management in academic settings

---

**Made with ?? for students and educators**
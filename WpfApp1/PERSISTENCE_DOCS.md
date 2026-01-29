# Assignment Persistence Documentation

## Overview
The Assignment Manager application now automatically saves all assignments to persistent storage. Assignments are saved as a JSON file in the user's AppData folder and are automatically loaded when the application starts.

## Storage Location
Assignments are stored at:
```
%AppData%\WpfApp1\assignments.json
```

On Windows, this typically expands to:
```
C:\Users\[YourUsername]\AppData\Roaming\WpfApp1\assignments.json
```

## How It Works

### Automatic Loading
- When the application starts, it automatically loads assignments from the JSON file
- If no saved assignments exist (first run), sample assignments are loaded and saved
- The NextId counter is automatically adjusted to prevent duplicate assignment IDs

### Automatic Saving
Assignments are saved automatically when:
- **Creating** a new assignment (via "Create New Assignment" button)
- **Updating** an existing assignment (via the edit popup)
- **Deleting** an assignment (via the delete button in the popup)

### Manual Saving
You can also manually save all assignments in code:
```csharp
_viewModel.SaveAllAssignments();
```

## Implementation Details

### AssignmentStorageService
The `AssignmentStorageService` class in `WpfApp1/Services/AssignmentStorageService.cs` handles all storage operations:

- `LoadAssignments()` - Loads assignments from the JSON file
- `SaveAssignments(List<Assignment>)` - Saves the entire list to JSON
- `SaveAssignment(Assignment)` - Saves or updates a single assignment
- `DeleteAssignment(int)` - Deletes a specific assignment by ID

### Integration
The `AssignmentViewModel` uses the storage service to:
1. Load assignments on initialization
2. Save when assignments are added, updated, or deleted
3. Automatically update the NextId counter based on stored assignments

## JSON Format
The assignments are stored in the following JSON format:
```json
[
  {
    "id": 1,
    "name": "Assignment 1",
    "description": "Complete chapter 1-3",
    "dueDate": "2024-12-31T00:00:00",
    "dueTime": "17:00:00",
    "lectureSection": "CS 101",
    "submissionUrl": "https://classroom.google.com"
  },
  ...
]
```

## First Run
On the first run, if no assignments exist:
1. Sample assignments are loaded into the application
2. These sample assignments are immediately saved to storage
3. On subsequent runs, these saved assignments will be loaded

## Debugging
To verify the storage location and debug persistence issues, you can:
1. Check the debug output in Visual Studio
2. Navigate to `%AppData%\WpfApp1\` to view the JSON file directly
3. The storage service includes error handling that logs to the debug output

## Notes
- The application directory is automatically created if it doesn't exist
- JSON is formatted with indentation for easy manual inspection
- All datetime values are stored in ISO 8601 format
- The storage service handles missing or corrupted files gracefully by returning an empty list

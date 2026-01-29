# Brightspace API Integration Documentation

## Overview

The Assignment Manager now supports integration with Brightspace (D2L Learning Management System) to automatically sync assignments to your calendar. This feature allows you to pull assignments directly from your Brightspace courses.

## Features

- **Automatic Assignment Sync**: Retrieve assignments from all your enrolled Brightspace courses
- **Secure Credential Storage**: Your API credentials are stored locally and securely
- **One-Click Sync**: Manually sync assignments at any time
- **Auto-Sync Option**: Automatically sync assignments on application startup
- **Connection Validation**: Test your Brightspace connection before syncing

## Setting Up Brightspace Integration

### Step 1: Get Your Brightspace Credentials

1. Log in to your Brightspace instance (https://your-institution.brightspace.com)
2. Navigate to **User Profile** > **API Keys** (or **Developer Tools**)
3. Create a new API key and note:
   - **Brightspace URL**: The base URL of your Brightspace instance (e.g., https://myuniversity.brightspace.com)
   - **Access Token**: Your generated API access token

### Step 2: Configure in Assignment Manager

1. Open the Assignment Manager application
2. Go to the **Settings** tab
3. Enter your Brightspace credentials:
   - **Brightspace URL**: Full URL to your Brightspace instance
   - **API Access Token**: Your generated API token
4. Click **Test Connection** to verify your credentials are correct
5. Click **Save Settings** to store your credentials locally

### Step 3: Sync Assignments

Once connected, you can:
- **Manual Sync**: Click the **Sync Now** button to immediately pull assignments
- **Auto-Sync**: Check the "Automatically sync Brightspace assignments on startup" option to sync automatically when the app starts

## API Endpoints Used

The integration uses the following Brightspace API endpoints:

- **Authentication**: Bearer token-based authentication
- **Enrollments**: `GET /d2l/api/lp/1.0/enrollments/myenrollments/` - Retrieves your enrolled courses
- **Assignments**: `GET /d2l/api/le/1.0/courses/{courseId}/dropbox/folders/` - Retrieves assignments for each course
- **Validation**: `GET /d2l/api/lp/1.0/users/whoami` - Validates API connection

## Data Synced

When you sync Brightspace assignments, the following information is imported:

| Field | Source | Description |
|-------|--------|-------------|
| Name | Assignment Name | The title of the assignment |
| Course | Course Name | The course the assignment belongs to |
| Description | Assignment Description | Details about the assignment |
| Due Date | Due Date | When the assignment is due |
| Due Time | Due Date/Time | When the assignment deadline is |
| Submission URL | Brightspace Link | Direct link to submit the assignment |

## Security

- **Local Storage**: Your credentials are stored in your user's AppData folder (`%APPDATA%\WpfApp1\brightspace_credentials.json`)
- **Token-Based Auth**: The application uses OAuth 2.0 Bearer token authentication
- **No Password Storage**: Your Brightspace password is never stored - use API tokens instead
- **Clear Credentials**: You can clear your stored credentials at any time from the Settings tab

## Troubleshooting

### Connection Failed

If you get a "Connection failed" error:
1. Verify your Brightspace URL is correct (should not include `/d2l/` or `/api/`)
2. Check that your API token is valid and hasn't expired
3. Ensure your Brightspace instance is accessible from your network
4. Contact your institution's Brightspace administrator if issues persist

### No Assignments Synced

If assignments aren't appearing:
1. Ensure you have at least one course enrolled in Brightspace
2. Check that your courses have assignments with due dates
3. Try clicking **Sync Now** again to force a refresh
4. Check the sync status message at the bottom of the Settings tab

### Missing Courses

The integration syncs only courses with a valid enrollment. If a course is missing:
1. Verify you're actually enrolled in that course on Brightspace
2. Check that your role allows viewing assignments
3. Re-sync after waiting a moment for Brightspace to process your enrollment

## API Documentation Reference

For more information about the Brightspace API, visit:
- **Official Documentation**: https://docs.valence.desire2learn.com/
- **Brightspace Developer Community**: https://community.d2l.com/brightspace/group/29-developers
- **Getting Started Guide**: https://docs.valence.desire2learn.com/gettingstarted.html

## Support

If you encounter any issues with the Brightspace integration:
1. Check the troubleshooting section above
2. Verify your credentials are correct
3. Contact your institution's Brightspace administrator
4. Report issues with detailed error messages for further assistance

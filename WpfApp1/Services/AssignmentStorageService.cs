using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using WpfApp1.Models;

namespace WpfApp1.Services
{
    /// <summary>
    /// Service for persisting assignments to and from JSON files.
    /// Stores data in %appdata%\WpfApp1\assignments.json
    /// </summary>
    public class AssignmentStorageService
    {
        private readonly string _appDataPath;
        private readonly string _assignmentsFilePath;
        private readonly string _initializationFlagPath;
        private readonly JsonSerializerOptions _jsonOptions;

        public AssignmentStorageService()
        {
            // Create application-specific folder in AppData
            _appDataPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "WpfApp1"
            );

            _assignmentsFilePath = Path.Combine(_appDataPath, "assignments.json");
            _initializationFlagPath = Path.Combine(_appDataPath, ".initialized");

            // Configure JSON serializer options
            _jsonOptions = new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNameCaseInsensitive = true,
                Converters = { new JsonStringEnumConverter() }
            };

            // Ensure the directory exists
            EnsureDirectoryExists();
        }

        /// <summary>
        /// Loads all assignments from the JSON file.
        /// Returns an empty list if the file doesn't exist.
        /// </summary>
        public List<Assignment> LoadAssignments()
        {
            try
            {
                if (!File.Exists(_assignmentsFilePath))
                {
                    return new List<Assignment>();
                }

                string json = File.ReadAllText(_assignmentsFilePath);
                if (string.IsNullOrWhiteSpace(json))
                {
                    return new List<Assignment>();
                }

                var assignments = JsonSerializer.Deserialize<List<Assignment>>(json, _jsonOptions);
                return assignments ?? new List<Assignment>();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading assignments: {ex.Message}");
                return new List<Assignment>();
            }
        }

        /// <summary>
        /// Checks if this is the first time the app has been run.
        /// </summary>
        public bool IsFirstTimeInitialization()
        {
            return !File.Exists(_initializationFlagPath);
        }

        /// <summary>
        /// Marks the app as initialized (called after loading sample data for the first time).
        /// </summary>
        public void MarkAsInitialized()
        {
            try
            {
                EnsureDirectoryExists();
                File.WriteAllText(_initializationFlagPath, "initialized");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error marking as initialized: {ex.Message}");
            }
        }

        /// <summary>
        /// Saves all assignments to the JSON file.
        /// </summary>
        public void SaveAssignments(List<Assignment> assignments)
        {
            try
            {
                EnsureDirectoryExists();

                string json = JsonSerializer.Serialize(assignments, _jsonOptions);
                File.WriteAllText(_assignmentsFilePath, json);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error saving assignments: {ex.Message}");
            }
        }

        /// <summary>
        /// Saves a single assignment to the collection.
        /// </summary>
        public void SaveAssignment(Assignment assignment)
        {
            var assignments = LoadAssignments();
            
            // Find and update existing assignment or add new one
            var existingIndex = assignments.FindIndex(a => a.Id == assignment.Id);
            if (existingIndex >= 0)
            {
                assignments[existingIndex] = assignment;
            }
            else
            {
                assignments.Add(assignment);
            }

            SaveAssignments(assignments);
        }

        /// <summary>
        /// Deletes a single assignment from the collection.
        /// </summary>
        public void DeleteAssignment(int assignmentId)
        {
            var assignments = LoadAssignments();
            assignments.RemoveAll(a => a.Id == assignmentId);
            SaveAssignments(assignments);
        }

        /// <summary>
        /// Gets the storage file path (for debugging/testing purposes).
        /// </summary>
        public string GetStorageFilePath()
        {
            return _assignmentsFilePath;
        }

        private void EnsureDirectoryExists()
        {
            if (!Directory.Exists(_appDataPath))
            {
                Directory.CreateDirectory(_appDataPath);
            }
        }
    }
}

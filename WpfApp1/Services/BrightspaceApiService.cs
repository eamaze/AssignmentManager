using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using WpfApp1.Models;

namespace WpfApp1.Services
{
    /// <summary>
    /// Service for integrating with Brightspace (D2L) API to sync assignments.
    /// Brightspace API Documentation: https://docs.valence.desire2learn.com/reference.html
    /// </summary>
    public class BrightspaceApiService
    {
        private string _baseUrl;
        private string _accessToken;
        private readonly HttpClient _httpClient;
        private const string ApiVersion = "1.0";

        public BrightspaceApiService()
        {
            _httpClient = new HttpClient();
        }

        /// <summary>
        /// Initializes the service with Brightspace credentials.
        /// </summary>
        public void Initialize(string brightspaceUrl, string accessToken)
        {
            _baseUrl = brightspaceUrl.TrimEnd('/');
            _accessToken = accessToken;
            
            // Set default headers for Brightspace API
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        /// <summary>
        /// Checks if the service is properly initialized with valid credentials.
        /// </summary>
        public bool IsInitialized => !string.IsNullOrEmpty(_baseUrl) && !string.IsNullOrEmpty(_accessToken);

        /// <summary>
        /// Validates the API connection by fetching user information.
        /// </summary>
        public async Task<bool> ValidateConnectionAsync()
        {
            try
            {
                if (!IsInitialized) return false;

                var response = await _httpClient.GetAsync($"{_baseUrl}/d2l/api/lp/{ApiVersion}/users/whoami");
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Retrieves all courses for the current user.
        /// </summary>
        public async Task<List<BrightspaceCourse>> GetCoursesAsync()
        {
            try
            {
                if (!IsInitialized)
                    throw new InvalidOperationException("API service not initialized. Please provide credentials.");

                var response = await _httpClient.GetAsync($"{_baseUrl}/d2l/api/lp/{ApiVersion}/enrollments/myenrollments/?roleId=0&limit=200");
                
                if (!response.IsSuccessStatusCode)
                    return new List<BrightspaceCourse>();

                var json = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var result = JsonSerializer.Deserialize<BrightspaceEnrollmentResponse>(json, options);

                return result?.Items?.ToList() ?? new List<BrightspaceCourse>();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error fetching courses: {ex.Message}");
                return new List<BrightspaceCourse>();
            }
        }

        /// <summary>
        /// Retrieves assignments for a specific course.
        /// </summary>
        public async Task<List<BrightspaceAssignment>> GetAssignmentsByCourseAsync(string courseId)
        {
            try
            {
                if (!IsInitialized)
                    throw new InvalidOperationException("API service not initialized. Please provide credentials.");

                // Get dropbox info (assignments)
                var response = await _httpClient.GetAsync(
                    $"{_baseUrl}/d2l/api/le/{ApiVersion}/courses/{courseId}/dropbox/folders/?limit=200");

                if (!response.IsSuccessStatusCode)
                    return new List<BrightspaceAssignment>();

                var json = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var result = JsonSerializer.Deserialize<BrightspaceAssignmentResponse>(json, options);

                return result?.Objects?.ToList() ?? new List<BrightspaceAssignment>();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error fetching assignments for course {courseId}: {ex.Message}");
                return new List<BrightspaceAssignment>();
            }
        }

        /// <summary>
        /// Syncs assignments from Brightspace to the local database.
        /// </summary>
        public async Task<List<Assignment>> SyncAssignmentsAsync(AssignmentStorageService storageService)
        {
            var syncedAssignments = new List<Assignment>();

            try
            {
                if (!IsInitialized)
                    throw new InvalidOperationException("API service not initialized. Please provide credentials.");

                // Get all courses
                var courses = await GetCoursesAsync();

                // Get assignments for each course
                foreach (var course in courses)
                {
                    var assignments = await GetAssignmentsByCourseAsync(course.CourseId);

                    foreach (var brightspaceAssignment in assignments)
                    {
                        // Create Assignment from Brightspace data
                        var assignment = new Assignment
                        {
                            Id = brightspaceAssignment.Id,
                            Name = brightspaceAssignment.Name,
                            Description = brightspaceAssignment.Description ?? "",
                            DueDate = brightspaceAssignment.DueDate ?? DateTime.Now.AddDays(7),
                            DueTime = brightspaceAssignment.DueDate?.TimeOfDay ?? new TimeSpan(23, 59, 0),
                            LectureSection = course.CourseName,
                            SubmissionUrl = $"{_baseUrl}/d2l/lms/dropbox/user/folders/submit/{course.CourseId}/{brightspaceAssignment.Id}"
                        };

                        // Check if assignment already exists
                        var existingAssignments = storageService.LoadAssignments();
                        if (!existingAssignments.Any(a => a.Id == assignment.Id))
                        {
                            storageService.SaveAssignment(assignment);
                            syncedAssignments.Add(assignment);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error syncing assignments: {ex.Message}");
            }

            return syncedAssignments;
        }
    }

    /// <summary>
    /// Represents a Brightspace course enrollment.
    /// </summary>
    public class BrightspaceCourse
    {
        [System.Text.Json.Serialization.JsonPropertyName("courseId")]
        public string CourseId { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("courseName")]
        public string CourseName { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("courseCode")]
        public string CourseCode { get; set; }
    }

    /// <summary>
    /// Represents a Brightspace assignment (dropbox folder).
    /// </summary>
    public class BrightspaceAssignment
    {
        [System.Text.Json.Serialization.JsonPropertyName("id")]
        public int Id { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("name")]
        public string Name { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("description")]
        public string Description { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("dueDate")]
        public DateTime? DueDate { get; set; }
    }

    /// <summary>
    /// Response wrapper for enrollment list.
    /// </summary>
    public class BrightspaceEnrollmentResponse
    {
        [System.Text.Json.Serialization.JsonPropertyName("items")]
        public List<BrightspaceCourse> Items { get; set; }
    }

    /// <summary>
    /// Response wrapper for assignment list.
    /// </summary>
    public class BrightspaceAssignmentResponse
    {
        [System.Text.Json.Serialization.JsonPropertyName("objects")]
        public List<BrightspaceAssignment> Objects { get; set; }
    }
}

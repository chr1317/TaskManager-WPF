using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using TaskManagerWpfApp.Dto;

namespace TaskManagerWpfApp.Services
{
    public class ApiClient
    {
        private readonly HttpClient _http = new()
        {
            BaseAddress = new Uri("http://localhost:5272/api/")
        };

        private readonly JsonSerializerOptions _jsonOptions = new()
        {
            PropertyNameCaseInsensitive = true,
            Converters = { new JsonStringEnumConverter() }
        };

        public async Task<List<TasksDto>> GetTasksByUserAsync(int userId)
        {
            var response = await _http.GetAsync($"Task?uid={userId}");
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<TasksDto>>(json, _jsonOptions) ?? new();
        }

        public async Task<TasksDto?> GetTaskByIdAsync(int id)
        {
            return await _http.GetFromJsonAsync<TasksDto>($"Task/{id}");
        }

        public async Task<TasksDto?> CreateTaskAsync(TasksDto task)
        {
            var response = await _http.PostAsJsonAsync("Task", task);
            
            if(response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<TasksDto>();
            }
            return null;
        }

        public async Task<bool> UpdateTaskAsync(int taskId, TasksDto task)
        {
            var response = await _http.PutAsJsonAsync($"Task/{taskId}", task);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteTaskAsync(int id)
        {
            var response = await _http.DeleteAsync($"Task/{id}");
            return response.IsSuccessStatusCode;
        }
        public async Task<List<CategoryDto>> GetCategoriesAsync()
        {
            return await _http.GetFromJsonAsync<List<CategoryDto>>("Categories") ?? new List<CategoryDto>();
        }
    }
}


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

        public async Task<List<TasksDto>> GetTasksByUserAsync(int userId)
        {
            return await _http.GetFromJsonAsync<List<TasksDto>>($"Task?uid={userId}");
        }

        public async Task<TasksDto?> GetTaskByIdAsync(int id)
        {
            return await _http.GetFromJsonAsync<TasksDto>($"Task/{id}");
        }

        public async Task<bool> CreateTaskAsync(TasksDto task)
        {
            var response = await _http.PostAsJsonAsync("Task", task);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateTaskAsync(TasksDto task)
        {
            var response = await _http.PutAsJsonAsync($"Task/{task.TID}", task);
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


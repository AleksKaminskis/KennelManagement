using Client.Models;
using System.Net.Http.Json;

namespace Client.Services
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;

        public ApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // Customer methods
        public async Task<List<CustomerDto>> GetCustomersAsync()
        {
            return await _httpClient.GetFromJsonAsync<List<CustomerDto>>("api/customers") ?? new();
        }

        public async Task<CustomerDto?> GetCustomerAsync(int id)
        {
            return await _httpClient.GetFromJsonAsync<CustomerDto>($"api/customers/{id}");
        }

        public async Task<CustomerDto?> CreateCustomerAsync(CustomerFormModel model)
        {
            var response = await _httpClient.PostAsJsonAsync("api/customers", model);
            return response.IsSuccessStatusCode ? await response.Content.ReadFromJsonAsync<CustomerDto>() : null;
        }

        public async Task<bool> UpdateCustomerAsync(int id, CustomerFormModel model)
        {
            var response = await _httpClient.PutAsJsonAsync($"api/customers/{id}", model);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteCustomerAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"api/customers/{id}");
            return response.IsSuccessStatusCode;
        }

        // Dog methods
        public async Task<List<DogDto>> GetDogsAsync()
        {
            return await _httpClient.GetFromJsonAsync<List<DogDto>>("api/dogs") ?? new();
        }

        public async Task<DogDto?> GetDogAsync(int id)
        {
            return await _httpClient.GetFromJsonAsync<DogDto>($"api/dogs/{id}");
        }

        public async Task<DogDto?> CreateDogAsync(DogFormModel model)
        {
            var response = await _httpClient.PostAsJsonAsync("api/dogs", model);
            return response.IsSuccessStatusCode ? await response.Content.ReadFromJsonAsync<DogDto>() : null;
        }

        public async Task<bool> UpdateDogAsync(int id, DogFormModel model)
        {
            var response = await _httpClient.PutAsJsonAsync($"api/dogs/{id}", model);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteDogAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"api/dogs/{id}");
            return response.IsSuccessStatusCode;
        }

        // Kennel methods
        public async Task<List<KennelDto>> GetKennelsAsync()
        {
            return await _httpClient.GetFromJsonAsync<List<KennelDto>>("api/kennels") ?? new();
        }

        public async Task<List<KennelDto>> GetAvailableKennelsAsync()
        {
            return await _httpClient.GetFromJsonAsync<List<KennelDto>>("api/kennels/available") ?? new();
        }

        public async Task<KennelDto?> GetKennelAsync(int id)
        {
            return await _httpClient.GetFromJsonAsync<KennelDto>($"api/kennels/{id}");
        }

        public async Task<KennelDto?> CreateKennelAsync(KennelFormModel model)
        {
            var response = await _httpClient.PostAsJsonAsync("api/kennels", model);
            return response.IsSuccessStatusCode ? await response.Content.ReadFromJsonAsync<KennelDto>() : null;
        }

        public async Task<bool> UpdateKennelAsync(int id, KennelFormModel model)
        {
            var response = await _httpClient.PutAsJsonAsync($"api/kennels/{id}", model);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteKennelAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"api/kennels/{id}");
            return response.IsSuccessStatusCode;
        }

        // Booking methods
        public async Task<List<BookingDto>> GetBookingsAsync()
        {
            return await _httpClient.GetFromJsonAsync<List<BookingDto>>("api/bookings") ?? new();
        }

        public async Task<BookingDto?> GetBookingAsync(int id)
        {
            return await _httpClient.GetFromJsonAsync<BookingDto>($"api/bookings/{id}");
        }

        public async Task<BookingDto?> CreateBookingAsync(BookingFormModel model)
        {
            var response = await _httpClient.PostAsJsonAsync("api/bookings", model);
            return response.IsSuccessStatusCode ? await response.Content.ReadFromJsonAsync<BookingDto>() : null;
        }

        public async Task<bool> UpdateBookingAsync(int id, BookingFormModel model)
        {
            var updateDto = new
            {
                model.DogId,
                model.KennelId,
                model.CheckInDate,
                model.CheckOutDate,
                model.SpecialRequirements,
                model.Status,
                model.TotalCost
            };

            var response = await _httpClient.PutAsJsonAsync($"api/bookings/{id}", updateDto);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteBookingAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"api/bookings/{id}");
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateBookingStatusAsync(int id, string status)
        {
            var response = await _httpClient.PatchAsJsonAsync($"api/bookings/{id}/status", status);
            return response.IsSuccessStatusCode;
        }
    }
}

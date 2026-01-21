using Client.Models;
using System.Text.Json.Serialization;

namespace Client
{
    // Add ALL DTOs used by the client here
    [JsonSerializable(typeof(RegisterModel))]
    [JsonSerializable(typeof(LoginModel))]
    [JsonSerializable(typeof(AuthResponse))]
    [JsonSerializable(typeof(BookingDto))]
    [JsonSerializable(typeof(BookingFormModel))]
    [JsonSerializable(typeof(CustomerDto))]
    [JsonSerializable(typeof(CustomerFormModel))]
    [JsonSerializable(typeof(DogDto))]
    [JsonSerializable(typeof(DogFormModel))]
    [JsonSerializable(typeof(KennelDto))]
    [JsonSerializable(typeof(KennelFormModel))]
    
    public partial class ClientJsonContext : JsonSerializerContext
    {
    }

}

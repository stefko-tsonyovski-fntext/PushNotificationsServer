using System.Text.Json.Serialization;

namespace PWAPushNotificationsServer
{
    public class PayloadDto
    {
        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName ("message")]
        public string Message { get; set; }
    }
}

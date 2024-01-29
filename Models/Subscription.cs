using System.ComponentModel.DataAnnotations;

namespace PWAPushNotificationsServer.Models
{
    public class Subscription
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; }

        [Required]
        public string Endpoint { get; set; }

        [Required]
        public string P256dh { get; set; }

        [Required]
        public string Auth { get; set; }
    }
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PWAPushNotificationsServer.Data;
using PWAPushNotificationsServer.Models;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json;
using WebPush;

namespace PWAPushNotificationsServer.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class NotificationsController : ControllerBase
    {
        private readonly string publicVapidKey = "BDZJSiMXSJUhryPkjFh_H84ZeEjVNfq5STCXVDEW4bpXye1mybGCjufRFIVmMxJN1wHOGUunGyBra0qvSa0fGJ8";
        private readonly string privateVapidKey = "upQsMoPu4_T6aT3a8Nwg8b7Cd3wNjQwfD5PgCYJjTmc";
        private readonly ApplicationDbContext dbContext;

        public NotificationsController(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        [HttpGet("vapidPublicKey")]
        public ActionResult GetVapidPublicKey()
        {
            return this.Ok(this.publicVapidKey); // Replace 'yourVapidPublicKey' with your actual VAPID public key
        }

        [HttpGet]
        public async Task<ActionResult<List<Subscription>>> List()
        {
            List<Subscription> subscriptions = await this.dbContext.Subscriptions
                .ToListAsync();

            return this.Ok(subscriptions);
        }

        [HttpPost("subscribe")]
        public async Task<ActionResult<Subscription>> Subscribe([FromBody] SubscribeDto subscribeDto)
        {
            Subscription subscription = new Subscription
            {
                UserId = subscribeDto.UserId,
                Endpoint = subscribeDto.Endpoint,
                P256dh = subscribeDto.P256dh,
                Auth = subscribeDto.Auth,
            };

            var result = this.dbContext.Subscriptions.Add(subscription);
            await this.dbContext.SaveChangesAsync();

            return this.Ok(result.Entity);
        }

        [HttpPost("send")]
        public async Task<ActionResult> SendNotification([FromBody] NotificationDto notificationDto)
        {
            List<Subscription> subscriptions = await this.dbContext.Subscriptions
                .Where(s => s.UserId == notificationDto.UserId)
                .ToListAsync();

            foreach (var subscription in subscriptions)
            {
                var subject = @"mailto:example@example.com";

                var subscriptionToBePushed = new PushSubscription(subscription.Endpoint, subscription.P256dh, subscription.Auth);
                var vapidDetails = new VapidDetails(subject, this.publicVapidKey, this.privateVapidKey);

                // var gcmAPIKey = @"[your key here]";

                var webPushClient = new WebPushClient();

                PayloadDto payloadDto = new PayloadDto
                {
                    Title = notificationDto.Title,
                    Message = notificationDto.Message,
                };

                string payload = JsonSerializer.Serialize(payloadDto);

                try
                {
                    await webPushClient.SendNotificationAsync(subscriptionToBePushed, payload, vapidDetails);
                    // await webPushClient.SendNotificationAsync(subscription, "payload", gcmAPIKey);
                }
                catch (WebPushException exception)
                {
                    return this.BadRequest(exception.Message);
                }
            }

            return this.Ok();
        }
    }
}

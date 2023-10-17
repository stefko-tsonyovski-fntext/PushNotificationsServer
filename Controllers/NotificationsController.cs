using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography.X509Certificates;
using WebPush;

namespace PWAPushNotificationsServer.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class NotificationsController : ControllerBase
    {
        private readonly string publicVapidKey = VapidHelper.GenerateVapidKeys().PublicKey;
        private readonly string privateVapidKey = VapidHelper.GenerateVapidKeys().PrivateKey;

        [HttpGet("vapidPublicKey")]
        public ActionResult GetVapidPublicKey()
        {
            return this.Ok(this.publicVapidKey); // Replace 'yourVapidPublicKey' with your actual VAPID public key
        }

        [HttpPost("send-notification")]
        public async Task<ActionResult> SendNotification([FromBody] NotificationDto notificationDto)
        {
            var subject = @"mailto:stefko.noisy.boy@gmail.com";

            var subscription = new PushSubscription(notificationDto.Endpoint, notificationDto.P256dh, notificationDto.Auth);
            var vapidDetails = new VapidDetails(subject, this.publicVapidKey, this.privateVapidKey);
            //var gcmAPIKey = @"[your key here]";

            var webPushClient = new WebPushClient();

            try
            {
                await webPushClient.SendNotificationAsync(subscription, "payload", vapidDetails);
                return this.Ok(subscription);
                //await webPushClient.SendNotificationAsync(subscription, "payload", gcmAPIKey);
            }
            catch (WebPushException exception)
            {
                return this.BadRequest(exception);
            }
        }
    }
}
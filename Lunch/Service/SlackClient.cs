using System;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.Helpers;
using Lunch.Entity;
using Lunch.Helper;
using Lunch.Models;

namespace Lunch.Service
{
    public class SlackClient : INotificationService
    {
        public bool SendAlert(IAlertBody alertBody, SlackChannel slackChannel)
        {
            var client = new WebClient();
            client.Headers.Add(HttpRequestHeader.ContentType, "application/json");
            client.Encoding = Encoding.UTF8;
            //var proxy = ConfigHelper.Instance.GetSingleConfig("proxy");
            //if (!string.IsNullOrEmpty(proxy))
            //{
            //    client.Proxy = new WebProxy(proxy);
            //}
            var jsonPayload = Json.Encode(alertBody);
            var dataBytes = Encoding.UTF8.GetBytes(jsonPayload);
            byte[] responseBytes;
            try
            {
                responseBytes = client.UploadData(GetSlackWebhook(slackChannel), dataBytes);
            }
            catch (Exception ex)
            {
                throw;
            }

            var response = Encoding.UTF8.GetString(responseBytes);
            return response.ToLower() == "ok";
        }

        private string GetSlackWebhook(SlackChannel slackChannel)
        {
            switch (slackChannel)
            {
                case SlackChannel.TransferMessage:
                    return ConfigHelper.Instance.GetSingleConfig("slack/TransferMessageWebhookUrl");
                default:
                    return ConfigHelper.Instance.GetSingleConfig("slack/TransferMessageWebhookUrl");
            }
        }
    }
}
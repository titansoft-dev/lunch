using Lunch.Entity;
using Lunch.Models;

namespace Lunch.Service
{
    public interface INotificationService
    {
        bool SendAlert(IAlertBody alertBody, SlackChannel slackChannel);
    }
}
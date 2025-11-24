using static SinkDNS.Modules.Enums;

namespace SinkDNS.Modules
{
    internal class NotificationManager
    {
        // This will handle notifications to the user via the Notification Area (System Tray).
        public static void ShowNotification(string title, string message, NotificationType messageType, int duration = 5000)
        {
            ShowSystemTrayNotification(title, message, messageType, duration);
        }

        private static void ShowSystemTrayNotification(string title, string message, NotificationType messageType, int duration = 5000)
        {
            try
            {
                var notifyIcon = new NotifyIcon
                {
                    Icon = GetNotificationIcon(messageType),
                    Visible = true,
                    Text = title
                };

                notifyIcon.ShowBalloonTip(
                    duration,
                    title,
                    message,
                    GetBalloonTipIcon(messageType)
                );

                Task.Delay(duration).ContinueWith(_ =>
                {
                    try
                    {
                        notifyIcon.Visible = false;
                        notifyIcon.Dispose();
                    }
                    catch
                    {
                    }
                });
            }
            catch (Exception ex)
            {
                // If all else fails, just write to console
                Console.WriteLine($"Notification failed: {title} - {message}, {ex.Message}");
            }
        }

        private static Icon GetNotificationIcon(NotificationType messageType)
        {
            try
            {
                switch (messageType)
                {
                    case NotificationType.Error:
                        return SystemIcons.Error;
                    case NotificationType.Warning:
                        return SystemIcons.Warning;
                    case NotificationType.Information:
                        return SystemIcons.Information;
                    default:
                        return SystemIcons.Information;
                }
            }
            catch
            {
                return SystemIcons.Information;
            }
        }

        private static ToolTipIcon GetBalloonTipIcon(NotificationType messageType)
        {
            switch (messageType)
            {
                case NotificationType.Error:
                    return ToolTipIcon.Error;
                case NotificationType.Warning:
                    return ToolTipIcon.Warning;
                case NotificationType.Information:
                    return ToolTipIcon.Info;
                default:
                    return ToolTipIcon.Info;
            }
        }
    }
}
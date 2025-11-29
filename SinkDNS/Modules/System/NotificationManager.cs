//MIT License

//Copyright (c) 2025 Dimon

//Permission is hereby granted, free of charge, to any person obtaining a copy
//of this software and associated documentation files (the "Software"), to deal
//in the Software without restriction, including without limitation the rights
//to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//copies of the Software, and to permit persons to whom the Software is
//furnished to do so, subject to the following conditions:

//The above copyright notice and this permission notice shall be included in all
//copies or substantial portions of the Software.

//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//SOFTWARE.

using SinkDNS.Modules.SinkDNSInternals;
using static SinkDNS.Modules.Enums;

namespace SinkDNS.Modules.System
{
    internal class NotificationManager
    {
        // This will handle notifications to the user via the Notification Area (System Tray).
        public static void ShowNotification(string title, string message, StatusSeverityType messageType, int duration = 5000)
        {
            ShowSystemTrayNotification(title, message, messageType, duration);
        }

        private static void ShowSystemTrayNotification(string title, string message, StatusSeverityType messageType, int duration = 5000)
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
                TraceLogger.Log($"Failed to show system tray notification: {ex.Message}", StatusSeverityType.Error);
            }
        }

        private static Icon GetNotificationIcon(StatusSeverityType messageType)
        {
            try
            {
                return messageType switch
                {
                    StatusSeverityType.Error => SystemIcons.Error,
                    StatusSeverityType.Warning => SystemIcons.Warning,
                    StatusSeverityType.Information => SystemIcons.Information,
                    _ => SystemIcons.Information,
                };
            }
            catch
            {
                return SystemIcons.Information;
            }
        }

        private static ToolTipIcon GetBalloonTipIcon(StatusSeverityType messageType)
        {
            return messageType switch
            {
                StatusSeverityType.Error => ToolTipIcon.Error,
                StatusSeverityType.Warning => ToolTipIcon.Warning,
                StatusSeverityType.Information => ToolTipIcon.Info,
                _ => ToolTipIcon.Info,
            };
        }
    }
}

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

using SinkDNS.Properties;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using static SinkDNS.Modules.Enums;

namespace SinkDNS.Modules.SinkDNSInternals
{
    public static class TraceLogger
    {
        private static readonly Lock _lock = new();
        private static readonly string _logDirectory = Settings.Default.LogsFolder;
        private static string _currentDate = DateTime.Now.ToString("dd-MM-yyyy");
        private static DateTime _lastDateCheck = DateTime.MinValue;

        public static void PurgeAllLogs()
        {
            foreach (string file in Directory.GetFiles(_logDirectory))
            {
                Console.WriteLine($"Deleting all logs. Currently deleting: {file}");
                File.Delete(file);
            }
        }
        public static void ClearExpiredLogs()
        {
            lock (_lock)
            {
                _lastDateCheck = DateTime.MinValue;
                try
                {
                    if (!Directory.Exists(_logDirectory))
                        return;
                    var logFiles = Directory.GetFiles(_logDirectory, "*.log");
                    var expiryDate = DateTime.Now.AddDays(-Settings.Default.LogExpiryInDays);
                    foreach (var logFile in logFiles)
                    {
                        var fileInfo = new FileInfo(logFile);
                        if (fileInfo.CreationTime < expiryDate)
                        {
                            fileInfo.Delete();
                            Log($"Deleted expired log file: {fileInfo.Name}");
                            Debug.WriteLine($"Deleted expired log file: {fileInfo.Name}");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log($"Failed to clear expired logs: {ex.Message}", StatusSeverityType.Error);
                    Debug.WriteLine($"Failed to clear expired logs: {ex.Message}");
                }
            }
        }

        public static void Log(string message, StatusSeverityType severity = StatusSeverityType.Information,
                              [CallerMemberName] string memberName = "",
                              [CallerFilePath] string filePath = "",
                              [CallerLineNumber] int lineNumber = 0)
        {
            string className = ExtractClassName(filePath);
            if (string.IsNullOrEmpty(message))
            {
                Log($"The function {memberName} in {className} class has called the TraceLogger.Log at line {lineNumber} but hasn't defined any of the log variables! That class may be malfunctioning.", StatusSeverityType.Warning);
            }
            string logEntry = string.Empty;
            string filePathLog = Path.Combine(_logDirectory, $"{_currentDate}.log");
            try
            {
                var now = DateTime.Now;
                var currentDate = now.ToString("dd-MM-yyyy");
                if (now.Subtract(_lastDateCheck).TotalSeconds > 10)
                {
                    _currentDate = currentDate;
                    _lastDateCheck = now;
                }
                string timestamp = now.ToString("yyyy-MM-dd HH:mm:ss.fff");
                string severityText = severity.ToString().ToUpper();
                logEntry = $"[{timestamp}] [{severityText}] [{className}] [{memberName}] [Line: {lineNumber}]: {message}";
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to prepare log entry: {ex.Message}");
            }
            Debug.WriteLine(logEntry);
            lock (_lock)
            {
                try
                {
                    if (Settings.Default.EnableDiskLogging)
                    {
                        File.AppendAllText(filePathLog, $"{logEntry}{Environment.NewLine}", Encoding.UTF8);
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Failed to write to log: {ex.Message}");
                }
            }
        }

        public static void LogAndThrowMsgBox(string message, StatusSeverityType severity = StatusSeverityType.Information, string title = "",
                              [CallerMemberName] string memberName = "",
                              [CallerFilePath] string filePath = "",
                              [CallerLineNumber] int lineNumber = 0)
        {
            Log(message, severity);
            string className = ExtractClassName(filePath);
            //Depending on the severity, show the right msgbox
            if (!string.IsNullOrEmpty(message))
            {
                switch (severity)
                {
                    case StatusSeverityType.Information:
                        MessageBox.Show($"SinkDNS Information Trace Details: {className} {memberName} {lineNumber} : {message}", "SinkDNS Trace Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        break;
                    case StatusSeverityType.Warning:
                        MessageBox.Show($"Warning, SinkDNS is reporting a possible problem. The {memberName} has reported a warning at line {lineNumber}: {message}", "SinkDNS Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        break;
                    case StatusSeverityType.Error:
                        MessageBox.Show($"Error! SinkDNS is having trouble. The {memberName} class has reported a problem at line {lineNumber}: {message}", "SinkDNS Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;
                    case StatusSeverityType.Debug:
                        MessageBox.Show($"SinkDNS DEBUG Trace Details: {className} {memberName} {lineNumber} : {message}", "SinkDNS DEBUG Trace Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        break;
                }
            }
        }

        private static string ExtractClassName(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                return "UnknownClass";

            try
            {
                string fileName = Path.GetFileName(filePath);

                int lastDot = fileName.LastIndexOf('.');
                if (lastDot > 0)
                    fileName = fileName[..lastDot];

                int lastDotInPath = filePath.LastIndexOf(Path.DirectorySeparatorChar);
                if (lastDotInPath > 0)
                {
                    string directoryPath = filePath[..lastDotInPath];
                    int lastDirectorySeparator = directoryPath.LastIndexOf(Path.DirectorySeparatorChar);
                    if (lastDirectorySeparator > 0)
                    {
                        string className = fileName;
                        return className;
                    }
                }

                return fileName;
            }
            catch
            {
                return "UnknownClass";
            }
        }
    }
}
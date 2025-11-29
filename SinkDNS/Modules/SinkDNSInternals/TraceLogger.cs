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
using System.Text;
using static SinkDNS.Modules.Enums;

namespace SinkDNS.Modules.SinkDNSInternals
{
    public static class TraceLogger
    {
        private static readonly Lock _lock = new();

        private static readonly string _logDirectory = Settings.Default.LogsFolder;

        public static void Log(string message, StatusSeverityType severity = StatusSeverityType.Information)
        {
            lock (_lock)
            {
                try
                {
                    string date = DateTime.Now.ToString("dd-MM-yyyy");
                    string filePath = Path.Combine(_logDirectory, $"{date}.log");
                    string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
                    string severityText = severity.ToString().ToUpper();

                    // Get caller info (class + line number)
                    var callerInfo = GetCallerInfo();

                    string logEntry = $"[{timestamp}] [{severityText}] [{callerInfo.ClassName}] [Line: {callerInfo.LineNumber}]: {message}{Environment.NewLine}";
                    File.AppendAllText(filePath, logEntry, Encoding.UTF8);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to write to log: {ex.Message}");
                }
            }
        }

        private static (string ClassName, int LineNumber) GetCallerInfo()
        {
            try
            {
                StackTrace stackTrace = new(true); // 'true' enables file info (line numbers)
                for (int i = 1; i < stackTrace.FrameCount; i++)
                {
                    StackFrame? frame = stackTrace.GetFrame(i);
                    if (frame == null) continue;

                    var method = frame.GetMethod();
                    if (method == null) continue;

                    var declaringType = method.DeclaringType;
                    if (declaringType == null) continue;

                    // Skip frames from TraceLogger itself
                    if (declaringType != typeof(TraceLogger))
                    {
                        return (declaringType.Name, frame.GetFileLineNumber());
                    }
                }
            }
            catch
            {
                return ("UnknownClass", -1);
            }
            return ("NullClass", -1);
        }
    }
}

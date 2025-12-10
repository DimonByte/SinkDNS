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

using SinkDNS.Modules;
using SinkDNS.Modules.SinkDNSInternals;
using System.Runtime.InteropServices;

public class DNSCryptQueryMonitor
{
    private string logFilePath;
    private FileSystemWatcher fileWatcher;
    public bool isMonitoring = false;
    private HashSet<string> processedLines;
    private CancellationTokenSource _cancellationTokenSource;
    private Task _pollingTask;
    private DateTime _lastProcessedTime = DateTime.MinValue;
    private readonly TimeSpan _minimumProcessingInterval = TimeSpan.FromMilliseconds(100);

    public event Action<string> OnDomainBlocked;

    public DNSCryptQueryMonitor(string logFilePath)
    {
        this.logFilePath = logFilePath;
        this.processedLines = new HashSet<string>();
    }

    public async Task StartMonitoringAsync()
    {
        try
        {
            if (!File.Exists(logFilePath))
            {
                TraceLogger.Log($"DNS log file not found, unable to start monitoring: {logFilePath}", Enums.StatusSeverityType.Error);
                return;
            }

            // Read existing content
            await ReadExistingLog();

            // Set up file watcher for real-time monitoring
            SetupFileWatcher();

            // Start polling as a backup mechanism
            _cancellationTokenSource = new CancellationTokenSource();
            _pollingTask = Task.Run(async () => await BackgroundPollingAsync(_cancellationTokenSource.Token));

            isMonitoring = true;
            TraceLogger.Log($"Monitoring DNS log file: {logFilePath}", Enums.StatusSeverityType.Information);
        }
        catch (Exception ex)
        {
            TraceLogger.Log($"Error starting monitoring: {ex.Message}", Enums.StatusSeverityType.Error);
        }
    }
    private bool IsFileLocked(IOException ex)
    {
        int errorCode = Marshal.GetHRForException(ex) & 0xFFFF;
        return errorCode == 32 || errorCode == 33; // ERROR_SHARING_VIOLATION or ERROR_LOCK_VIOLATION
    }

    private async Task ReadExistingLog()
    {
        const int maxRetries = 5;
        const int delayMs = 100;

        for (int attempt = 0; attempt < maxRetries; attempt++)
        {
            try
            {
                using (var stream = new FileStream(logFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                using (var reader = new StreamReader(stream))
                {
                    string? line;
                    while ((line = await reader.ReadLineAsync()) != null)
                    {
                        ProcessLogLine(line);
                    }
                }
                return;
            }
            catch (IOException ex) when (IsFileLocked(ex))
            {
                TraceLogger.Log("IOException File Lock or File Sharing Violation.", Enums.StatusSeverityType.Error);
                TraceLogger.Log($"File is locked, retrying in {delayMs}ms... (Attempt {attempt + 1}/{maxRetries})", Enums.StatusSeverityType.Warning);
                if (attempt < maxRetries - 1)
                {
                    await Task.Delay(delayMs);
                }
                else
                {
                    TraceLogger.Log($"Failed to read log file after {maxRetries} attempts: {ex.Message}", Enums.StatusSeverityType.Error);
                    throw;
                }
            }
            catch (Exception ex)
            {
                TraceLogger.Log($"Error reading existing log: {ex.Message}", Enums.StatusSeverityType.Error);
                throw;
            }
        }
    }
    private void SetupFileWatcher()
    {
        fileWatcher = new FileSystemWatcher();
        fileWatcher.Path = Path.GetDirectoryName(logFilePath);
        fileWatcher.Filter = Path.GetFileName(logFilePath);

        fileWatcher.NotifyFilter = NotifyFilters.LastWrite |
                                  NotifyFilters.Size |
                                  NotifyFilters.CreationTime |
                                  NotifyFilters.LastAccess;

        fileWatcher.Changed += OnLogChanged;
        fileWatcher.Created += OnLogChanged; // Also watch for new files
        fileWatcher.EnableRaisingEvents = true;

        TraceLogger.Log($"File watcher configured for: {logFilePath}", Enums.StatusSeverityType.Information);
    }

    private void OnLogChanged(object sender, FileSystemEventArgs e)
    {
        //TraceLogger.Log($"File change detected: {e.ChangeType} for {e.FullPath} at {DateTime.Now}", Enums.StatusSeverityType.Information);
        if (e.ChangeType == WatcherChangeTypes.Changed || e.ChangeType == WatcherChangeTypes.Created)
        {
            // Debounce the processing to avoid too many rapid calls
            if (DateTime.Now - _lastProcessedTime > _minimumProcessingInterval)
            {
                Task.Run(async () =>
                {
                    await Task.Delay(50); // Small delay to ensure file write is complete
                    await ProcessNewLogLines();
                    _lastProcessedTime = DateTime.Now;
                });
            }
        }
    }

    private async Task BackgroundPollingAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                await Task.Delay(2000, cancellationToken); // Poll every 2 seconds
                await ProcessNewLogLines();
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                TraceLogger.Log($"Background polling error: {ex.Message}", Enums.StatusSeverityType.Error);
            }
        }
    }
    private void ProcessLogLine(string line)
    {
        try
        {
            // [2025-12-10 21:32:06] ::1 bing.com A REJECT 0ms -
            if (string.IsNullOrWhiteSpace(line))
                return;

            // Split by tab characters
            var parts = line.Split('\t');
            if (parts.Length >= 5)
            {
                // Check if this is a REJECT entry
                var status = parts[4].Trim();
                if (status == "REJECT")
                {
                    // The domain is in the 3rd column (index 2)
                    var domain = parts[2].Trim();
                    if (!string.IsNullOrEmpty(domain) && domain != "-")
                    {
                        // Remove any trailing dots or special characters
                        domain = domain.TrimEnd('.');
                        TraceLogger.Log($"BLOCKED: {domain} at {DateTime.Now:yyyy-MM-dd HH:mm:ss}", Enums.StatusSeverityType.Information);
                        OnDomainBlocked?.Invoke(domain);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            TraceLogger.Log($"Error processing log line: {ex.Message}", Enums.StatusSeverityType.Error);
        }
    }
    private async Task ProcessNewLogLines()
    {
        const int maxRetries = 3;
        const int delayMs = 50;

        for (int attempt = 0; attempt < maxRetries; attempt++)
        {
            try
            {
                using (var stream = new FileStream(logFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                using (var reader = new StreamReader(stream))
                {
                    string? line;
                    while ((line = await reader.ReadLineAsync()) != null)
                    {
                        if (!processedLines.Contains(line))
                        {
                            ProcessLogLine(line);
                            processedLines.Add(line);
                        }
                    }
                }
                return;
            }
            catch (IOException ex) when (IsFileLocked(ex))
            {
                TraceLogger.Log($"File is locked during processing, retrying... (Attempt {attempt + 1}/{maxRetries})", Enums.StatusSeverityType.Warning);
                if (attempt < maxRetries - 1)
                {
                    await Task.Delay(delayMs);
                }
                else
                {
                    TraceLogger.Log($"Failed to process new log lines after {maxRetries} attempts: {ex.Message}", Enums.StatusSeverityType.Error);
                }
            }
            catch (Exception ex)
            {
                TraceLogger.Log($"Error processing new log lines: {ex.Message}", Enums.StatusSeverityType.Error);
                break;
            }
        }
    }
}
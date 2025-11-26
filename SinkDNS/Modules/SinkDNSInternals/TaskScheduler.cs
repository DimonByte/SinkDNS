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

using static SinkDNS.Modules.Enums;

namespace SinkDNS.Modules.SinkDNSInternals
{
    public class TaskScheduler
    {
        private static readonly string ConfigPath = Path.Combine("config", "taskschedules.ini");
        private static readonly Lock LockObject = new();

        public enum TaskType
        {
            RunAtApplicationStart,
            RunAtSpecificTime,
            RunAtInterval
        }

        public enum TaskAction
        {
            UpdateBlocklists,
            CheckDNSCryptUpdates,
            UpdateDNSCryptResolvers,
            UpdateAll
        }

        public class ScheduledTask
        {
            public TaskType Type { get; set; }
            public TaskAction Action { get; set; }
            public required string Name { get; set; }
            public DateTime? ScheduledTime { get; set; }
            public TimeSpan? Interval { get; set; }
            public bool IsEnabled { get; set; } = true;
        }

        private static List<ScheduledTask> _tasks = [];

        static TaskScheduler()
        {
            LoadTasks();
        }

        public static void CreateTask(TaskType type, TaskAction action, string name, DateTime? scheduledTime = null, TimeSpan? interval = null)
        {
            TraceLogger.Log($"Creating scheduled task: {name}", StatusSeverityType.Information);
            lock (LockObject)
            {
                var task = new ScheduledTask
                {
                    Type = type,
                    Action = action,
                    Name = name,
                    ScheduledTime = scheduledTime,
                    Interval = interval
                };

                _tasks.Add(task);
                SaveTasks();
            }
        }

        public static void RemoveTask(string name)
        {
            TraceLogger.Log($"Removing scheduled task: {name}", StatusSeverityType.Information);
            lock (LockObject)
            {
                _tasks.RemoveAll(t => t.Name == name);
                SaveTasks();
            }
        }

        public static void ExecutePendingTasks()
        {
            TraceLogger.Log("Checking for pending scheduled tasks to execute.", StatusSeverityType.Information);
            lock (LockObject)
            {
                var now = DateTime.Now;
                var pendingTasks = _tasks.Where(t => t.IsEnabled &&
                    (t.Type == TaskType.RunAtApplicationStart ||
                     (t.ScheduledTime.HasValue && t.ScheduledTime.Value <= now) ||
                     (t.Type == TaskType.RunAtInterval && t.Interval.HasValue &&
                      t.ScheduledTime.HasValue &&
                      t.ScheduledTime.Value.Add(t.Interval.Value) <= now)))
                    .ToList();

                foreach (var task in pendingTasks)
                {
                    TraceLogger.Log($"Executing scheduled task: {task.Name}", StatusSeverityType.Information);
                    ExecuteTask(task);
                    if (task.Type == TaskType.RunAtInterval)
                    {
                        task.ScheduledTime = now;
                    }
                }
                SaveTasks();
            }
        }

        private static void ExecuteTask(ScheduledTask task)
        {
            try
            {
                switch (task.Action)
                {
                    case TaskAction.UpdateBlocklists:
                        //BlockListManager.UpdateList();
                        break;
                    case TaskAction.CheckDNSCryptUpdates:
                        //DNSCryptManager.CheckForUpdates();
                        break;
                    case TaskAction.UpdateDNSCryptResolvers:
                        //DNSCryptManager.UpdateResolvers();
                        break;
                    case TaskAction.UpdateAll:
                        //BlocklistManager.UpdateList();
                        //DNSCryptManager.CheckForUpdates();
                        //DNSCryptManager.UpdateResolvers();
                        break;
                }
            }
            catch (Exception ex)
            {
                TraceLogger.Log($"Error executing task {task.Name}: {ex.Message}", StatusSeverityType.Error);
            }
        }

        private static void SaveTasks()
        {
            try
            {
                TraceLogger.Log("Saving scheduled tasks to configuration.", StatusSeverityType.Information);
                Directory.CreateDirectory(Path.GetDirectoryName(ConfigPath));

                var lines = new List<string>
                {
                    "[ScheduledTasks]"
                };

                foreach (var task in _tasks)
                {
                    TraceLogger.Log($"Saving task: {task.Name}", StatusSeverityType.Information);
                    lines.Add($"TaskName={task.Name}");
                    lines.Add($"TaskType={(int)task.Type}");
                    lines.Add($"TaskAction={(int)task.Action}");
                    lines.Add($"ScheduledTime={task.ScheduledTime?.ToString("o") ?? ""}");
                    lines.Add($"Interval={task.Interval?.TotalMinutes ?? 0}");
                    lines.Add($"IsEnabled={task.IsEnabled}");
                    lines.Add("");
                }

                File.WriteAllLines(ConfigPath, lines);
                TraceLogger.Log("Scheduled tasks saved successfully.", StatusSeverityType.Information);
            }
            catch (Exception ex)
            {
                TraceLogger.Log($"Error saving tasks: {ex.Message}", StatusSeverityType.Error);
            }
        }

        private static void LoadTasks()
        {
            try
            {
                TraceLogger.Log("Loading scheduled tasks from configuration.", StatusSeverityType.Information);
                if (!File.Exists(ConfigPath))
                {
                    _tasks = [];
                    TraceLogger.Log("No scheduled tasks configuration file found.", StatusSeverityType.Information);
                    return;
                }

                var lines = File.ReadAllLines(ConfigPath);
                ScheduledTask? currentTask = null;
                var inTaskSection = false;

                foreach (var line in lines)
                {
                    if (line.StartsWith("[") && line.EndsWith("]"))
                    {
                        inTaskSection = line.Equals("[ScheduledTasks]", StringComparison.OrdinalIgnoreCase);
                        continue;
                    }

                    if (!inTaskSection) continue;

                    if (string.IsNullOrWhiteSpace(line)) continue;

                    var parts = line.Split(['='], 2);
                    if (parts.Length != 2) continue;

                    var key = parts[0].Trim();
                    var value = parts[1].Trim();

                    switch (key)
                    {
                        case "TaskName":
                            if (currentTask != null)
                            {
                                _tasks.Add(currentTask);
                            }
                            currentTask = new ScheduledTask { Name = value };
                            break;
                        case "TaskType":
                            if (currentTask != null && Enum.TryParse<TaskType>(value, out var taskType))
                                currentTask.Type = taskType;
                            break;
                        case "TaskAction":
                            if (currentTask != null && Enum.TryParse<TaskAction>(value, out var taskAction))
                                currentTask.Action = taskAction;
                            break;
                        case "ScheduledTime":
                            if (currentTask != null && DateTime.TryParse(value, out var scheduledTime))
                                currentTask.ScheduledTime = scheduledTime;
                            break;
                        case "Interval":
                            if (currentTask != null && double.TryParse(value, out var intervalMinutes))
                                currentTask.Interval = TimeSpan.FromMinutes(intervalMinutes);
                            break;
                        case "IsEnabled":
                            if (currentTask != null && bool.TryParse(value, out var isEnabled))
                                currentTask.IsEnabled = isEnabled;
                            break;
                    }
                }

                if (currentTask != null)
                {
                    TraceLogger.Log($"Loaded task: {currentTask.Name}", StatusSeverityType.Information);
                    _tasks.Add(currentTask);
                }
            }
            catch (Exception ex)
            {
                TraceLogger.Log($"Error loading tasks: {ex.Message}", StatusSeverityType.Error);
                _tasks = [];
            }
        }
    }

    // Example usage:
    // TaskScheduler.CreateTask(TaskScheduler.TaskType.RunAtApplicationStart, TaskScheduler.TaskAction.UpdateBlocklists, "UpdateBlocklists");
    // TaskScheduler.CreateTask(TaskScheduler.TaskType.RunAtInterval, TaskScheduler.TaskAction.CheckDNSCryptUpdates, "CheckDNSCryptUpdates", interval: TimeSpan.FromHours(1));
    // TaskScheduler.ExecutePendingTasks();
}
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

namespace SinkDNS.Modules.System
{
    using SinkDNS.Modules.SinkDNSInternals;
    using SinkDNS.Properties;

    //This will handle folder and file management for SinkDNS, like creating necessary directories.
    internal class IOManager
    {
        public static void CreateNecessaryDirectories()
        {
            string[] directories = [Settings.Default.LogsFolder, Settings.Default.ConfigFolder, Settings.Default.ResolversFolder, Settings.Default.HostFilesFolder, Settings.Default.BackupFolder, Settings.Default.BlocklistFolder, Settings.Default.WhitelistFolder, Settings.Default.TaskScheduleFolder];
            foreach (string dir in directories)
            {
                if (!Directory.Exists(dir))
                {
                    try
                    {
                        Directory.CreateDirectory(dir);
                        TraceLogger.Log($"Created directory: {dir}");
                    }
                    catch (Exception ex)
                    {
                        TraceLogger.Log($"Error creating directory {dir}: {ex.Message}", Enums.StatusSeverityType.Error);
                    }
                }
            }
        }

        public static void BackupFile(string filePath)
        {
            if (File.Exists(filePath))
            {
                string backupFilePath = $"{Settings.Default.BackupFolder}/{Path.GetFileName(filePath) + Path.GetExtension(filePath)}.bak";
                try
                {
                    TraceLogger.Log($"Creating backup for file {filePath} at {backupFilePath}");
                    File.Copy(filePath, backupFilePath, true);
                }
                catch (Exception ex)
                {
                    TraceLogger.Log($"Error creating backup for file {filePath}: {ex.Message}", Enums.StatusSeverityType.Error);
                }
            }
        }

        public static void AddToIniFile(string iniFilePath, string domain)
        {
            var directory = Path.GetDirectoryName(iniFilePath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                Directory.CreateDirectory(directory);
            File.AppendAllText(iniFilePath, $"{domain}{Environment.NewLine}");
        }

        public static void MergeFiles(string sourceFolder, string outputFile)
        {
            // Delete existing combined file, we don't want that mess to happen...
            TraceLogger.Log($"Creating combined file: {outputFile}");
            if (File.Exists(outputFile))
                File.Delete(outputFile);

            var files = Directory.GetFiles(sourceFolder, "*.txt");
            if (files.Length == 0)
            {
                TraceLogger.Log($"No files found to merge in {sourceFolder}", Enums.StatusSeverityType.Warning);
                return;
            }

            using var writer = new StreamWriter(outputFile);
            foreach (var file in files)
            {
                TraceLogger.Log($"Merging file: {file}");
                var lines = File.ReadAllLines(file);
                foreach (var line in lines)
                {
                    if (!string.IsNullOrWhiteSpace(line) && !line.StartsWith("#"))
                    {
                        writer.WriteLine(line);
                    }
                }
            }
            writer.Flush();
            writer.Dispose();
            TraceLogger.Log($"Total entries in {outputFile}: {File.ReadAllLines(outputFile).Length}");
        }

        public static void ClearFiles(string folder)
        {
            var files = Directory.GetFiles(folder, "*.txt");
            foreach (var file in files)
            {
                try
                {
                    File.Delete(file);
                }
                catch (Exception ex)
                {
                    TraceLogger.Log($"Error deleting file {file}: {ex.Message}", Enums.StatusSeverityType.Error);
                }
            }
            TraceLogger.Log($"Cleared all files in folder: {folder}");
        }

        public static void RemoveDuplicates(string MergedFileLoc)
        {
            TraceLogger.Log("Removing duplicates from merge...");
            TraceLogger.Log($"Total lines in {MergedFileLoc} before removing duplicates: {File.ReadAllLines(MergedFileLoc).Length}");
            if (!File.Exists(MergedFileLoc))
            {
                TraceLogger.Log($"File not found: {MergedFileLoc}", Enums.StatusSeverityType.Warning);
                return;
            }
            var lines = File.ReadAllLines(MergedFileLoc);
            var uniqueLines = new HashSet<string>(lines);
            File.WriteAllLines(MergedFileLoc, uniqueLines);
            TraceLogger.Log($"Removed {lines.Length - uniqueLines.Count} duplicate entries.");
            TraceLogger.Log($"Total lines in {MergedFileLoc} after removing duplicates: {File.ReadAllLines(MergedFileLoc).Length}");
        }
    }
}

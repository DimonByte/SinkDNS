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

namespace SinkDNS.Modules.System
{
    //This will handle folder and file management for SinkDNS, like creating necessary directories.
    internal class IOManager
    {
        public static void CreateNecessaryDirectories()
        {
            string[] directories = ["logs", "config", "resolvers", "blocklists", "backup"];
            foreach (string dir in directories) {
                if (!Directory.Exists(dir))
                {
                    try
                    {
                        Directory.CreateDirectory(dir);
                        TraceLogger.Log($"Created directory: {dir}", Enums.StatusSeverityType.Information);
                    } 
                    catch (Exception ex) 
                    {
                        TraceLogger.Log($"Error creating directory {dir}: {ex.Message}", Enums.StatusSeverityType.Error);
                    }
                }
            }
        }
        public static void MergeMultipleFiles(string outputFilePath, List<string> inputFilePaths)
        {
            if (File.Exists(outputFilePath))
            {
                try
                {
                    TraceLogger.Log($"Deleting existing output file {outputFilePath} before merging.", Enums.StatusSeverityType.Information);
                    File.Delete(outputFilePath); //Ensure that the last combined file isn't added to the merge.
                }
                catch (Exception ex)
                {
                    TraceLogger.Log($"Error deleting existing output file {outputFilePath}: {ex.Message}", Enums.StatusSeverityType.Error);
                    return;
                }
            }
            using var outputStream = File.Create(outputFilePath);
            foreach (var inputFilePath in inputFilePaths)
            {
                using var inputStream = File.OpenRead(inputFilePath);
                TraceLogger.Log($"Merging file {inputFilePath} into {outputFilePath}", Enums.StatusSeverityType.Information);
                inputStream.CopyTo(outputStream);
            }
        }
        public static void BackupFile(string filePath)
        {
            if (File.Exists(filePath))
            {
                string backupFilePath = $"backup/{Path.GetFileName(filePath) + Path.GetExtension(filePath)}.bak";
                try
                {
                    TraceLogger.Log($"Creating backup for file {filePath} at {backupFilePath}", Enums.StatusSeverityType.Information);
                    File.Copy(filePath, backupFilePath, true);
                }
                catch (Exception ex)
                {
                    TraceLogger.Log($"Error creating backup for file {filePath}: {ex.Message}", Enums.StatusSeverityType.Error);
                }
            }
        }
    }
}

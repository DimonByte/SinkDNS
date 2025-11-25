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

namespace SinkDNS.Modules
{
    //This class is responsible for parsing and writing configuration data to and from DNSCrypt.
    //It handles reading configuration files, interpreting settings, and converting them into usable formats for the application.
    internal class DNSCryptConfigurationWriter(string configFilePath)
    {
        private readonly string _configFilePath = configFilePath ?? throw new ArgumentNullException(nameof(configFilePath));

        public void ChangeSetting(string section, string settingName, string value)
        {
            if (!File.Exists(_configFilePath))
                TraceLogger.Log("Configuration file not found!", Enums.StatusSeverityType.Error);

            var lines = File.ReadAllLines(_configFilePath).ToList();
            var modified = false;

            if (string.IsNullOrEmpty(section))
            {
                modified = ModifySettingInSection(lines, null, settingName, value);
            }
            else
            {
                // Search for section and modify setting within it, e.g. [blocked_lists]
                modified = ModifySettingInSection(lines, section, settingName, value);
            }

            if (modified)
            {
                TraceLogger.Log($"Setting '{settingName}' changed to '{value}' in section '{section ?? "global"}'.", Enums.StatusSeverityType.Information);
                File.WriteAllLines(_configFilePath, lines);
                TraceLogger.Log("Configuration file updated successfully.", Enums.StatusSeverityType.Information);
            }
        }

        private bool ModifySettingInSection(List<string> lines, string? section, string settingName, string value)
        {
            bool found = false;
            int startLine = -1;
            int endLine = -1;

            // Find section if specified
            if (!string.IsNullOrEmpty(section))
            {
                for (int i = 0; i < lines.Count; i++)
                {
                    if (lines[i].Trim() == section)
                    {
                        startLine = i;
                        break;
                    }
                }

                if (startLine == -1)
                    return false; // Section not found

                // Find end of section (next section or end of file)
                for (int i = startLine + 1; i < lines.Count; i++)
                {
                    if (lines[i].Trim().StartsWith("[") && lines[i].Trim().EndsWith("]"))
                    {
                        endLine = i;
                        break;
                    }
                }

                if (endLine == -1)
                    endLine = lines.Count;
            }
            else
            {
                startLine = 0;
                endLine = lines.Count;
            }

            // Search for setting within section
            for (int i = startLine; i < endLine; i++)
            {
                var line = lines[i].Trim();
                if (line.StartsWith(settingName + "=") || line.StartsWith(settingName + " ="))
                {
                    lines[i] = $"{settingName} = {FormatValue(value)}";
                    found = true;
                    break;
                }
                else if (line.StartsWith(settingName + "\"") || line.StartsWith(settingName + " \""))
                {
                    // Handle quoted values
                    lines[i] = $"{settingName} = \"{value}\"";
                    found = true;
                    break;
                }
            }

            // If setting not found, add it
            if (!found)
            {
                // Find the end of the section to insert the new setting
                int insertPosition = endLine;
                if (startLine >= 0 && startLine < lines.Count)
                {
                    // Insert after section header
                    insertPosition = startLine + 1;
                }
                lines.Insert(insertPosition, $"{settingName} = {FormatValue(value)}");
            }

            return true;
        }

        private string FormatValue(string value)
        {
            // If value contains spaces or special characters, wrap in quotes
            if (string.IsNullOrEmpty(value))
                return "\"\"";

            if (value.Contains(" ") || value.Contains("=") || value.Contains("#"))
            {
                // Escape quotes in value
                var escapedValue = value.Replace("\"", "\\\"");
                return $"\"{escapedValue}\"";
            }

            return value;
        }

        public string? GetSetting(string section, string settingName)
        {
            if (!File.Exists(_configFilePath))
                throw new FileNotFoundException("Configuration file not found", _configFilePath);

            var lines = File.ReadAllLines(_configFilePath);
            int startLine = -1;
            int endLine = -1;

            // Find section if specified
            if (!string.IsNullOrEmpty(section))
            {
                for (int i = 0; i < lines.Length; i++)
                {
                    if (lines[i].Trim() == section)
                    {
                        startLine = i;
                        break;
                    }
                }

                if (startLine == -1)
                    return null; // Section not found

                // Find end of section (next section or end of file)
                for (int i = startLine + 1; i < lines.Length; i++)
                {
                    if (lines[i].Trim().StartsWith("[") && lines[i].Trim().EndsWith("]"))
                    {
                        endLine = i;
                        break;
                    }
                }

                if (endLine == -1)
                    endLine = lines.Length;
            }
            else
            {
                startLine = 0;
                endLine = lines.Length;
            }

            // Search for setting within section
            for (int i = startLine; i < endLine; i++)
            {
                var line = lines[i].Trim();
                if (line.StartsWith(settingName + "=") || line.StartsWith(settingName + " ="))
                {
                    var value = line.Substring(settingName.Length + 1).Trim();
                    // Remove quotes if present
                    if (value.StartsWith("\"") && value.EndsWith("\""))
                    {
                        return value.Substring(1, value.Length - 2);
                    }
                    return value;
                }
            }

            return null; // Setting not found
        }
        public void BackupDNSCryptConfiguration()
        {
            if (File.Exists(_configFilePath))
            {
                IOManager.BackupFile(_configFilePath);
            }
        }
    }
}

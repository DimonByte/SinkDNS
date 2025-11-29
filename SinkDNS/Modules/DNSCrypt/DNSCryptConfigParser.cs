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

namespace SinkDNS.Modules.DNSCrypt
{
    using SinkDNS.Modules.SinkDNSInternals;

    public class DNSCryptConfigParser(string SinkDNSconfigFilePath, string DNSCryptConfigPath)
    {
        private readonly string _SinkDNSconfigFilePath = SinkDNSconfigFilePath ?? throw new ArgumentNullException(nameof(SinkDNSconfigFilePath));

        private readonly DNSCryptConfigurationWriter _DNSCryptConfigWriter = new(DNSCryptConfigPath);

        private static readonly char[] separator = ['='];

        public void ParseAndApplySettings()
        {
            if (!File.Exists(_SinkDNSconfigFilePath))
            {
                TraceLogger.Log("Configuration file not found!", Enums.StatusSeverityType.Error);
                return;
            }
            TraceLogger.Log($"Reading configuration from {_SinkDNSconfigFilePath}");
            var lines = File.ReadAllLines(_SinkDNSconfigFilePath);
            TraceLogger.Log($"Read {lines.Length} lines from configuration file.");
            var settings = ParseSettings(lines);
            TraceLogger.Log($"Parsed {settings.Count} settings from configuration file.");

            foreach (var setting in settings)
            {
                TraceLogger.Log($"Applying setting: {setting.Key} = '{setting.Value}'");
                _DNSCryptConfigWriter.ChangeSetting(setting.Key, setting.Value);
            }
            _DNSCryptConfigWriter.WriteToConfigFile();
            TraceLogger.Log("All settings have been applied.");
        }

        private static Dictionary<string, string> ParseSettings(string[] lines)
        {
            var settings = new Dictionary<string, string>();

            foreach (var line in lines)
            {
                // Skip empty lines and comments
                if (string.IsNullOrWhiteSpace(line) || line.StartsWith(";") || line.StartsWith("#"))
                    continue;

                // Split on the first '=' to separate key and value
                var parts = line.Split(separator, 2);
                if (parts.Length == 2)
                {
                    var key = parts[0].Trim();
                    var value = parts[1].Trim();

                    settings[key] = value;
                }
            }

            return settings;
        }
    }
}

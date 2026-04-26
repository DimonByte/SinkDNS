using System.Text.Json;
using System.Text.Json.Serialization;
using SinkDNS.Modules.SinkDNSInternals;
using SinkDNS.Properties;

namespace SinkDNS.Modules.DNSCrypt.Data
{
    public enum MetricsType
    {
        TotalQueries,
        RecentQueries,
        TopDomains,
        ServerMetrics,
        QueryTypes
    }

    internal class DNSCryptDataAPI
    {
        private const string ApiUrl = "http://127.0.0.1:8080/api/metrics";
        private static readonly HttpClient Client = new HttpClient(new HttpClientHandler
        {
            Credentials = new System.Net.NetworkCredential(Settings.Default.DNSCryptMonitoringUIUsername, Settings.Default.DNSCryptMonitoringUIPassword)
        });

        public class MetricsData
        {
            [JsonPropertyName("avg_response_time")]
            public double AvgResponseTime { get; set; }

            [JsonPropertyName("blocked_queries")]
            public int BlockedQueries { get; set; }

            [JsonPropertyName("cache_hit_ratio")]
            public double CacheHitRatio { get; set; }

            [JsonPropertyName("cache_hits")]
            public int CacheHits { get; set; }

            [JsonPropertyName("cache_misses")]
            public int CacheMisses { get; set; }

            [JsonPropertyName("queries_per_second")]
            public double QueriesPerSecond { get; set; }

            [JsonPropertyName("query_types")]
            public List<QueryType> QueryTypes { get; set; }

            [JsonPropertyName("recent_queries")]
            public List<RecentQuery> RecentQueries { get; set; }

            [JsonPropertyName("servers")]
            public List<ServerMetrics> Servers { get; set; }

            [JsonPropertyName("top_domains")]
            public List<TopDomain> TopDomains { get; set; }

            [JsonPropertyName("total_queries")]
            public int TotalQueries { get; set; }

            [JsonPropertyName("uptime_seconds")]
            public double UptimeSeconds { get; set; }
        }

        public class QueryType
        {
            [JsonPropertyName("count")]
            public int Count { get; set; }
            [JsonPropertyName("type")]
            public string Type { get; set; }
        }

        public class RecentQuery
        {
            [JsonPropertyName("timestamp")]
            public DateTime Timestamp { get; set; }
            [JsonPropertyName("client_ip")]
            public string ClientIp { get; set; }
            [JsonPropertyName("domain")]
            public string Domain { get; set; }
            [JsonPropertyName("type")]
            public string Type { get; set; }
            [JsonPropertyName("response_code")]
            public string ResponseCode { get; set; }
            [JsonPropertyName("response_time")]
            public double ResponseTime { get; set; }
            [JsonPropertyName("server")]
            public string Server { get; set; }
            [JsonPropertyName("cache_hit")]
            public bool CacheHit { get; set; }
        }

        public class ServerMetrics
        {
            [JsonPropertyName("avg_response_ms")]
            public double AvgResponseMs { get; set; }
            [JsonPropertyName("name")]
            public string Name { get; set; }
            [JsonPropertyName("queries")]
            public int Queries { get; set; }
        }

        public class TopDomain
        {
            [JsonPropertyName("count")]
            public int Count { get; set; }
            [JsonPropertyName("domain")]
            public string Domain { get; set; }
        }

        public static async Task<object> GetData(MetricsType metricsType)
        {
            try
            {
                TraceLogger.Log($"Attempting to fetch {metricsType} data...", Enums.StatusSeverityType.Information);

                string jsonString = await Client.GetStringAsync(ApiUrl);
                TraceLogger.Log("Data fetched successfully. Deserializing...", Enums.StatusSeverityType.Information);
                // Use CaseInsensitive just in case, but JsonPropertyName does the heavy lifting
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var metricsData = JsonSerializer.Deserialize<MetricsData>(jsonString, options);

                if (metricsData == null)
                {
                    TraceLogger.Log("Deserialization returned null.", Enums.StatusSeverityType.Error);
                    //throw new Exception("Deserialization returned null.");
                }
                TraceLogger.Log($"Successfully deserialized data. Extracting {metricsType}...", Enums.StatusSeverityType.Information);
                return metricsType switch
                {
                    MetricsType.TotalQueries => metricsData.TotalQueries,
                    MetricsType.RecentQueries => metricsData.RecentQueries,
                    MetricsType.TopDomains => metricsData.TopDomains,
                    MetricsType.ServerMetrics => metricsData.Servers,
                    MetricsType.QueryTypes => metricsData.QueryTypes,
                    _ => throw new ArgumentOutOfRangeException(nameof(metricsType), $"Type {metricsType} not supported.")
                };
            }
            catch (Exception ex)
            {
                TraceLogger.Log($"Error while retrieving data from DNSCryptAPI: {ex.Message}", Enums.StatusSeverityType.Error);
                return $"[ERROR] {ex.Message}";
            }
        }
    }
    /*
    // Usage
    int total = await DNSCryptDataAPI.GetData(MetricsType.TotalQueries) as int? ?? 0;
    MessageBox.Show($"Total Queries: {total}");

    // To loop through recent queries: //Not tested yet
    var queries = await DNSCryptDataAPI.GetData(MetricsType.RecentQueries) as List<DNSCryptDataAPI.RecentQuery>;
    if (queries != null) {
        foreach (var item in queries) {
            if (item.ResponseCode == "REJECT") {
                listBox1.Items.Add($"{item.Timestamp}: {item.Domain} (REJECTED)");
            }
        }
    }
    */
}

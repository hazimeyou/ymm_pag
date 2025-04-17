using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace YMM4SamplePlugin.AudioEffect
{
    public static class AutoConsoleSender
    {
        static readonly HttpClient client = new HttpClient();
        static readonly string configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.json");
        static string url = "http://127.0.0.1:5000/api/console";

        static AutoConsoleSender()
        {
            try
            {
                if (File.Exists(configPath))
                {
                    var json = File.ReadAllText(configPath);
                    var cfg = JsonSerializer.Deserialize<Config>(json);
                    if (!string.IsNullOrWhiteSpace(cfg.AutoConsoleUrl))
                        url = cfg.AutoConsoleUrl;
                }
            }
            catch { /* 無視 */ }
        }

        public static async Task SendTextAsync(string text)
        {
            try
            {
                var json = JsonSerializer.Serialize(new { text });
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                await client.PostAsync(url, content);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[AutoConsoleSender] エラー: {ex.Message}");
            }
        }

        private class Config
        {
            public string AutoConsoleUrl { get; set; }
        }
    }
}

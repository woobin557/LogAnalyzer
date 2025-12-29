using LogAnalyzer.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace LogAnalyzer.Services
{
    public class LogParser
    {
        public List<LogEntry> Parse(string content)
        {
            var logs = new List<LogEntry>();
            var lines = content.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var raw in lines)
            {
                string line = raw.Trim();

                LogEntry log =
                       ParseJson(line)
                    ?? ParseKeyValue(line)
                    ?? ParseBracket(line)
                    ?? ParseCsv(line)
                    ?? ParseLoose(line)
                    ?? CreateUnknown(line);

                logs.Add(log);
            }

            return logs;
        }

        // ================= JSON =================
        private LogEntry ParseJson(string line)
        {
            if (!line.TrimStart().StartsWith("{")) return null;

            try
            {
                var json = JObject.Parse(line);

                return new LogEntry
                {
                    Time = NormalizeTime(json["time"]?.ToString()),
                    Level = json["level"]?.ToString()?.ToUpper() ?? "UNKNOWN",
                    Message = json["message"]?.ToString() ?? ""
                };
            }
            catch
            {
                return null;
            }
        }

        // ================= Key=Value =================
        private LogEntry ParseKeyValue(string line)
        {
            if (!line.Contains("time=") || !line.Contains("level=")) return null;

            string time = GetValue(line, "time");
            string level = GetValue(line, "level");
            string msg = GetValue(line, "msg");

            if (time == null || level == null) return null;

            return new LogEntry
            {
                Time = NormalizeTime(time),
                Level = level.ToUpper(),
                Message = msg ?? ""
            };
        }

        private string GetValue(string text, string key)
        {
            // 공백 포함 값 허용
            var match = Regex.Match(text, key + @"=(""[^""]+""|[^=]+?)(?=\s\w+=|$)");
            if (!match.Success) return null;

            return match.Groups[1].Value.Trim('"').Trim();
        }

        // ================= [LEVEL] =================
        private LogEntry ParseBracket(string line)
        {
            int s = line.IndexOf('[');
            int e = line.IndexOf(']');

            if (s < 0 || e <= s) return null;

            return new LogEntry
            {
                Time = NormalizeTime(line.Substring(0, s).Trim()),
                Level = line.Substring(s + 1, e - s - 1).Trim(),
                Message = line.Substring(e + 1).Trim()
            };
        }

        // ================= CSV =================
        private LogEntry ParseCsv(string line)
        {
            if (!line.Contains(",")) return null;

            var parts = line.Split(',');
            if (parts.Length < 3) return null;

            return new LogEntry
            {
                Time = NormalizeTime(parts[0].Trim()),
                Level = parts[1].Trim().ToUpper(),
                Message = string.Join(",", parts.Skip(2)).Trim()
            };
        }

        // ================= Loose text =================
        private LogEntry ParseLoose(string line)
        {
            var levelMatch = Regex.Match(line, @"\b(ERROR|WARN|INFO|DEBUG)\b", RegexOptions.IgnoreCase);
            if (!levelMatch.Success) return null;

            var timeMatch = Regex.Match(line, @"\b\d{2}:\d{2}\b");

            return new LogEntry
            {
                Time = timeMatch.Success ? timeMatch.Value : "UNKNOWN",
                Level = levelMatch.Value.ToUpper(),
                Message = line.Replace(levelMatch.Value, "").Replace(timeMatch.Value, "").Trim()
            };
        }

        // ================= UNKNOWN =================
        private LogEntry CreateUnknown(string line)
        {
            return new LogEntry
            {
                Time = "UNKNOWN",
                Level = "UNKNOWN",
                Message = line
            };
        }

        // ================= Time Normalize =================
        private string NormalizeTime(string raw)
        {
            if (string.IsNullOrWhiteSpace(raw)) return "UNKNOWN";

            if (DateTime.TryParse(raw, out DateTime dt))
                return dt.ToString("yyyy-MM-dd HH:mm:ss");

            return raw;
        }
    }
}

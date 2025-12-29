using LogAnalyzer.Services;
using LogAnalyzer.Models;
using LogAnalyzer.Services;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace LogAnalyzer.Views
{
    public partial class MainWindow : Window
    {
        private List<LogEntry> _allLogs = new List<LogEntry>();
        private List<LogEntry> _currentView = new List<LogEntry>();
        private string _lastSelectedLevel = "ALL";

        public MainWindow()
        {
            InitializeComponent();
        }

        // 파일 열기
        private void OpenFile_Click(object sender, RoutedEventArgs e)
        {
            FileService fileService = new FileService();
            string content = fileService.OpenLogFile();
            if (string.IsNullOrEmpty(content)) return;

            LogParser parser = new LogParser();
            _allLogs = parser.Parse(content);
            _lastSelectedLevel = "ALL";
            ApplyFilters();
        }

        // 필터 적용
        private void ApplyFilters()
        {
            if (_allLogs == null) return;

            var logs = _allLogs.AsEnumerable();

            // 레벨 필터
            if (_lastSelectedLevel != "ALL")
                logs = logs.Where(log => !string.IsNullOrEmpty(log.Level) &&
                                         log.Level.Equals(_lastSelectedLevel, StringComparison.OrdinalIgnoreCase));

            // 검색어 필터
            string keyword = SearchBox.Text?.Trim();
            bool caseSensitive = (CaseCheckBox.IsChecked == true);
            StringComparison comparison = caseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;

            if (!string.IsNullOrEmpty(keyword))
            {
                logs = logs.Where(log =>
                {
                    string msg = (log.Message ?? "").Trim();
                    string time = (log.Time ?? "").Trim();
                    return msg.IndexOf(keyword, comparison) >= 0 ||
                           time.IndexOf(keyword, comparison) >= 0;
                });
            }

            _currentView = logs.ToList();
            LogGrid.ItemsSource = null;
            LogGrid.ItemsSource = _currentView;

            UpdateStats();
        }

        // --- 레벨 버튼 ---
        private void AllFilter_Click(object sender, RoutedEventArgs e) { _lastSelectedLevel = "ALL"; ApplyFilters(); }
        private void ErrorFilter_Click(object sender, RoutedEventArgs e) { _lastSelectedLevel = "ERROR"; ApplyFilters(); }
        private void WarnFilter_Click(object sender, RoutedEventArgs e) { _lastSelectedLevel = "WARN"; ApplyFilters(); }
        private void InfoFilter_Click(object sender, RoutedEventArgs e) { _lastSelectedLevel = "INFO"; ApplyFilters(); }

        // --- 검색/옵션 ---
        private void Search_Click(object sender, RoutedEventArgs e) => ApplyFilters();
        private void CaseCheckBox_Changed(object sender, RoutedEventArgs e) => ApplyFilters();

        // --- 정렬 ---
        private void SortTime_Click(object sender, RoutedEventArgs e)
        {
            _currentView = _currentView.OrderBy(l => DateTime.TryParse(l.Time, out DateTime t) ? t : DateTime.MinValue).ToList();
            LogGrid.ItemsSource = null;
            LogGrid.ItemsSource = _currentView;
        }

        private void SortLevel_Click(object sender, RoutedEventArgs e)
        {
            _currentView = _currentView.OrderBy(l => l.Level).ToList();
            LogGrid.ItemsSource = null;
            LogGrid.ItemsSource = _currentView;
        }

        private void SortMessage_Click(object sender, RoutedEventArgs e)
        {
            _currentView = _currentView.OrderBy(l => l.Message).ToList();
            LogGrid.ItemsSource = null;
            LogGrid.ItemsSource = _currentView;
        }

        // --- 통계 ---
        private void UpdateStats()
        {
            int errorCount = _currentView.Count(l => l.Level.Equals("ERROR", StringComparison.OrdinalIgnoreCase));
            int warnCount = _currentView.Count(l => l.Level.Equals("WARN", StringComparison.OrdinalIgnoreCase));
            int infoCount = _currentView.Count(l => l.Level.Equals("INFO", StringComparison.OrdinalIgnoreCase));
            StatsText.Text = $"ERROR: {errorCount} | WARN: {warnCount} | INFO: {infoCount} | Total: {_currentView.Count}";
        }
    }
}
